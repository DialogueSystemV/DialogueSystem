using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

// ReSharper disable All
namespace DialogueSystem;
public class DialogueGraph
{
    public List<Node> nodes;

    public DialogueGraph(List<Node> nodes)
    {
        this.nodes = nodes;
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
        foreach (Node currNode in nodes)
        {
            currNode.QuestionPool.RemoveAll(qandas => questionsToRemove.Contains(qandas.Question));
            foreach (var kvp in currNode.OutgoingEdges)
            {
                if (kvp.Key >= currNode.QuestionPool.Count)
                {
                    if (kvp.Value == currNode)
                    {
                        currNode.OutgoingEdges.Remove(kvp.Key);
                    }
                }
            }
        }
    }

    internal void AddQuestions(List<string> questionsToAdd)
    {
        //will have to check whether 
        throw new NotImplementedException();
    }
    
    internal void OnQuestionChosen(PossibleAnswer chosenAnswer, Conversation convo)
    {
        if (chosenAnswer.PerformActionIfChosen != null) chosenAnswer.PerformActionIfChosen(chosenAnswer.Ped);
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