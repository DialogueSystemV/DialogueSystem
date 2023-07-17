using System.Runtime.InteropServices;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace DialogueSystem;

public class ConversationWithMenu : Conversation
{
    public UIMenu ConversationMenu { get; set; }

    public ConversationWithMenu(DialogueGraph graph, bool EndNaturally, UIMenu ConversationMenu) : base(graph, EndNaturally)
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
        foreach (QuestionAndAnswers qandas in currNode.QuestionPool)
        {
            ConversationMenu.AddItem(new UIMenuItem(qandas.Question));
        }
    }

    internal void OnItemSelect(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        GameFiber.StartNew(delegate
        {
            QuestionAndAnswers qands = currNode.QuestionPool[index];
            Game.DisplaySubtitle(qands.Question);
            if (currNode.QuestionPool[index].EndsConversation)
            {
                DisplayDialogueEnd();
                return;
            }
            PossibleAnswer chosenAnswer = qands.ChooseAnswer();
            InvokeEvent((qands, chosenAnswer));
            UpdateNumbers(qands.Effect);
            Game.DisplaySubtitle(chosenAnswer.Answer);
            if (chosenAnswer.EndsConversation)
            {
                DisplayDialogueEnd();
                Graph.OnQuestionChosen(chosenAnswer, this);
                return;
            }
            Graph.GetLinkedNode(currNode.Identifier, index, this);
        });
    }

    public void Activate()
    {
        if (base.EndNaturally && CheckIfGraphValid())
        {
            ConversationMenu.OnItemSelect += OnItemSelect;
            AddQuestionsToMenu();
        }
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