using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAGENativeUI;

namespace csharpdsa
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "dialogue.json"; // Make sure this path is correct

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found at {filePath}");
                return;
            }

            string jsonContent;
            try
            {
                jsonContent = File.ReadAllText(filePath);
                Console.WriteLine($"Successfully read JSON from {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return;
            }

            try
            {
                Graph graph = DialogueLoader.ParseGraphManually(jsonContent);

                Console.WriteLine("\n--- Manual Parsing Results ---");
                Console.WriteLine($"Total Nodes: {graph.nodes.Count}");
                Console.WriteLine($"Total Edges: {graph.edges.Count}");
                Console.WriteLine(graph.GetConnectedNodes(graph.nodes[0]));
                foreach (Node node in graph.nodes)
                {
                    Console.WriteLine(
                        $"  Question Text: '{node.value ?? "N/A"}'"); // Display questionText
                    if (node is QuestionNode questionNode)
                    {
                        Console.WriteLine(
                            $"  Is QuestionNode. Answers count: {questionNode.possibleAnswers.Count}");
                        foreach (var answer in questionNode.possibleAnswers)
                        {
                            Console.WriteLine(
                                $"    Answer: {answer.value} (Prob: {answer.probability})");
                        }
                    }
                }

                foreach (Edge edge in graph.edges)
                {
                    Console.WriteLine($"\nEdge ID: {edge.Id}");
                    Console.WriteLine($"  From: {edge.from.value}");
                    Console.WriteLine($"  To: {edge.to.value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during manual parsing: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
