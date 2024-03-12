using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace csharpdsa;

public class Conversation
{
    public Graph graph { get; private set; }
    private QuestionNode currNode;
    private bool convoStarted;
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
                if(!convoStarted || (convoStarted && !currNode.RemoveQuestionAfterAsked)) connectedNodes.Add(currNode);
                convoStarted = true;
                if(connectedNodes.Count == 0)
                {
                    Console.WriteLine("No more questions to ask");
                    break;
                }
                connectedNodes.PrintNodes();
                var indexPressed = WaitForValidKeyPress();
                QuestionNode qNode = connectedNodes[indexPressed];
                Console.WriteLine(qNode.Value);
                AnswerNode answer = qNode.ChooseQuestion(graph);
                if (qNode.EndsConversation)
                {
                    break;
                }
                Console.WriteLine($" --> {answer.Value}");
                Console.WriteLine();
                if (answer.EndsConversation)
                {
                    break;
                }

            }

    }

    private int WaitForValidKeyPress()
    {
        Console.Write($"\nInput the number of the question you want to ask: ");
        string input = Console.ReadLine();
        return int.Parse(input) - 1;
    }
}