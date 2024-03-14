using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DialogueSystem
{
    public class QuestionNode : Node
    { 
        
        public List<AnswerNode> possibleAnswers { get; set; }

        private AnswerNode chosenAnswer = null;

        internal Random rndm = new Random(DateTime.Now.Millisecond);
        
        public bool removeQuestionAfterAsked { get; set; }
        
        
        public QuestionNode(string value, bool remQAA, params AnswerNode[] possibleAnswers) : base(value)
        {
            removeQuestionAfterAsked = remQAA;
            this.possibleAnswers = possibleAnswers.ToList();
        }
        public QuestionNode(string value, params AnswerNode[] possibleAnswers) : base(value)
        {
            removeQuestionAfterAsked = false;
            this.possibleAnswers = possibleAnswers.ToList();
        }

        public AnswerNode ChooseQuestion(Graph graph)
        {
            ProcessEdit(graph);
            AnswerNode node = ChooseAnswer();
            node.ProcessEdit(graph);
            return node;
        }
        
        private AnswerNode ChooseAnswer()
        {
            if (chosenAnswer != null) return chosenAnswer;
            List<AnswerNode> EnabledAnswers = new List<AnswerNode>();
            EnabledAnswers = possibleAnswers.FindAll(PA => PA.Condition == null || PA.Condition(null));
            if (EnabledAnswers.Count == 0)
            {
                throw new NoValidAnswerException($"No Valid Answer for Question Node: {value}");
            }
            double maxProbability = EnabledAnswers.Max(node => node.Probability);
            List<AnswerNode> nodesWithHighestProbability = EnabledAnswers.Where(node => node.Probability == maxProbability).ToList();
            chosenAnswer = nodesWithHighestProbability[rndm.Next(nodesWithHighestProbability.Count)];
            return chosenAnswer;
        }

        public override void ProcessEdit(Graph graph)
        {
            if (removeQuestionAfterAsked) graph.RemoveQuestion(this);
            base.ProcessEdit(graph);
        }

        public bool HasBeenAnswered() => chosenAnswer != null;

        internal void ResetChosenAnswer()
        {
            chosenAnswer = null;
        }
     }
}