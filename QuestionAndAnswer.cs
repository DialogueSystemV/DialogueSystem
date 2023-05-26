namespace DialogueSystem;

public class QuestionAndAnswer
{
    /// <summary>
    /// Question that the player will ask
    /// </summary>
    public string question { get; set; }
    /// <summary>
    /// The effect that the question will have on the suspect(Positive,Neutral,Negative)
    /// </summary>
    public QuestionEffect effect { get; set; }
    
    /// <summary>
    /// Answer that suspect will give
    /// </summary>
    public string answer { get; set; }
    
    public QuestionAndAnswer(string question, QuestionEffect effect, string answer)
    {
        this.question = question;
        this.effect = effect;
        this.answer = answer;
    }
}