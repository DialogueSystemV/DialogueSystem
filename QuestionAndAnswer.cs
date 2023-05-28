using System;

namespace DialogueSystem;

public class QuestionAndAnswer
{
    /// <summary>
    /// Question that the player will ask
    /// </summary>
    public string Question { get; set; }
    /// <summary>
    /// The effect that the question will have on the suspect(Positive,Neutral,Negative)
    /// </summary>
    public QuestionEffect Effect { get; set; }
    
    /// <summary>
    /// Answer that suspect will give
    /// </summary>
    public string Answer { get; set; }

    public QuestionAndAnswer(string question, QuestionEffect effect, string answer)
    {
        Question = question;
        Effect = effect;
        Answer = answer;
    }
    public QuestionAndAnswer(string question, QuestionEffect effect, string answer, string[]? questionReplacements,string[]? answerReplacements)
    {
        Question = question;
        Answer = answer;
        Effect = effect;
        StringFormat(questionReplacements,answerReplacements);
    }
    private void StringFormat(string[]? questionReplacements,string[]? answerReplacements)
    {
        if (questionReplacements != null) { string.Format(Question, questionReplacements); }
        if(answerReplacements != null){ string.Format(Answer, answerReplacements); }
    }
}