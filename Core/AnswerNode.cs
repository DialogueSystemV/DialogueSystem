using DialogueSystem.Core.Logic;
using DialogueSystem.Engine;
using Rage;
using System.Collections.Generic;

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

            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:CTOR] Created AnswerNode | Value='{value}' | Probability={probability} | EndsConversation={endsConversation}"
            );
        }

        internal AnswerNode() : base()
        {
            questionsToAdd = new HashSet<QuestionNode>();
            questionsToRemove = new HashSet<QuestionNode>();

            Game.LogTrivial(
                "[DialogueSystem][AnswerNode:CTOR] Internal AnswerNode constructor called"
            );
        }

        public override void ProcessEdit(Graph graph)
        {
            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Processing edits for Answer '{value}'"
            );

            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Parent Question: {(parent != null ? parent.value : "NULL")}"
            );

            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:ProcessEdit] QuestionsToAdd Count: {questionsToAdd.Count}"
            );

            foreach (var qNode in questionsToAdd)
            {
                Game.LogTrivial(
                    $"[DialogueSystem][AnswerNode:ProcessEdit] Adding Question: '{qNode.value}'"
                );

                graph.AddEdge(new Edge(parent, qNode));
            }

            // Verify connections were created
            var connectedNodes = graph.GetConnectedNodes(parent);
            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Connected nodes after adding: {connectedNodes.Count}"
            );

            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:ProcessEdit] QuestionsToRemove Count: {questionsToRemove.Count}"
            );

            foreach (var qNode in questionsToRemove)
            {
                Game.LogTrivial(
                    $"[DialogueSystem][AnswerNode:ProcessEdit] Removing all links from Question '{qNode.value}'"
                );

                graph.RemoveAllLinksFromQuestion(qNode);
            }

            Game.LogTrivial(
                $"[DialogueSystem][AnswerNode:ProcessEdit] Completed ProcessEdit for Answer '{value}'"
            );
        }
    }
}