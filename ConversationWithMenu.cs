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
            AnswerNode chosenAnswerNode = qNode.ChooseAnswer();
            InvokeEvent((qNode, chosenAnswerNode));
            UpdateNumbers(qNode.Effect);
            Game.DisplaySubtitle(chosenAnswerNode.Value);
            if (chosenAnswerNode.EndsConversation)
            {
                DisplayDialogueEnd();
                Graph.OnQuestionChosen(chosenAnswerNode, this); 
                return;
            }
        });
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