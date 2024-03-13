using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace csharpdsa;

public class Conversation
{
    public Graph graph { get; private set; }
    private QuestionNode currNode;
    private QuestionNode startNode;
    private bool convoStarted;
    private bool autoStartWithCurrNode;
    public event EventHandler<(QuestionNode,AnswerNode)> OnQuestionSelect;
    
    public Conversation(Graph graph, QuestionNode currNode, bool autoStartWithCurrNode)
    {
        this.graph = graph;
        this.currNode = currNode;
        startNode = currNode;
        convoStarted = false;
        this.autoStartWithCurrNode = autoStartWithCurrNode;
    }

    private void Start()
    {
        if (convoStarted) return;
        if (!autoStartWithCurrNode)
        {
            convoStarted = true;
            return;
        }
        graph.startingEdges = new HashSet<Edge>(graph.edges);
        graph.CloneAdjList();
        Console.WriteLine(currNode.value);
        AnswerNode answer = currNode.ChooseQuestion(graph);
        Console.WriteLine($" --> {answer.value}");
        Console.WriteLine();
        OnQuestionSelect?.Invoke(this, (currNode, answer));
        convoStarted = true;
        
    }
    
    public void Run()
    {
        Start();
        bool firstTime = true;
        while (true)
        {
            var connectedNodes = graph.GetConnectedNodes(currNode);
            if (!autoStartWithCurrNode && firstTime)
            {
                connectedNodes.Add(currNode);
            }
            AnswerNode answer = null;
            if (connectedNodes.Count == 0)
            {
                Console.WriteLine("No more questions to ask.");
                break;
            }
            connectedNodes.PrintNodes();
            var indexPressed = WaitForValidKeyPress();
            QuestionNode qNode = connectedNodes[indexPressed];
            Console.WriteLine(qNode.value);
            answer = qNode.ChooseQuestion(graph);
            OnQuestionSelect?.Invoke(this, (qNode, answer));
            Console.WriteLine($" --> {answer.value}");
            Console.WriteLine();
            if (answer.endsConversation)
            {
                if (answer.action != null) answer.action();
                break;
            }
            firstTime = false;
        }
        endConvo();
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