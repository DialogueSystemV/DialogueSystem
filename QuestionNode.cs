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

    public bool RemoveAfterAsked { get; set; }

    internal List<int> defaultProbs = new();
    internal int defaultProbMax = -100;

    internal Random rndm = new(DateTime.Now.Millisecond);
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, QuestionEffect Effect, bool removeAfterAsked) : base(Value)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, QuestionEffect Effect, bool EndsConversation, bool removeAfterAsked) : base(Value, EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, QuestionEffect Effect, bool removeAfterAsked) : base(Value, PerformActionIfChosen)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, QuestionEffect Effect, bool EndsConversation, bool removeAfterAsked) : base(Value, PerformActionIfChosen, EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    internal AnswerNode ChooseAnswer(Conversation convo)
    {
        List<AnswerNode> AnswersThatMeetCondition = new List<AnswerNode>();
        int max = -100;
        foreach (AnswerNode PA in PossibleAnswers)
        {
            if (PA.Condition != null && PA.Condition(convo.Ped))
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