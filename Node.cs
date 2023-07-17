using System.Collections.Generic;

namespace DialogueSystem;

public class Node
{
    public string Identifier { get; set; }
    public List<QuestionAndAnswers> QuestionPool { get; set; }

    internal Dictionary<int, Node> OutgoingEdges { get; set; }

    public Node(string identifier, List<QuestionAndAnswers> questionPool)
    {
        Identifier = identifier;
        QuestionPool = questionPool;
        OutgoingEdges = new Dictionary<int, Node>();
    }
    
    public Node(string identifier, List<QuestionAndAnswers> questionPool, Dictionary<int, Node> OutgoingEdges)
    {
        Identifier = identifier;
        QuestionPool = questionPool;
        this.OutgoingEdges = OutgoingEdges;
    }
    
    internal string DisplayQuestions()
    {
        string displaystr = "";
        for(int i = 0; i< QuestionPool.Count; i++)
        {
            displaystr += $"[{i}]: {QuestionPool[i].Question}\n";
        }
        return displaystr;
    }
    
    internal bool IsValidIndex(int i) => i < QuestionPool.Count;
}