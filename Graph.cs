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

        public void RemoveEdge(Edge edge)
        {
            if (!edges.Contains(edge)) { return; }
            adjList[nodes.IndexOf(edge.to), nodes.IndexOf(edge.from)] = false;
            edges.Remove(edge);
        }

        public void RemoveEdges(HashSet<Edge> edges)
        {
            foreach (Edge e in edges) 
            {
                RemoveEdge(e);
            }
        }

        public void AddEdges(HashSet<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                AddEdge(e);
            }
        }
        
        public void AddNodes(List<QuestionNode> nodes)
        {
            foreach (QuestionNode e in nodes)
            {
                AddNode(e);
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

        public void RemoveNode(QuestionNode n)
        {
            if (!nodes.Contains(n)) { return; }
            int index = nodes.IndexOf(n);
            nodes.RemoveAt(index);
            edges.RemoveWhere(e => e.from.Equals(n) || e.to.Equals(n));
            adjList = new bool[nodes.Count, nodes.Count];
            foreach (var edge in edges.ToList())
            {
                int fromIndex = nodes.IndexOf(edge.from);
                int toIndex = nodes.IndexOf(edge.to);
                adjList[toIndex, fromIndex] = true;
            }
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