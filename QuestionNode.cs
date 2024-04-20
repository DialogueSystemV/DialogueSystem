namespace DialogueSystem
{
    public class QuestionNode : Node
    { 
        /// <summary>
        /// List of possible answers to the question
        /// </summary>
        public List<AnswerNode> possibleAnswers { get; set; }

        private AnswerNode chosenAnswer = null;

        private Random rndm = new Random(DateTime.Now.Millisecond);
        
        /// <summary>
        /// Whether the question should be removed from the pool after being asked
        /// </summary>
        public bool removeQuestionAfterAsked { get; set; }
        
        /// <summary>
        /// Instantiates a new node with the given question and possible answers
        /// </summary>
        /// <param name="value">Question wanting to be asked</param>
        /// <param name="removeQuestionAfterAsked">Whether the question should be removed from the pool after being asked</param>
        /// <param name="possibleAnswers">array of all answers as AnswerNode object</param>
        public QuestionNode(string value, bool removeQuestionAfterAsked = false, params AnswerNode[] possibleAnswers) : base(value)
        {
            this.removeQuestionAfterAsked = removeQuestionAfterAsked;
            foreach (var answer in possibleAnswers)
            {
                answer.parent = this;
                this.possibleAnswers.Add(answer);
            }
        }
        
        /// <summary>
        /// Chooses the answer to the question based on the probability of the answer
        /// and conditions provided
        /// </summary>
        /// <param name="graph">Graph which the question is associated with</param>
        /// <returns>The AnswerNode chosen</returns>
        public AnswerNode ChooseQuestion(Graph graph)
        {
            AnswerNode node = ChooseAnswer();
            if (chosenAnswer == null)
            {
                ProcessEdit(graph);
                node.ProcessEdit(graph);
            }
            return node;
        }
        
        
        private AnswerNode ChooseAnswer()
        {
            if (chosenAnswer != null) return chosenAnswer;
            List<AnswerNode> EnabledAnswers = new List<AnswerNode>();
            EnabledAnswers = possibleAnswers.FindAll(PA => PA.enabled && (PA.condition == null || PA.condition(null)));
            if (EnabledAnswers.Count == 0)
            {
                throw new NoValidAnswerException($"No Valid Answer for Question Node: {value}");
            }
            double maxProbability = EnabledAnswers.Max(node => node.probability);
            List<AnswerNode> nodesWithHighestProbability = EnabledAnswers.Where(node => node.probability == maxProbability).ToList();
            chosenAnswer = nodesWithHighestProbability[rndm.Next(nodesWithHighestProbability.Count)];
            return chosenAnswer;
        }

        /// <summary>
        /// Processes all links to be added and removed from the question
        /// </summary>
        /// <param name="graph">Graph which the question is associated with</param>
        public override void ProcessEdit(Graph graph)
        {
            if (removeQuestionAfterAsked) graph.RemoveAllLinksFromQuestion(this);
            base.ProcessEdit(graph);
        }

        /// <summary>
        /// Returns whether the question has been answered
        /// </summary>
        /// <returns>boolean</returns>
        public bool HasBeenAnswered() => chosenAnswer != null;

        internal void ResetChosenAnswer()
        {
            chosenAnswer = null;
        }
     }
}