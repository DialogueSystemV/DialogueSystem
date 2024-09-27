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
        
        public override bool Equals(Object other)
        {
            if(!(other is Edge))
                return false;
            Edge edgeOther = (Edge)other;
            return Node.Equals(this.to, edgeOther.to) && Node.Equals(this.from, edgeOther.from);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}