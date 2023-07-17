using System;
using System.Collections.Generic;
using Rage;

namespace DialogueSystem;

public class AnswerNode : Node
{
    public Predicate<Ped>? Condition { get; set; }
    public List<QuestionNode>? RemoveThoseQuestionsIfChosen{ get; set; }
    public Ped? Ped { get; set; }

    public AnswerNode(string answer, Predicate<Ped> Condition, Ped Ped) : base(answer)
    {
        this.Condition = Condition;
        this.Ped = Ped;
        PerformActionIfChosen = null;
        RemoveThoseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, Predicate<Ped> Condition, Ped Ped, bool EndsConversation) : base(answer, EndsConversation)
    {
        this.Condition = Condition;
        this.Ped = Ped;
        PerformActionIfChosen = null;
        RemoveThoseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, Predicate<Ped> Condition, Ped Ped, Action<Ped> PerformActionIfChosen) : base(answer,PerformActionIfChosen)
    {
        this.Condition = Condition;
        this.Ped = Ped;
        this.PerformActionIfChosen = PerformActionIfChosen;
        RemoveThoseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, Predicate<Ped> Condition, Ped Ped, Action<Ped> PerformActionIfChosen, bool EndsConversation) : base(answer,PerformActionIfChosen, EndsConversation)
    {
        this.Condition = Condition;
        this.Ped = Ped;
        this.PerformActionIfChosen = PerformActionIfChosen;
        RemoveThoseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, Predicate<Ped> Condition, Ped Ped, Action<Ped> PerformActionIfChosen, List<QuestionNode> RemoveThoseQuestionsIfChosen): base(answer, PerformActionIfChosen)
    {
        this.Condition = Condition;
        this.Ped = Ped;
        this.PerformActionIfChosen = PerformActionIfChosen;
        this.RemoveThoseQuestionsIfChosen = RemoveThoseQuestionsIfChosen;
    }
    
}