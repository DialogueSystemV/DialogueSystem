using DialogueSystem.Core.Logic;
using DialogueSystem.Engine;
using Rage;
using System.Collections.Generic;
using DialogueSystem.Logging;
#nullable enable
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

        public AnswerNode(string answer, int probability, QuestionNode parent, bool endsConversation = false) :
            base(answer)
        {
            this.probability = probability;
            this.endsConversation = endsConversation;
            this.parent = parent;
            enabled = true;
        }

        internal AnswerNode() : base()
        {
            questionsToAdd = new HashSet<QuestionNode>();
            questionsToRemove = new HashSet<QuestionNode>();
        }

        public override void ProcessEdit(Graph graph)
        {
            Logger.logger.Log(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Processing edits for Answer '{value}'"
            );

            Logger.logger.Log(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Parent Question: {(parent != null ? parent.value : "NULL")}"
            );

            Logger.logger.Log(
                $"[DialogueSystem][AnswerNode:ProcessEdit] QuestionsToAdd Count: {questionsToAdd.Count}"
            );

            foreach (var qNode in questionsToAdd)
            {
                Logger.logger.Log(
                    $"[DialogueSystem][AnswerNode:ProcessEdit] Adding Question: '{qNode.value}'"
                );

                graph.AddEdge(new Edge(parent, qNode));
            }

            // Verify connections were created
            var connectedNodes = graph.GetConnectedNodes(parent);
            Logger.logger.Log(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Connected nodes after adding: {connectedNodes.Count}"
            );

            Logger.logger.Log(
                $"[DialogueSystem][AnswerNode:ProcessEdit] QuestionsToRemove Count: {questionsToRemove.Count}"
            );

            foreach (var qNode in questionsToRemove)
            {
                Logger.logger.Log(
                    $"[DialogueSystem][AnswerNode:ProcessEdit] Removing all links from Question '{qNode.value}'"
                );

                graph.RemoveAllLinksFromQuestion(qNode);
            }

            Logger.logger.Log(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Completed ProcessEdit for Answer '{value}'"
            );
        }
    }
}
