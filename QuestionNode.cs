using System;
using System.Collections.Generic;
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
    
    internal AnswerNode ChooseAnswer()
    {
        foreach (AnswerNode PA in PossibleAnswers)
        {
            if (PA.Condition != null && PA.Condition(PA.Ped))
            {
                return PA;
            }
        }

        return PossibleAnswers[0];
    }
    
    
}