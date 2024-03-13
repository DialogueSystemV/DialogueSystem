using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace csharpdsa;

public class Conversation
{
    public Graph graph { get; private set; }
    private QuestionNode currNode;
    private bool convoStarted;
    public event EventHandler<(QuestionNode,AnswerNode)> OnQuestionSelect;
    
    public Conversation(Graph graph, QuestionNode currNode)
    {
        this.graph = graph;
        this.currNode = currNode;
        convoStarted = false;
    }
    
    
    public void Run()
    {
            while (true)
            {
                var connectedNodes = graph.GetConnectedNodes(currNode);
                if(!convoStarted || (convoStarted && !currNode.removeQuestionAfterAsked)) connectedNodes.Add(currNode);
                if (!convoStarted)
                {
                    convoStarted = true;
                    graph.startingEdges = graph.edges;
                    graph.startingAdjList = graph.adjList;
                }
                if(connectedNodes.Count == 0)
                {
                    Console.WriteLine("No more questions to ask");
                    break;
                }
                connectedNodes.PrintNodes();
                var indexPressed = WaitForValidKeyPress();
                QuestionNode qNode = connectedNodes[indexPressed];
                Console.WriteLine(qNode.value);
                AnswerNode answer = qNode.ChooseQuestion(graph);
                OnQuestionSelect?.Invoke(this, (qNode, answer));
                Console.WriteLine($" --> {answer.value}");
                Console.WriteLine();
                if (answer.endsConversation)
                {
                    if (answer.action != null) answer.action();
                    break;
                }
            }
            endConvo();
    }

    private void endConvo()
    {
        Console.WriteLine("Conversation Ended");
        foreach(QuestionNode q in graph.nodes)
        {
            q.ResetChosenAnswer();
        }
        graph.edges = graph.startingEdges;
        graph.adjList = graph.startingAdjList;
        convoStarted = false;
    }

    private int WaitForValidKeyPress()
    {
        Console.Write($"\nInput the number of the question you want to ask: ");
        string input = Console.ReadLine();
        return int.Parse(input) - 1;
    }
}