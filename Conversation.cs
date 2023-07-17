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
    /// Number of positive questions asked by the player. The starting value is 0.
    /// </summary>
    public int NumberOfPositive { get; private set; }
    /// <summary>
    /// Number of negative questions asked by the player. The starting value is 0.
    /// </summary>
    public int NumberOfNegative { get; private set; }
    /// <summary>
    /// Number of neutral questions asked by the player. The starting value is 0.
    /// </summary>
    public int NumberOfNeutral { get; private set; }
    /// <summary>
    /// List of question pools. This will be looped through in the Run() method.
    /// </summary>
    public DialogueGraph Graph { get; set; }
    
    public event EventHandler<(QuestionAndAnswers, PossibleAnswer)> OnQuestionSelect;
    
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

    internal Node currNode;

    private bool IsGraphValid;

    /// <summary>
    /// Initializes an instance of the Conversation object 
    /// </summary>
    /// <param name="dialouge">This is the dialogue that you want to take place. It has to be a <code>List<QuestionPool></code>.</param>
    /// <param name="useNumpadKeys">This is a boolean to either use numpad keys or not. This could be part of an ini setting.</param>
    public Conversation(DialogueGraph graph)
    {
        Graph = graph;
        NumberOfNegative = 0;
        NumberOfNeutral = 0;
        NumberOfPositive = 0;
        currNode = Graph.nodes[0];
    }
    
    
    /// <summary>
    /// Initializes an instance of the Conversation object 
    /// </summary>
    /// <param name="dialouge">This is the dialogue that you want to take place. It has to be a <code>List<QuestionPool></code>.</param>
    /// <param name="useNumpadKeys">This is a boolean to either use numpad keys or not. This could be part of an ini setting.</param>
    public Conversation(DialogueGraph graph,bool useNumpadKeys)
    {
        Graph = graph;
        NumberOfNegative = 0;
        NumberOfNeutral = 0;
        NumberOfPositive = 0;
        if (useNumpadKeys)
        {
            _validKeys = _numpadKeys;
        }
        currNode = Graph.nodes[0];
    }
    
    
    internal void UpdateNumbers(QuestionEffect effect)
    {
        switch (effect)
        {
            case QuestionEffect.Positive:
                NumberOfPositive++;
                break;
            case QuestionEffect.Neutral:
                NumberOfNeutral++;
                break;
            case QuestionEffect.Negative:
                NumberOfNegative++;
                break;
        }
    }

    internal bool CheckIfGraphValid()
    {
        if (!IsGraphValid)
        {
            Game.DisplayNotification("Dialogue System does not have any \"conversation enders\". Dialogue System will not be running. " +
                                     "Please report this to the dev.");
            return false;
        }

        return true;
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
                if (Game.IsKeyDown(key) && currNode.IsValidIndex(i))
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
                Game.DisplayHelp(currNode.DisplayQuestions(), 10000);
                int indexPressed = WaitForValidKeyPress();
                QuestionAndAnswers qands = currNode.QuestionPool[indexPressed];
                Game.HideHelp();
                Game.DisplaySubtitle(qands.Question);
                if (qands.EndsConversation)
                {
                    DisplayDialogueEnd();
                    break;
                }
                PossibleAnswer chosenAnswer = qands.ChooseAnswer();
                OnQuestionSelect?.Invoke(this, (qands, chosenAnswer));
                UpdateNumbers(qands.Effect);
                Game.HideHelp();
                Game.DisplaySubtitle(chosenAnswer.Answer);
                if (chosenAnswer.EndsConversation)
                {
                    DisplayDialogueEnd();
                    Graph.OnQuestionChosen(chosenAnswer, this);
                    break;
                }
                Graph.GetLinkedNode(currNode.Identifier, indexPressed, this);
            }

        });
    }
    
    public void InterruptConversation()
    {
        try
        {
            Game.HideHelp();
            if (ConversationThread.IsAlive) ConversationThread.Abort();
        }
        catch (ThreadAbortException TAE)
        {
            Game.LogTrivial("Conversation interrupted");            
        }
    }
    
    internal void InvokeEvent((QuestionAndAnswers, PossibleAnswer) e)
    {
        OnQuestionSelect?.Invoke(this,e);
    }

    internal virtual void DisplayDialogueEnd()
    {
        Game.DisplaySubtitle("~y~CONVERSATION OVER");
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