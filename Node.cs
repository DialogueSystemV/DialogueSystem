namespace DialogueSystem
{
    public abstract class Node
    {
        private Guid ID;
        public string value { get; set; }
        public HashSet<QuestionNode> questionsToRemove { get; set; }
        public HashSet<QuestionNode> questionsToAdd { get; set; }

        public Node(string Value)
        {
            this.value = Value;
            ID = new Guid();
            questionsToAdd = new HashSet<QuestionNode>();
            questionsToRemove = new HashSet<QuestionNode>();
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
                    var answerNode = (AnswerNode)this;
                    graph.AddEdge(new Edge(answerNode.parent, qNode));
                }
                graph.AddEdge(new Edge((QuestionNode)this, qNode));
            }
            
            foreach(var qNode in questionsToRemove)
            {
                if (this is AnswerNode)
                {
                    var answerNode = (AnswerNode)this;
                    graph.RemoveEdge(new Edge(answerNode.parent, qNode));
                }
                graph.RemoveEdge(new Edge((QuestionNode)this, qNode));
            }
        }
    }
}