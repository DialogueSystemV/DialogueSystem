using System;
using System.Collections.Generic;

namespace csharpdsa
{
    public abstract class Node
    {
        private Guid ID;
        public string value { get; set; }
        public bool endsConversation { get; set; }
        public HashSet<Edge> edgesToRemove { get; set; }
        public HashSet<Edge> edgesToAdd { get; set; }

        public Node(string Value, bool endsConversation)
        {
            this.value = Value;
            ID = new Guid();
            this.endsConversation = endsConversation;
            edgesToAdd = new HashSet<Edge>();
            edgesToRemove = new HashSet<Edge>();
        }
        
        public Node(string Value)
        {
            this.value = Value;
            ID = new Guid();
            endsConversation = false;
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