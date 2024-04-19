using Rage;

namespace DialogueSystem
{
    public class AnswerNode : Node
    {
        /// <summary>
        /// Probability the answer gets chosen
        /// </summary>
        public int probability { get; set; }
        
        /// <summary>
        /// Condition that has to be met for the answer to be chosen
        /// </summary>
        public Predicate<Ped>? condition { get; set; }
        /// <summary>
        /// Whether the answer ends the conversation abruptly
        /// </summary>
        public bool endsConversation { get; set; }
        /// <summary>
        /// Method that gets run when the answer gets chosen
        /// </summary>
        public Action? action { get; set; }

        internal QuestionNode parent;
        

        public AnswerNode(string answer, int probability, bool endsConversation = false, Action action = null) : base(answer)
        {
            this.probability = probability;
            this.endsConversation = endsConversation;
            this.action = action;

        }
    }

}