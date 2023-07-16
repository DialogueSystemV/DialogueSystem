using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

// ReSharper disable All
namespace DialogueSystem;
public class DialogueGraph
{
    public List<Node> nodes;
    public Ped Ped;

    public DialogueGraph(Ped ped)
    {
        nodes = new List<Node>();
        Ped = ped;
    }

    public void AddNode(string identifier, List<QuestionAndAnswers> questionPool)
    {
        if (FindNode(identifier) == null)
        {
            Node newNode = new Node(identifier,questionPool);
            nodes.Add(newNode);
        }
    }

    public void LinkQuestionToNode(string source, int index, Node destination)
    {
        Node sourceNode = FindNode(source);

        if (sourceNode == null)
        {
            throw new ArgumentException("Source node not found.");
        }

        if (index < 0 || index >= sourceNode.QuestionPool.Count)
        {
            throw new ArgumentException("String index is out of range.");
        }
        sourceNode.OutgoingEdges[index] = destination;
    }

    internal Node FindNode(string identifier)
    {
        return nodes.Find(node => node.Identifier == identifier);
    }

    internal bool IsQuestionLinked(string NodeIdentifier, int index)
    {
        return FindNode(NodeIdentifier).OutgoingEdges.ContainsKey(index);
    }
    
    internal void GetLinkedNode(string identifier, int index, Conversation convo)
    {
        Node node = FindNode(identifier);
        if (node == null)
        {
            return;
        }

        if (node.OutgoingEdges.ContainsKey(index))
        {
            convo.currNode = node.OutgoingEdges[index];
        }
    }
    
    internal void GetLinkedNode(string identifier, int index, ConversationWithMenu convo)
    {
        Node node = FindNode(identifier);
        if (node == null)
        {
            return;
        }

        if (node.OutgoingEdges.ContainsKey(index))
        {
            convo.currNode = node.OutgoingEdges[index];
            convo.ConversationMenu.Clear();
            convo.AddQuestionsToMenu();
            GameFiber.StartNew(() => CheckForConversationEnders());
        }
    }

    internal void RemoveLinks(string identifier, int index)
    {
        Node node = FindNode(identifier);
        node.OutgoingEdges.Remove(index);
    }
    
    
    internal void RemoveQuestions(List<string> questionsToRemove)
    {
        foreach (Node n in nodes)
        {
            for(int i = 0; i < n.QuestionPool.Count; i++)
            {
                var qandas = n.QuestionPool[i];
                foreach (string q in questionsToRemove)
                {
                    if (qandas.Question.Equals(q))
                    {
                        if(IsQuestionLinked(n.Identifier,i))
                        {
                            RemoveLinks(n.Identifier, i);   
                        }
                        n.QuestionPool.Remove(qandas);
                    }
                    break;
                }
            }
        }
    }

    internal void AddQuestions(List<string> questionsToAdd)
    {
        throw new NotImplementedException();
    }
    
    internal void OnQuestionChosen(PossibleAnswer chosenAnswer, Conversation convo)
    {
        if (chosenAnswer.PerformActionIfChosen != null) chosenAnswer.PerformActionIfChosen(Ped);
        if(chosenAnswer.RemoveThoseQuestionsIfChosen.Count != 0) RemoveQuestions(chosenAnswer.RemoveThoseQuestionsIfChosen);
        // add AddQuestionsIfChosen here and to if statement below
        if (convo.EndNaturally && chosenAnswer.RemoveThoseQuestionsIfChosen.Count != 0)
        {
            CheckForConversationEnders();
        }
    }

    internal bool CheckForConversationEnders()
    {
        if (!nodes.Any(q =>
                q.QuestionPool.Any(a => a.EndsConversation || a.PossibleAnswers.Any(pa => pa.EndsConversation))))
        {
            return false;
        }
        return true;
    }
}