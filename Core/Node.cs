namespace DialogueSystem.Core
{
    public abstract class Node
    {
        public string ID { get; internal set; }
        public string value { get; set; }
        public HashSet<QuestionNode> questionsToRemove { get; set; }
        public HashSet<QuestionNode> questionsToAdd { get; set; }
        
        public HashSet<AnswerNode> answersToRemove { get; set; }
        public HashSet<AnswerNode> answersToAdd { get; set; }

        public Node(string Value)
        {
            this.value = Value;
            ID = Guid.NewGuid().ToString();
            questionsToAdd = new HashSet<QuestionNode>();
            questionsToRemove = new HashSet<QuestionNode>();
        }

        protected Node()
        {
            
        }

        static bool Equals(Node n1, Node n2)
        {
            return n1.ID.Equals(n2.ID);
        }

        public virtual void ProcessEdit(Graph graph)
        {
            foreach (var qNode in questionsToAdd)
            {
                if (this is AnswerNode)
                {
                    var node = this as AnswerNode;
                    graph.AddEdge(new Edge(node.parent, qNode));
                }
            }
            
            foreach(var qNode in questionsToRemove)
            {
                graph.RemoveAllLinksFromQuestion(qNode);
            }

            foreach (var aNode in answersToAdd)
            {
                aNode.enabled = true;
            }
            
            foreach (var aNode in answersToRemove)
            {
                aNode.enabled = false;
            }
        }
    }
}
