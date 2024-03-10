using System;

namespace csharpdsa
{
    public abstract class Node
    {
        private Guid ID;
        public string Value { get; set; }
        public bool EndsConversation { get; set; }

        public Node(string Value, bool endsConversation)
        {
            this.Value = Value;
            ID = new Guid();
            EndsConversation = endsConversation;
        }
        
        public Node(string Value)
        {
            this.Value = Value;
            ID = new Guid();
            EndsConversation = false;
        }

        static bool Equals(Node n1, Node n2)
        {
            return n1.ID.Equals(n2.ID);
        }
    }
}