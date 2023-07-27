using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Rage;
using Rage.Native;

namespace DialogueSystem;

public class Conversation
{
    /// <summary>
    /// List of question pools. This will be looped through in the Run() method.
    /// </summary>
    public DialogueGraph Graph { get; set; }
    
    public event EventHandler<(QuestionNode,AnswerNode)> OnQuestionSelect;
    
    private static Keys[] _validKeys = new[]
    {
        Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
    };
    
    private static Keys[] _numpadKeys = new[]
    {
        Keys.NumPad0,Keys.NumPad1,Keys.NumPad2,Keys.NumPad3,Keys.NumPad4,Keys.NumPad5,Keys.NumPad6,Keys.NumPad7,Keys.NumPad8,Keys.NumPad9
    };

    internal GameFiber ConversationThread;

    private static DateTime lastTimePressed = DateTime.MinValue;
    
    public Ped Ped { get; set; }

    /// <summary>
    /// Initializes an instance of the Conversation object 
    /// </summary>
    /// <param name="dialouge">This is the dialogue that you want to take place. It has to be a <code>List<QuestionPool></code>.</param>
    /// <param name="useNumpadKeys">This is a boolean to either use numpad keys or not. This could be part of an ini setting.</param>
    public Conversation(DialogueGraph graph, Ped Ped)
    {
        Graph = graph;
        this.Ped = Ped;
        if (Ped == null)
        {
            throw new ArgumentNullException("Ped cannot be null");
        }
    }
    
    
    /// <summary>
    /// Initializes an instance of the Conversation object 
    /// </summary>
    /// <param name="dialouge">This is the dialogue that you want to take place. It has to be a <code>List<QuestionPool></code>.</param>
    /// <param name="useNumpadKeys">This is a boolean to either use numpad keys or not. This could be part of an ini setting.</param>
    public Conversation(DialogueGraph graph,bool useNumpadKeys, Ped Ped)
    {
        Graph = graph;
        if (useNumpadKeys)
        {
            _validKeys = _numpadKeys;
        }
        this.Ped = Ped;
        if (Ped == null)
        {
            throw new ArgumentNullException("Ped cannot be null");
        }
    }

    private int WaitForValidKeyPress()
    {
        bool isValidKeyPressed = false;
        int indexPressed = 0;
        DisableControlAction(2,37,true);
        while (!isValidKeyPressed)
        {
            GameFiber.Yield();
            if (DateTime.Now < lastTimePressed.AddSeconds(2.5))
            {
                continue;
            }
            lastTimePressed = DateTime.Now;
            for (int i = 0; i < _validKeys.Length; i++)
            {
                Keys key = _validKeys[i];
                if (Game.IsKeyDown(key) && Graph.IsValidIndex(i))
                {
                    isValidKeyPressed = true;
                    indexPressed = i;
                    break;
                }
            }
        }
        EnableControlAction(2,37,true);
        return indexPressed;
    }

    /// <summary>
    /// This method runs the dialogue in a new GameFiber. This method will iterate through all your question pools and updates the number of question integers for each category.
    /// </summary>
    public virtual void Run()
    {
        ConversationThread = GameFiber.StartNew(delegate
        {
            while (true)
            {
                GameFiber.Yield();
                Game.DisplayHelp(Graph.DisplayQuestions(), 10000);
                var indexPressed = WaitForValidKeyPress();
                QuestionNode qNode = Graph.nodes[indexPressed];
                Game.HideHelp();
                Game.DisplaySubtitle(qNode.Value);
                if (qNode.EndsConversation)
                {
                    EndDialogue();
                    break;
                }
                if (qNode.RemoveAfterAsked)
                {
                    Graph.RemoveQuestions(new List<QuestionNode>(){qNode});
                }
                AnswerNode chosenAnswer = qNode.ChooseAnswer(this);
                OnQuestionSelect?.Invoke(this, (qNode, chosenAnswer));
                Game.HideHelp();
                Game.DisplaySubtitle(chosenAnswer.Value);
                if (chosenAnswer.EndsConversation)
                {
                    EndDialogue();
                    OnQuestionChosen(qNode);
                    break;
                }
                OnQuestionChosen(qNode);
                qNode.QuestionAskedAlready = true;
            }

        });
    }
    
    public virtual void PauseConversation()
    {
        try
        {
            Game.HideHelp();
            if (ConversationThread.IsAlive) ConversationThread.Abort();
        }
        catch (ThreadAbortException)
        {
            Game.LogTrivial("Conversation interrupted");            
        }
    }

    public virtual void ResumeConversation()
    {
        Run();
    }
    
    internal void InvokeEvent((QuestionNode, AnswerNode) e)
    {
        OnQuestionSelect?.Invoke(this,e);
    }

    public virtual void EndDialogue()
    {
        Game.DisplaySubtitle("~y~CONVERSATION OVER");
        foreach (var node in Graph.nodes)
        {
            node.chosenAnswer = null;
            node.QuestionAskedAlready = false;
        }
        Graph.nodes.Clear();
        Graph.allNodes.Clear();
    }
    
    internal virtual void OnQuestionChosen(QuestionNode qNode)
    {
        if (qNode.QuestionAskedAlready) return;
        var chosenAnswerNode = qNode.chosenAnswer;
        if(chosenAnswerNode.PerformActionIfChosen != null) chosenAnswerNode.PerformActionIfChosen(Ped);
        if(chosenAnswerNode.RemoveTheseQuestionsIfChosen.Count != 0) Graph.RemoveQuestions(chosenAnswerNode.RemoveTheseQuestionsIfChosen);
        if(chosenAnswerNode.AddTheseQuestionsIfChosen.Count != 0) Graph.AddQuestions(chosenAnswerNode.AddTheseQuestionsIfChosen);
        if(Graph.nodes.Count == 0) EndDialogue();
    }
    
    private void EnableControlAction(int control, int action, bool enable)
    {
        NativeFunction.Natives.ENABLE_CONTROL_ACTION(control, action, enable);
    }
    private void DisableControlAction(int control, int action, bool disable)
    {
        NativeFunction.Natives.DISABLE_CONTROL_ACTION(control, action, disable);
    }
}