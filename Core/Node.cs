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
        }
    }
}
