using static DialogueSystem.ListExtensions;
namespace DialogueSystem
{
    public class Graph
    {
        internal HashSet<Edge> edges;
        internal HashSet<Edge> startingEdges;
        internal List<QuestionNode> nodes;
        internal bool[,] adjList;
        internal bool[,] startingAdjList;
        
        
        /// <summary>
        /// GraphConfig that will allow to use variables in the questions and answers
        /// </summary>
        public GraphConfig vars {get; set;}

        public Graph(List<QuestionNode> nodes, HashSet<Edge> edges, GraphConfig config)
        {
            vars = config;
            this.edges = new HashSet<Edge>();
            this.nodes = new List<QuestionNode>();
            AddNodes(nodes);
            adjList = new bool[this.nodes.Count, this.nodes.Count];
            AddEdges(edges);
        }
        
        /// <summary>
        /// Adds a(n) link(edge) between the specified nodes
        /// </summary>
        /// <param name="fromNode">The source node</param>
        /// <param name="toNode">The destination node</param>
        public void LinkQuestions(QuestionNode fromNode, QuestionNode toNode)
        {
            AddEdge(new Edge(fromNode, toNode));
        }
        
        /// <summary>
        /// Removes a(n) link(edge) between the specified nodes
        /// </summary>
        /// <param name="fromNode">The source node</param>
        /// <param name="toNode">The destination node</param>
        public void RemoveLink(QuestionNode fromNode, QuestionNode toNode)
        {
            RemoveEdge(new Edge(fromNode, toNode));
        }

        /// <summary>
        /// Gets all connected questions from the specified node
        /// </summary>
        /// <param name="n">The node to get connected nodes from</param>
        /// <returns>List of connected nodes.</returns>
        public List<QuestionNode> GetAllConnectedQuestionsFromNode(QuestionNode n)
        {
            return GetConnectedNodes(n);
        }
        
        /// <summary>
        /// Removes all links(edges) connected to the specified node
        /// </summary>
        /// <param name="n">The node to remove all links from</param>
        public void RemoveAllLinksFromQuestion(QuestionNode n)
        {
            int index = nodes.IndexOf(n);
            for (int i = 0; i < adjList.GetLength(1); i++)
            {
                adjList[index, i] = false; 
            }
        }
        

        
        internal void AddEdge(Edge edge)
        {
            if (edges.Add(edge))
            {
                adjList[nodes.IndexOf(edge.to), nodes.IndexOf(edge.from)] = true;
            }
        }

        internal void RemoveEdge(Edge edge)
        {
            if (!edges.Contains(edge)) { return; }
            adjList[nodes.IndexOf(edge.to), nodes.IndexOf(edge.from)] = false;
            edges.Remove(edge);
        }
        
        internal void RemoveEdges(HashSet<Edge> edges)
        {
            foreach (Edge e in edges) 
            {
                RemoveEdge(e);
            }
        }
        
        internal void RemoveEdges(List<Edge> edges)
        {
            foreach (Edge e in edges) 
            {
                RemoveEdge(e);
            }
        }

        internal void AddEdges(HashSet<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                AddEdge(e);
            }
        }
        
        internal void AddEdges(List<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                AddEdge(e);
            }
        }
        
        private void AddNodes(List<QuestionNode> nodes)
        {
            foreach (QuestionNode e in nodes)
            {
                AddNode(e, true);
            }
        }

        private bool AddNode(QuestionNode n, bool partOfList = false)
        {
            if (nodes.Contains(n))
            {
                return false;
            }
            nodes.Add(n);
            vars.ReplaceVariables(n);
            foreach(var na in n.possibleAnswers) {vars.ReplaceVariables(na);}
            if (!partOfList) {RedoAdjList();}
            return true;
        }

        private void RedoAdjList()
        {
            adjList = new bool[nodes.Count, nodes.Count];
            foreach (var edge in edges.ToList())
            {
                int fromIndex = nodes.IndexOf(edge.from);
                int toIndex = nodes.IndexOf(edge.to);
                adjList[toIndex, fromIndex] = true;
            }
        }

        private void RemoveNode(QuestionNode n)
        {
            if (!nodes.Contains(n)) { return; }
            int index = nodes.IndexOf(n);
            nodes.RemoveAt(index);
            edges.RemoveWhere(e => e.from.Equals(n) || e.to.Equals(n));
            RedoAdjList();
        }

        internal List<QuestionNode> GetConnectedNodes(QuestionNode node)
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
        
        internal void CloneAdjList()
        {
            int rows = adjList.GetLength(0);
            int cols = adjList.GetLength(1);

            bool[,] newArray = new bool[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    newArray[i, j] = adjList[i, j];
                }
            }
            startingAdjList = newArray;
        }
    }
}