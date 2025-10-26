using DialogueSystem.Core.Logic;
using DialogueSystem.Engine;

namespace DialogueSystem.Core
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
        public ExternalCondition? condition { get; set; }

        /// <summary>
        /// Whether the answer ends the conversation abruptly
        /// </summary>
        public bool endsConversation { get; set; }

        /// <summary>
        /// Method that gets run when the answer gets chosen
        /// </summary>
        public ExternalAction? action { get; set; }

        public bool enabled { get; set; }

        internal QuestionNode parent;


        public AnswerNode(string answer, int probability, bool endsConversation = false) :
            base(answer)
        {
            this.probability = probability;
            this.endsConversation = endsConversation;
            enabled = true;
        }

        internal AnswerNode() : base()
        {
            questionsToAdd = new HashSet<QuestionNode>();
            questionsToRemove = new HashSet<QuestionNode>();
        }

        public override void ProcessEdit(Graph graph)
        {
            foreach (var qNode in questionsToAdd)
            {
                graph.AddEdge(new Edge(parent, qNode));
            }

            foreach (var qNode in questionsToRemove)
            {
                graph.RemoveAllLinksFromQuestion(qNode);
            }
            base.ProcessEdit(graph);
        }
    }
}
