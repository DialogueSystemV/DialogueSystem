namespace DialogueSystem;

public class QuestionAndAnswer
{
    public string question { get; set; }
    public QuestionEffect effect { get; set; }
        
    public string answer { get; set; }
    
    public QuestionAndAnswer(string question, QuestionEffect effect, string answer)
    {
        this.question = question;
        this.effect = effect;
        this.answer = answer;
    }
}