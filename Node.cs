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
}