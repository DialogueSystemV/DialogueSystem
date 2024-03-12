using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace csharpdsa
{
    public class QuestionNode : Node
    { 
        
        public List<AnswerNode> PossibleAnswers { get; set; }

        private AnswerNode chosenAnswer = null;

        internal Random rndm = new Random(DateTime.Now.Millisecond);
        
        public bool RemoveQuestionAfterAsked { get; set; }
        
        
        public QuestionNode(string Value, bool endsConversation, List<AnswerNode> possibleAnswers) : base(Value, endsConversation)
        {
            PossibleAnswers = possibleAnswers;
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
            EnabledAnswers = PossibleAnswers.FindAll(PA => PA.Condition == null || PA.Condition(null));
            if (EnabledAnswers.Count == 0)
            {
                throw new NoValidAnswerException($"No Valid Answer for Question Node: {Value}");
            }
            double maxProbability = EnabledAnswers.Max(node => node.Probability);
            List<AnswerNode> nodesWithHighestProbability = EnabledAnswers.Where(node => node.Probability == maxProbability).ToList();
            chosenAnswer = nodesWithHighestProbability[rndm.Next(nodesWithHighestProbability.Count)];
            return chosenAnswer;
        }

        public override void ProcessEdit(Graph graph)
        {
            if (RemoveQuestionAfterAsked)
            {
                int index = graph.nodes.IndexOf(this);
                for (int i = 0; i < graph.adjList.GetLength(1); i++)
                {
                    graph.adjList[index, i] = false; 
                }
            }
            base.ProcessEdit(graph);
        }
    }
}