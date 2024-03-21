using System;
using System.Collections.Generic;
using RAGENativeUI;

namespace csharpdsa
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AnswerNode an1 = new AnswerNode("Roheat", 40);
            AnswerNode an2 = new AnswerNode("Rohit", 60);
            AnswerNode an3 = new AnswerNode("Mission Impossible", 50);
            AnswerNode an4 = new AnswerNode("Star Wars", 50);
            AnswerNode an5 = new AnswerNode("French Fries", 50);
            AnswerNode an6 = new AnswerNode("Idli", 50);
            AnswerNode an7 = new AnswerNode("Yes", 100);
            QuestionNode node1 = new QuestionNode("What is your name?",true, an1, an2);
            QuestionNode node2 = new QuestionNode("Are you a witness?",true, an1, an2);
            QuestionNode node3 = new QuestionNode("What is your fav movie?", true, an3, an4);
            QuestionNode node4 = new QuestionNode("What is your fav food?", true, an5, an6);
            Edge edge5 = new Edge(node1, node3);
            Edge edge2 = new Edge(node1, node4);            
            
            var l = new List<QuestionNode>()
            {
                node1, node2, node3, node4
            };
            var f = new HashSet<Edge>()
            { 
                edge2, edge5
            };
            GraphConfig config = new GraphConfig();
            config.AddVariable("fav", "favorite");
            Graph graph = new Graph(l,f, config);

            Conversation convo = new Conversation(graph, node1, node2);
            convo.Run();
            // var adjList = graph.adjList;
            // for (int i = 0; i < adjList.GetLength(0); i++)
            // {
            //     for (int j = 0; j < adjList.GetLength(1); j++)
            //     {
            //         Console.Write(adjList[i, j] + "              ");
            //     }
            //     Console.WriteLine();
            // }
            // graph.RemoveEdge(edge3);
            // Console.WriteLine("-------------------");
            // var fg = graph.GetConnectedNodes(node1);
            // foreach (var k in fg)
            // {
            //     Console.WriteLine(k.Value);                
            // }
            // Console.WriteLine("-------------------");
            // graph.AddEdge(edge3);
        }
    }
  
}