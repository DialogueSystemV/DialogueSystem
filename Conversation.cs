using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace DialogueSystem;

public class Conversation
{
    public Graph graph { get; private set; }
    private QuestionNode currNode;
    private QuestionNode[] startNodes;
    public bool convoStarted { get; private set; }
    public UIMenu convoMenu;
    private List<QuestionNode> connectedNodes;
    private bool firstTime;
    public event EventHandler<(QuestionNode,AnswerNode)> OnQuestionSelect;
    public event EventHandler OnCoversationEnded;
    
    public Conversation(Graph graph, UIMenu convoMenu, params QuestionNode[] startNodes)
    {
        this.graph = graph;
        currNode = null;
        this.startNodes = startNodes;
        convoStarted = false;
        this.convoMenu = convoMenu;
        connectedNodes = new List<QuestionNode>();
        firstTime = true;
    }

    public void Init()
    {
        if (convoStarted) return;
        graph.startingEdges = new HashSet<Edge>(graph.edges);
        graph.CloneAdjList();
        UpdateMenu(true);
    }

    private void UpdateMenu(bool start = false)
    {
        convoMenu.Clear();
        connectedNodes = graph.GetConnectedNodes(currNode);
        foreach (var item in start ? startNodes.ToList() : connectedNodes)
        {
            convoMenu.AddItem(new UIMenuItem(item.value));
        }
    }
    
    public void Run()
    {
        convoMenu.OnItemSelect += OnItemSelect;
        Game.DisplaySubtitle("Subbing to event");
    }

    private void OnItemSelect(UIMenu uiMenu, UIMenuItem selectedItem, int index)
    {
        Game.LogTrivial("In Dialogue System item select");
        AnswerNode answer = null;
        QuestionNode qNode = firstTime ? startNodes.ToList()[index] : connectedNodes[index];
        currNode = qNode;
        Game.DisplaySubtitle(qNode.value);
        answer = qNode.ChooseQuestion(graph);
        OnQuestionSelect?.Invoke(this, (qNode, answer));
        Game.DisplaySubtitle(answer.value);
        if (answer.action != null) answer.action();
        if (answer.endsConversation)
        {
            EndConvo();
            return;
        }

        UpdateMenu();
        if (connectedNodes.Count == 0)
        {
            EndConvo();
        }
        convoStarted = true;
        firstTime = false;
    }

    private void EndConvo()
    {
        convoMenu.Close();
        Game.DisplaySubtitle("Conversation Ended");
        foreach(QuestionNode q in graph.nodes)
        {
            q.ResetChosenAnswer();
        }
        graph.edges = graph.startingEdges;
        graph.adjList = graph.startingAdjList;
        convoMenu.OnItemSelect -= OnItemSelect;
        convoStarted = false;
        firstTime = true;
        currNode = null;
        OnCoversationEnded?.Invoke(this, EventArgs.Empty);
    }
    
}