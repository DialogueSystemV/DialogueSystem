using System;
using System.Collections.Generic;
using Rage;

namespace DialogueSystem;

public class AnswerNode : Node
{
    public int Probability { get; set; }
    public Predicate<Ped>? Condition { get; set; }
    public List<QuestionNode> RemoveTheseQuestionsIfChosen{ get; set; }
    public List<QuestionNode> AddTheseQuestionsIfChosen { get; set; }

    public AnswerNode(string answer, int probability) : base(answer)
    {
        Condition = null;
        Probability = probability;
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, bool EndsConversation) : base(answer, EndsConversation)
    {
        Condition = null;
        Probability = probability;
        PerformActionIfChosen = null;
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> Condition, Ped Ped) : base(answer)
    {
        this.Condition = Condition;
        base.Ped = Ped;
        Probability = probability;
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> Condition, Ped Ped, bool EndsConversation) : base(answer, EndsConversation)
    {
        this.Condition = Condition;
        base.Ped = Ped;
        Probability = probability;
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Action<Ped> PerformActionIfChosen, Ped Ped) : base(answer,PerformActionIfChosen, Ped)
    {
        Condition = null;
        Probability = probability;
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Action<Ped> PerformActionIfChosen, Ped Ped, bool EndsConversation) : base(answer,PerformActionIfChosen, Ped,EndsConversation)
    {
        Probability = probability;
        Condition = null;
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> Condition, Ped Ped, Action<Ped> PerformActionIfChosen) : base(answer,PerformActionIfChosen, Ped)
    {
        this.Condition = Condition;
        Probability = probability;
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> Condition, Ped Ped, Action<Ped> PerformActionIfChosen, bool EndsConversation) : base(answer,PerformActionIfChosen, Ped,EndsConversation)
    {
        this.Condition = Condition;
        Probability = probability;
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
        RemoveTheseQuestionsIfChosen = new List<QuestionNode>();
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> Condition, Action<Ped> PerformActionIfChosen, Ped Ped, List<QuestionNode> removeTheseQuestionsIfChosen): base(answer, PerformActionIfChosen,Ped)
    {
        this.Condition = Condition;
        Probability = probability;
        this.PerformActionIfChosen = PerformActionIfChosen;
        AddTheseQuestionsIfChosen = new List<QuestionNode>();
        RemoveTheseQuestionsIfChosen = removeTheseQuestionsIfChosen;
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> Condition, Action<Ped> PerformActionIfChosen, Ped Ped, List<QuestionNode> removeTheseQuestionsIfChosen, List<QuestionNode> addTheseQuestionsIfChosen): base(answer, PerformActionIfChosen,Ped)
    {
        this.Condition = Condition;
        Probability = probability;
        this.PerformActionIfChosen = PerformActionIfChosen;
        AddTheseQuestionsIfChosen = addTheseQuestionsIfChosen;
        RemoveTheseQuestionsIfChosen = removeTheseQuestionsIfChosen;
    }
 
    public AnswerNode(string answer, int probability, Action<Ped> PerformActionIfChosen, Ped Ped, List<QuestionNode> addTheseQuestionsIfChosen, List<QuestionNode> removeQuestions ) : base(answer,PerformActionIfChosen, Ped)
    {
        Condition = null;
        Probability = probability;
        AddTheseQuestionsIfChosen = addTheseQuestionsIfChosen;
        RemoveTheseQuestionsIfChosen = removeQuestions;
    }
    
    public AnswerNode(string answer, int probability, Predicate<Ped> condition, Ped Ped, List<QuestionNode> addTheseQuestionsIfChosen, List<QuestionNode> removeQuestions ) : base(answer)
    {
        Condition = Condition;
        Probability = probability;
        base.Ped = Ped;
        AddTheseQuestionsIfChosen = addTheseQuestionsIfChosen;
        RemoveTheseQuestionsIfChosen = removeQuestions;
    }
    
    public AnswerNode(string answer, int probability, List<QuestionNode> addTheseQuestionsIfChosen, List<QuestionNode> removeQuestions ) : base(answer)
    {
        Condition = null;
        Probability = probability;
        AddTheseQuestionsIfChosen = addTheseQuestionsIfChosen;
        RemoveTheseQuestionsIfChosen = removeQuestions;
    }
}