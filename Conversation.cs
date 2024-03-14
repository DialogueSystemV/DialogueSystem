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
    private bool convoStarted;
    public UIMenu convoMenu;
    internal GameFiber convoThread;
    private List<QuestionNode> connectedNodes;
    private bool firstTime;
    public event EventHandler<(QuestionNode,AnswerNode)> OnQuestionSelect;
    
    public Conversation(Graph graph, UIMenu convoMenu, params QuestionNode[] startNodes)
    {
        this.graph = graph;
        currNode = null;
        this.startNodes = startNodes;
        convoStarted = false;
        this.convoMenu = convoMenu;
        connectedNodes = new List<QuestionNode>();
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
        convoMenu.Visible = true;
    }

    private void OnItemSelect(UIMenu uiMenu, UIMenuItem selectedItem, int index)
    {
        convoThread = new GameFiber(delegate
        {
            AnswerNode answer = null;
            QuestionNode qNode = firstTime ? startNodes.ToList()[index] : connectedNodes[index];
            currNode = qNode;
            //Console.WriteLine(qNode.value);
            Game.DisplaySubtitle(qNode.value);
            answer = qNode.ChooseQuestion(graph);
            OnQuestionSelect?.Invoke(this, (qNode, answer));
            Game.DisplaySubtitle(answer.value);
            //Console.WriteLine($" --> {answer.value}");
            //Console.WriteLine();
            if (answer.action != null) answer.action();
            if (answer.endsConversation)
            {
                EndConvo();
                return;
            }

            UpdateMenu();
            convoStarted = true;
            firstTime = false;
        });
    }

    private void EndConvo()
    {
        // Console.WriteLine("Conversation Ended!");
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
    }
    
    // private int WaitForValidKeyPress()
    // {
    //     Console.Write($"\nInput the number of the question you want to ask: ");
    //     string input = Console.ReadLine();
    //     return int.Parse(input) - 1;
    // }
    
    
}