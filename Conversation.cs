using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace csharpdsa;

public class Conversation
{
    public Graph graph { get; private set; }
    private QuestionNode currNode;
    private QuestionNode startNode;
    private bool convoStarted;
    private GameFiber ConversationThread;
    public UIMenu convoMenu;
    private bool initalized;
    public event EventHandler<(QuestionNode,AnswerNode)> OnQuestionSelect;
    
    public Conversation(Graph graph, UIMenu convoMenu, QuestionNode currNode)
    {
        this.graph = graph;
        this.currNode = currNode;
        startNode = currNode;
        convoStarted = false;
        this.convoMenu = convoMenu;
        initalized = false;
    }

    private void Start()
    {
        graph.startingEdges = new HashSet<Edge>(graph.edges);
        graph.CloneAdjList();
        convoMenu.Clear();
        foreach (QuestionNode q in graph.nodes)
        {
            var item = new UIMenuItem(q.value);
            convoMenu.AddItem(item);
        }
        initalized = true;
    }
    
    public void Initialize()
    {
        if (initalized || convoStarted) return;
        Start();
    }
    
    public void Run()
    {
        ConversationThread = GameFiber.StartNew(delegate
        {
            if(!initalized && !convoStarted) Start();
            bool firstTime = true;
            while (true)
            {
                GameFiber.Yield();
                var connectedNodes = graph.GetConnectedNodes(currNode);
                AnswerNode answer = null;
                if (firstTime) connectedNodes.Add(currNode);
                if (connectedNodes.Count == 0 && !firstTime)
                {
                    Game.DisplayHelp("No more questions to ask.");
                    break;
                }

                //connectedNodes.PrintNodes();
                var indexPressed = WaitForValidKeyPress();
                QuestionNode qNode = connectedNodes[indexPressed];
                Game.DisplaySubtitle(qNode.value);
                answer = qNode.ChooseQuestion(graph);
                OnQuestionSelect?.Invoke(this, (qNode, answer));
                Game.DisplaySubtitle(answer.value);
                if (answer.endsConversation)
                {
                    if (answer.action != null) answer.action();
                    break;
                }
                firstTime = false;
                convoStarted = true;
            }

            endConvo();
        });
    }


    private void endConvo()
    {
        Console.WriteLine("Conversation Ended!");
        foreach(QuestionNode q in graph.nodes)
        {
            q.ResetChosenAnswer();
        }
        convoStarted = false;
        graph.edges = graph.startingEdges;
        graph.adjList = graph.startingAdjList;
        currNode = startNode;
    }
    
    private int WaitForValidKeyPress()
    {
        Console.Write($"\nInput the number of the question you want to ask: ");
        string input = Console.ReadLine();
        return int.Parse(input) - 1;
    }
}