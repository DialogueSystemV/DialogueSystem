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
    
    public bool EndsConversation { get; set; }
    internal QuestionAndAnswers(){}
    public QuestionAndAnswers(string question, List<PossibleAnswer> possibleAnswers, string DefaultAnswer, bool EndsConversation)
    {
        Question = question;
        PossibleAnswers = possibleAnswers;
        this.DefaultAnswer = DefaultAnswer;
        this.EndsConversation = EndsConversation;
    }
    
    internal void FormatString(string replaceString, string valueToBeReplaced)
    {
        Question = Question.Replace(valueToBeReplaced, replaceString);
        foreach (var PA in PossibleAnswers)
        {
            PA.Answer = PA.Answer.Replace(valueToBeReplaced, replaceString);
        }
    }

    internal PossibleAnswer ChooseAnswer()
    {
        foreach (PossibleAnswer PA in PossibleAnswers)
        {
            if (PA.Condition != null && PA.Condition(PA.Ped))
            {
                return PA;
            }
        }
        return new PossibleAnswer(DefaultAnswer, false);
    }
    
}