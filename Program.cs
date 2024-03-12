using System;
using System.Collections.Generic;

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
            QuestionNode node1 = new QuestionNode("What is your name: 1",false, new List<AnswerNode>(){an1, an2});
            QuestionNode node2 = new QuestionNode("What is your fav movie: 2", false,new List<AnswerNode>(){an3, an4});
            QuestionNode node3 = new QuestionNode("What is your fav food: 3", false,new List<AnswerNode>(){an5, an6});
            
            Edge edge1 = new Edge(node1, node2);
            Edge edge2 = new Edge(node2, node3);
            Edge edge3 = new Edge(node1, node3);
            Edge edge4 = new Edge(node2, node1);

            an2.edgesToAdd.Add(edge3);
            
            
            var l = new List<QuestionNode>()
            {
                node1, node2, node3
            };
            var f = new HashSet<Edge>()
            {
                
            };
            GraphConfig config = new GraphConfig();
            config.AddVariable("fav", "favorite");
            Graph graph = new Graph(l,f, config);
            // var adjList = graph.adjList;
            // for (int i = 0; i < adjList.GetLength(0); i++)
            // {
            //     for (int j = 0; j < adjList.GetLength(1); j++)
            //     {
            //         Console.Write(adjList[i, j] + "              ");
            //     }
            //     Console.WriteLine();
            // }

            AnswerNode answer = node1.ChooseQuestion(graph);
            Console.WriteLine(answer.Value);
            var e = graph.GetConnectedNodes(node1);
            foreach (var k in e)
            {
                Console.WriteLine(k.Value);                
            }
            // graph.RemoveEdge(edge3);
            // Console.WriteLine("-------------------");
            // var fg = graph.GetConnectedNodes(node1);
            // foreach (var k in fg)
            // {
            //     Console.WriteLine(k.Value);                
            // }
            // Console.WriteLine("-------------------");
            // graph.AddEdge(edge3);
            // fg = graph.GetConnectedNodes(node1);
            // foreach (var VARIABLE in fg)
            // {
            //     Console.WriteLine(VARIABLE.Value);
            // }
        }
    }
  
}