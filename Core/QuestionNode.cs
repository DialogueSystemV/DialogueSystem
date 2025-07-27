using DialogueSystem.UI;
using DialogueSystem.Util;
using Rage;

namespace DialogueSystem.Core
{
    public class QuestionNode : Node
    {
        /// <summary>
        /// List of possible answers to the question
        /// </summary>
        public List<AnswerNode> possibleAnswers { get; set; }

        private AnswerNode chosenAnswer = null;

        private Random rndm = new Random(DateTime.Now.Millisecond);

        public bool startsConversation = false;
        private WeightedList<AnswerNode> _weightedAnswers;

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
        public QuestionNode(string value, bool removeQuestionAfterAsked = false,
            params AnswerNode[] possibleAnswers) : base(value)
        {
            this.removeQuestionAfterAsked = removeQuestionAfterAsked;
            foreach (var answer in possibleAnswers)
            {
                answer.parent = this;
                this.possibleAnswers.Add(answer);
            }
        }

        public QuestionNode() : base()
        {
        }

        /// <summary>
        /// Chooses the answer to the question based on the probability of the answer
        /// and conditions provided
        /// </summary>
        /// <param name="graph">Graph which the question is associated with</param>
        /// <returns>The AnswerNode chosen</returns>
        public AnswerNode ChooseQuestion(Graph graph, Conversation convo)
        {
            AnswerNode node = ChooseAnswer(graph, convo);
            if (chosenAnswer == null)
            {
                ProcessEdit(graph);
                node.ProcessEdit(graph);
            }

            Game.LogTrivial($"Setting {node.value} to the answer of {value}");
            chosenAnswer = node;
            return node;
        }


        private AnswerNode ChooseAnswer(Graph graph, Conversation convo)
        {
            if (chosenAnswer != null) return chosenAnswer;
            List<AnswerNode> EnabledAnswers = new List<AnswerNode>();
            EnabledAnswers =
                possibleAnswers.FindAll(PA => PA.enabled && IsAnswerConditionMet(PA, convo));
            EnabledAnswers = possibleAnswers;
            if (EnabledAnswers.Count == 0)
            {
                throw new NoValidAnswerException($"No Valid Answer for Question Node: {value}");
            }

            _weightedAnswers = new WeightedList<AnswerNode>(rndm);
            foreach (var aNode in EnabledAnswers)
            {
                _weightedAnswers.Add(aNode, aNode.probability);
            }

            chosenAnswer = _weightedAnswers.Next();
            Game.LogTrivial($"Adding all answers of {chosenAnswer.value} to weighted list");
            return chosenAnswer;
        }

        private bool IsAnswerConditionMet(AnswerNode answerNode, Conversation convo)
        {
            // If there's no condition, it's always met
            if (answerNode.condition == null)
            {
                return true;
            }

            // Try to get the cached result from the condition pool
            if (convo.conditionPool.TryGetValue(answerNode.ID, out bool cachedConditionResult))
            {
                return cachedConditionResult;
            }

            // If not in cache (should ideally not happen if conditions are pre-computed),
            // invoke the condition directly as a fallback.
            bool val = answerNode.condition.Invoke();
            convo.conditionPool.Add(answerNode.ID, val);
            return val;
        }

        /// <summary>
        /// Processes all links to be added and removed from the question
        /// </summary>
        /// <param name="graph">Graph which the question is associated with</param>
        public override void ProcessEdit(Graph graph)
        {
            if (removeQuestionAfterAsked)
            {
                graph.RemoveAllLinksFromQuestion(this);
            }
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
