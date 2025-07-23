using Newtonsoft.Json;

namespace DialogueSystem
{
    public struct Edge
    {
        public string Id { get; set; }
        public QuestionNode from;

        public QuestionNode to;
        
        public Edge(QuestionNode from, QuestionNode to)
        {
            Id = Guid.NewGuid().ToString();
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
