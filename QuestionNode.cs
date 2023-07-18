using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

namespace DialogueSystem;

public class QuestionNode : Node
{
    public QuestionEffect Effect { get; set; }
    public List<AnswerNode> PossibleAnswers { get; set; }

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
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, Ped ped, QuestionEffect Effect) : base(Value, PerformActionIfChosen, ped)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, Ped ped, QuestionEffect Effect, bool EndsConversation) : base(Value, PerformActionIfChosen, ped,EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
    }
    
    internal AnswerNode ChooseAnswer()
    {
        List<AnswerNode> AnswersThatMeetCondition = new List<AnswerNode>();
        foreach (AnswerNode PA in PossibleAnswers)
        {
            if (PA.Condition != null && PA.Condition(PA.Ped))
            {
                AnswersThatMeetCondition.Add(PA);
            }
        }
        return AnswersThatMeetCondition.Count == 0
            ? PossibleAnswers.OrderByDescending(item => item.Probability).First()
            : AnswersThatMeetCondition.OrderByDescending(item => item.Probability).First();
    }
    
    
}