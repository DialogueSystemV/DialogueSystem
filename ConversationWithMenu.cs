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
        List<UIMenuItem> newItems = new();
        foreach (QuestionNode n in Graph.nodes)
        {
            newItems.Add(new UIMenuItem(n.Value));
        }
        ConversationMenu.MenuItems = newItems;
        ConversationMenu.RefreshIndex();
    }

    internal void OnItemSelect(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        GameFiber.StartNew(delegate
        {
            QuestionNode qNode = Graph.nodes[index];
            if (qNode.EndsConversation)
            {
                DisplayDialogueEnd();
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
                DisplayDialogueEnd();
                OnQuestionChosen(chosenAnswerNode); 
                return;
            }
            OnQuestionChosen(chosenAnswerNode);
        });
    }
    
    internal override void OnQuestionChosen(AnswerNode chosenAnswerNode)
    {
        if(chosenAnswerNode.PerformActionIfChosen != null) chosenAnswerNode.PerformActionIfChosen(Ped);
        if(chosenAnswerNode.RemoveTheseQuestionsIfChosen.Count != 0) RemoveQuestionsFromMenu(chosenAnswerNode.RemoveTheseQuestionsIfChosen);
        if(chosenAnswerNode.AddTheseQuestionsIfChosen.Count != 0) AddQuestionsToMenu(chosenAnswerNode.AddTheseQuestionsIfChosen);
        if(Graph.nodes.Count == 0) DisplayDialogueEnd();
    }


    public void Activate()
    {
        ConversationMenu.OnItemSelect += OnItemSelect;
        AddQuestionsToMenu();
    }

    public override void Run()
    {
        Activate();
    }

    internal override void DisplayDialogueEnd()
    {
        ConversationMenu.Close();
        base.DisplayDialogueEnd();
    }
    
}