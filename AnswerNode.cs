namespace DialogueSystem
{
    public class AnswerNode : Node
    {
        /// <summary>
        /// Probability the answer gets chosen
        /// </summary>
        public int Probability { get; set; }
        
        /// <summary>
        /// Condition that has to be met for the answer to be chosen
        /// </summary>
        public Predicate<Node>? Condition { get; set; }
        /// <summary>
        /// Whether the answer ends the conversation abruptly
        /// </summary>
        public bool endsConversation { get; set; }
        /// <summary>
        /// Method that gets run when the answer gets chosen
        /// </summary>
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