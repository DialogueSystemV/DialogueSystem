namespace DialogueSystem.Core
{
    public class Graph
    {
        internal List<Edge> edges;
        internal List<Edge> startingEdges;
        internal List<QuestionNode> nodesToStartConversation;
        internal List<QuestionNode> nodes;
        internal bool[,] adjList;
        internal bool[,] startingAdjList;

        /// <summary>
        /// GraphConfig that will allow to use variables in the questions and answers
        /// </summary>
        public GraphConfig vars { get; set; }

        public Graph(List<QuestionNode> nodes, List<Edge> links, GraphConfig config)
        {
            vars = config;
            this.edges = new List<Edge>();
            this.nodes = new List<QuestionNode>();
            AddNodes(nodes);
            adjList = new bool[this.nodes.Count, this.nodes.Count];
            AddEdges(links);
        }

        /// <summary>
        /// Adds a(n) link(edge) between the specified nodes
        /// </summary>
        public void LinkQuestions(QuestionNode fromNode, QuestionNode toNode)
        {
            AddEdge(new Edge(fromNode, toNode));
        }

        /// <summary>
        /// Removes a(n) link(edge) between the specified nodes
        /// </summary>
        public void RemoveLink(QuestionNode fromNode, QuestionNode toNode)
        {
            RemoveEdge(new Edge(fromNode, toNode));
        }

        /// <summary>
        /// Gets all connected questions from the specified node
        /// </summary>
        public List<QuestionNode> GetAllConnectedQuestionsFromNode(QuestionNode n)
        {
            return GetConnectedNodes(n);
        }

        /// <summary>
        /// Removes all links(edges) connected to the specified node
        /// </summary>
        public void RemoveAllLinksFromQuestion(QuestionNode n)
        {
            int index = GetNodeIndexById(n.ID);
            if (index == -1)
            {
                return;
            }

            for (int i = 0; i < adjList.GetLength(1); i++)
            {
                adjList[index, i] = false;
            }
        }

        /// <summary>
        /// Returns the index of a node based on its string ID
        /// </summary>
        internal int GetNodeIndexById(string id)
        {
            return nodes.FindIndex(n => n.ID == id);
        }

        internal void AddEdge(Edge edge)
        {
            int fromIndex = GetNodeIndexById(edge.from.ID);
            int toIndex = GetNodeIndexById(edge.to.ID);

            Rage.Game.LogTrivial(
                $"[DialogueSystem][Graph:AddEdge] TRY | From='{edge.from?.value}' -> To='{edge.to?.value}' | " +
                $"FromIndex={fromIndex} | ToIndex={toIndex} | AlreadyExists={edges.Contains(edge)}"
            );

            if (!edges.Contains(edge))
            {
                if (fromIndex == -1 || toIndex == -1)
                {
                    Rage.Game.LogTrivial(
                        $"[DialogueSystem][Graph:AddEdge] FAILED | Invalid index | FromIndex={fromIndex} | ToIndex={toIndex}"
                    );
                    return;
                }

                adjList[fromIndex, toIndex] = true;
                edges.Add(edge);

                Rage.Game.LogTrivial(
                    $"[DialogueSystem][Graph:AddEdge] ADDED | From='{edge.from.value}' -> To='{edge.to.value}'"
                );
            }
        }

        internal void RemoveEdge(Edge edge)
        {
            if (!edges.Contains(edge))
            {
                return;
            }

            int fromIndex = GetNodeIndexById(edge.from.ID);
            int toIndex = GetNodeIndexById(edge.to.ID);

            if (fromIndex == -1 || toIndex == -1)
            {
                return;
            }

            adjList[fromIndex, toIndex] = false;
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

        internal void AddEdges(List<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                Console.WriteLine($"{e.from.value}, {e.to.value}");
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
            if (nodes.Any(x => x.ID == n.ID))
            {
                return false;
            }

            nodes.Add(n);
            vars.ReplaceVariables(n);

            foreach (var na in n.possibleAnswers)
            {
                vars.ReplaceVariables(na);
            }

            if (!partOfList)
            {
                RedoAdjList();
            }

            return true;
        }

        private void RedoAdjList()
        {
            adjList = new bool[nodes.Count, nodes.Count];

            foreach (var edge in edges.ToList())
            {
                int fromIndex = GetNodeIndexById(edge.from.ID);
                int toIndex = GetNodeIndexById(edge.to.ID);

                if (fromIndex != -1 && toIndex != -1)
                {
                    adjList[fromIndex, toIndex] = true;
                }
            }
        }

        private void RemoveNode(QuestionNode n)
        {
            int index = GetNodeIndexById(n.ID);
            if (index == -1)
            {
                return;
            }

            nodes.RemoveAt(index);
            edges.RemoveAll(e => e.from.ID == n.ID || e.to.ID == n.ID);
            RedoAdjList();
        }

        internal List<QuestionNode> GetConnectedNodes(QuestionNode node)
        {
            int fromIndex = GetNodeIndexById(node.ID);

            Rage.Game.LogTrivial(
                $"[DialogueSystem][Graph:GetConnectedNodes] Checking '{node?.value}' | NodeIndex={fromIndex}"
            );

            if (fromIndex == -1)
            {
                return new List<QuestionNode>();
            }

            List<QuestionNode> result = new List<QuestionNode>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (adjList[fromIndex, i])
                {
                    result.Add(nodes[i]);
                }
            }

            Rage.Game.LogTrivial(
                $"[DialogueSystem][Graph:GetConnectedNodes] Found {result.Count} connected nodes for '{node.value}'"
            );

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
