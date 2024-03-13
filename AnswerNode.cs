using System;
using System.Collections.Generic;

namespace csharpdsa
{
    public class AnswerNode : Node
    {
        public int Probability { get; set; }
        public Predicate<Node>? Condition { get; set; }
        public bool endsConversation { get; set; }
        

        public AnswerNode(string answer, int probability, bool endsConversation) : base(answer)
        {
            Probability = probability;
            this.endsConversation = endsConversation;
            
        }
    }

}