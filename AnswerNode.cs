using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    public class AnswerNode : Node
    {
        public int Probability { get; set; }
        public Predicate<Node>? Condition { get; set; }
        public bool endsConversation { get; set; }
        public Action? action { get; set; }
        

        public AnswerNode(string answer, int probability, bool endsConversation, Action action) : base(answer)
        {
            Probability = probability;
            this.endsConversation = endsConversation;
            this.action = action;

        }
        public AnswerNode(string answer, int probability) : base(answer)
        {
            Probability = probability;
            this.endsConversation = endsConversation;
            endsConversation = false;
        }
    }

}