using System.Runtime.InteropServices;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace DialogueSystem;

public class ConversationWithMenu : Conversation
{
    public UIMenu ConversationMenu { get; set; }

    public ConversationWithMenu(DialogueGraph graph, UIMenu ConversationMenu) : base(graph)
    {
        this.ConversationMenu = ConversationMenu;
        this.ConversationMenu.OnItemSelect += OnItemSelect;
        AddQuestionsToMenu();
    }
    
    /// <summary>
    /// This method will add all your questions from a question pool to the menu specified.
    /// </summary>
    /// <param name="menu">Menu you want to add the question to</param>
    /// <param name="q">Question pool the questions will be grabbed from</param>
    public void AddQuestionsToMenu()
    {
        foreach (QuestionAndAnswers qandas in currNode.QuestionPool)
        {
            ConversationMenu.AddItem(new UIMenuItem(qandas.Question));
        }
    }

    internal void OnItemSelect(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        GameFiber.StartNew(delegate
        {
            Game.DisplaySubtitle(currNode.QuestionPool[index].Question);
            if (currNode.QuestionPool[index].EndsConversation)
            {
                DisplayDialogueEnd();
                return;
            }

            PossibleAnswer chosenAnswer = currNode.QuestionPool[index].ChooseAnswer();
            InvokeEvent((currNode.QuestionPool[index], chosenAnswer));
            UpdateNumbers(currNode.QuestionPool[index].Effect);
            Game.HideHelp();
            Game.DisplaySubtitle(chosenAnswer.Answer);
            Graph.OnQuestionChosen(chosenAnswer);
            if (chosenAnswer.EndsConversation)
            {
                DisplayDialogueEnd();
                return;
            }

            Graph.GetLinkedNode(currNode.Identifier, index, this);
        });
    }

    internal override void DisplayDialogueEnd()
    {
        ConversationMenu.Close();
        base.DisplayDialogueEnd();
    }
    
}