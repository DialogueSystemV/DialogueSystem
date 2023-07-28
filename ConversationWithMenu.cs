using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace DialogueSystem;

public class ConversationWithMenu : Conversation
{
    public UIMenu ConversationMenu { get; set; }

    public ConversationWithMenu(DialogueGraph graph, Ped ped,UIMenu ConversationMenu) : base(graph, ped)
    {
        this.ConversationMenu = ConversationMenu;
    }
    
    /// <summary>
    /// This method will add all your questions from a question pool to the menu specified.
    /// </summary>
    /// <param name="menu">Menu you want to add the question to</param>
    /// <param name="q">Question pool the questions will be grabbed from</param>
    public void AddQuestionsToMenu()
    {
        foreach (QuestionNode qandas in Graph.nodes)
        {
            ConversationMenu.AddItem(new UIMenuItem(qandas.Value));
        }
    }
    
    internal void AddQuestionsToMenu(List<QuestionNode> questionsToAdd)
    {
        Graph.AddQuestions(questionsToAdd);
        foreach (QuestionNode n in questionsToAdd)
        {
            ConversationMenu.AddItem(new UIMenuItem(n.Value));
        }
    }

    internal void RemoveQuestionsFromMenu(List<QuestionNode> questionsToRemove)
    {
        Graph.RemoveQuestions(questionsToRemove);
        ConversationMenu.Clear();
        AddQuestionsToMenu();
    }

    internal void OnItemSelect(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        GameFiber.StartNew(delegate
        {
            QuestionNode qNode = Graph.nodes[index];
            if (qNode.EndsConversation)
            {
                EndDialogue();
                return;
            }
            if (qNode.RemoveAfterAsked)
            {
                RemoveQuestionsFromMenu(new List<QuestionNode>(){qNode});
            }
            AnswerNode chosenAnswerNode = qNode.ChooseAnswer(this);
            InvokeEvent((qNode, chosenAnswerNode));
            Game.DisplaySubtitle(chosenAnswerNode.Value);
            if (chosenAnswerNode.EndsConversation)
            {
                EndDialogue();
                OnQuestionChosen(qNode); 
                return;
            }
            OnQuestionChosen(qNode);
            qNode.QuestionAskedAlready = true;
        });
    }
    
    internal override void OnQuestionChosen(QuestionNode qNode)
    {
        if (qNode.QuestionAskedAlready) return;
        var chosenAnswerNode = qNode.chosenAnswer;
        if(chosenAnswerNode.PerformActionIfChosen != null) chosenAnswerNode.PerformActionIfChosen(Ped);
        if(chosenAnswerNode.RemoveTheseQuestionsIfChosen.Count != 0) RemoveQuestionsFromMenu(chosenAnswerNode.RemoveTheseQuestionsIfChosen);
        if(chosenAnswerNode.AddTheseQuestionsIfChosen.Count != 0) AddQuestionsToMenu(chosenAnswerNode.AddTheseQuestionsIfChosen); 
        if(Graph.nodes.Count == 0) EndDialogue();
    }


    public void Activate()
    {
        ConversationMenu.OnItemSelect += OnItemSelect;
        ConversationMenu.Clear();
        AddQuestionsToMenu();
    }


    public override void PauseConversation()
    {
        ConversationMenu.Close();
        ConversationMenu.OnItemSelect -= OnItemSelect;
    }

    public override void ResumeConversation()
    {
        ConversationMenu.OnItemSelect += OnItemSelect;
        ConversationMenu.Visible = true;
    }

    public override void Run()
    {
        Activate();
    }

    public override void EndDialogue()
    {
        ConversationMenu.Clear();
        ConversationMenu.Close();
        ConversationMenu.OnItemSelect -= OnItemSelect;
        base.EndDialogue();
    }
    
}