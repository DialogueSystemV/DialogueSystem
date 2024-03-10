using System;
using System.Collections.Generic;
using System.Linq;

namespace csharpdsa
{
    public class Graph
    {
        public HashSet<Edge> edges;
        public List<QuestionNode> nodes;
        public bool[,] adjList;


        public Graph(List<QuestionNode> nodes, HashSet<Edge> edges)
        {
            this.edges = new HashSet<Edge>();
            this.nodes = new List<QuestionNode>();
            AddNodes(nodes);
            adjList = new bool[this.nodes.Count, this.nodes.Count];
            AddEdges(edges);
        }

        public void AddEdge(Edge edge)
        {
            if (edges.Add(edge))
            {
                adjList[nodes.IndexOf(edge.to), nodes.IndexOf(edge.from)] = true;
            }
        }

        public void AddEdges(HashSet<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                this.AddEdge(e);
            }
        }
        
        public void AddNodes(List<QuestionNode> nodes)
        {
            foreach (QuestionNode e in nodes)
            {
                this.AddNode(e);
            }
        }

        public bool AddNode(QuestionNode n)
        {
            if (nodes.Contains(n))
            {
                return false;
            }
            nodes.Add(n);
            return true;
        }

        public bool RemoveNode(QuestionNode n)
        {
            if (!nodes.Contains(n)) { return false; }
            int index = nodes.IndexOf(n);
            nodes.RemoveAt(index);
            edges.RemoveWhere(e => e.from.Equals(n) || e.to.Equals(n));
            adjList = new bool[this.nodes.Count, this.nodes.Count];
            foreach (var edge in edges.ToList())
            {
                int fromIndex = nodes.IndexOf(edge.from);
                int toIndex = nodes.IndexOf(edge.to);
                adjList[toIndex, fromIndex] = true;
            }
            return true;
        }

        public List<QuestionNode> GetConnectedNodes(QuestionNode node)
        {
            int colIndex = nodes.IndexOf(node);
            if (colIndex == -1)
            {
                // If the index is out of range, return an empty list
                return new List<QuestionNode>();
            }
            int colLength = adjList.GetLength(1);
            List<QuestionNode> result = new List<QuestionNode>();
            for (int i = 0; i < colLength; i++)
            {
                if (adjList[i, colIndex])
                {
                    result.Add(nodes[i]);
                }
            }
    
            return result;
        }
        
        
        
    }
}