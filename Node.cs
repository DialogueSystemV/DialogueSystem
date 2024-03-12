using System;
using System.Collections.Generic;

namespace csharpdsa
{
    public abstract class Node
    {
        private Guid ID;
        public string Value { get; set; }
        public bool EndsConversation { get; set; }
        public HashSet<Edge> edgesToRemove { get; set; }
        public HashSet<Edge> edgesToAdd { get; set; }

        public Node(string Value, bool endsConversation)
        {
            this.Value = Value;
            ID = new Guid();
            EndsConversation = endsConversation;
            edgesToAdd = new HashSet<Edge>();
            edgesToRemove = new HashSet<Edge>();
        }
        
        public Node(string Value)
        {
            this.Value = Value;
            ID = new Guid();
            EndsConversation = false;
            edgesToAdd = new HashSet<Edge>();
            edgesToRemove = new HashSet<Edge>();
        }

        static bool Equals(Node n1, Node n2)
        {
            return n1.ID.Equals(n2.ID);
        }

        public virtual void ProcessEdit(Graph graph)
        {
            graph.RemoveEdges(edgesToRemove);
            graph.AddEdges(edgesToAdd);
        }
    }
}