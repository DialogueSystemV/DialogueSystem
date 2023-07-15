using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogueSystem;

public class QuestionAndAnswers 
{
    public string Question { get; set; }
    
    public QuestionEffect Effect { get; set; }
    /// <summary>
    /// List of possible answers for the question
    /// </summary>
    public List<PossibleAnswer> PossibleAnswers { get; set; }
    
    public string DefaultAnswer { get; set; }
    
    public QuestionAndAnswers(string question, List<PossibleAnswer> possibleAnswers, string DefaultAnswer)
    {
        this.Question = question;
        PossibleAnswers = possibleAnswers;
        this.DefaultAnswer = DefaultAnswer;
    }
    
    internal void FormatString(string replaceString, string valueToBeReplaced)
    {
        Question = Question.Replace(valueToBeReplaced, replaceString);
        foreach (var PA in PossibleAnswers)
        {
            PA.Answer = PA.Answer.Replace(valueToBeReplaced, replaceString);
        }
    }

    internal string ChooseAnswer()
    {
        return PossibleAnswers.FirstOrDefault(c => c.Condition != null ? c.Condition(c.Ped) : true).Answer ?? DefaultAnswer;
    }
}