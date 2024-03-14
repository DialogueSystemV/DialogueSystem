using System;

namespace DialogueSystem
{
    public struct Edge
    {
        public readonly QuestionNode from;

        public readonly QuestionNode to;
        
        public Edge(QuestionNode from, QuestionNode to)
        {
            this.from = from;
            this.to = to;
        }

        public static bool operator ==(Edge f, Edge g)
        {
            return f.Equals(g);
        }
        
        public static bool operator !=(Edge f, Edge g)
        {
              return !(f == g);
        }
        
        public bool Equals(Edge other)
        {
            return Node.Equals(this.to, other.to) && Node.Equals(this.from, other.from);
        }
        
        
        
    }
}