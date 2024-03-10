using System;
using System.Collections.Generic;

namespace csharpdsa
{
    public class AnswerNode : Node
    {
        public int Probability { get; set; }
        public Predicate<Node>? Condition { get; set; }

        public AnswerNode(string answer, int probability) : base(answer)
        {
            Probability = probability;
            
        }
    }

}