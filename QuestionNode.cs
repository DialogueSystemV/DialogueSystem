using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

namespace DialogueSystem;

public class QuestionNode : Node
{
    public QuestionEffect Effect { get; set; }

    private List<AnswerNode> _possibleAnswers;
    public List<AnswerNode> PossibleAnswers
    {
        get { return _possibleAnswers; }
        set
        {
            _possibleAnswers = value;
            SortProbs();
        }
    }

    internal List<int> defaultProbs = new();
    internal int defaultProbMax = -100;

    internal Random rndm = new(DateTime.Now.Millisecond);
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, QuestionEffect Effect) : base(Value)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
    }
    
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, QuestionEffect Effect, bool EndsConversation) : base(Value, EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, QuestionEffect Effect) : base(Value, PerformActionIfChosen)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, QuestionEffect Effect, bool EndsConversation) : base(Value, PerformActionIfChosen, EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
    }
    
    internal AnswerNode ChooseAnswer(DialogueGraph graph)
    {
        List<AnswerNode> AnswersThatMeetCondition = new List<AnswerNode>();
        int max = -100;
        foreach (AnswerNode PA in PossibleAnswers)
        {
            if (PA.Condition != null && PA.Condition(graph.Ped))
            {
                AnswersThatMeetCondition.Add(PA);
                if (PA.Probability > max)
                {
                    max = PA.Probability;
                }
            }
        }
        return AnswersThatMeetCondition.Count == 0 
            ? PossibleAnswers[rndm.Next(PossibleAnswers.RemoveAll(item => item.Probability < defaultProbMax))]
            : AnswersThatMeetCondition[rndm.Next(AnswersThatMeetCondition.RemoveAll(item => item.Probability < max))];
    }

    internal void SortProbs()
    {
        PossibleAnswers.OrderByDescending(item => item.Probability);
        foreach (AnswerNode PA in PossibleAnswers)
        {
            defaultProbs.Add(PA.Probability);
        }
        defaultProbMax = defaultProbs.Max();
    }

}