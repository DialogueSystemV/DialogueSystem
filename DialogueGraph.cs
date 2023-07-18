using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

// ReSharper disable All
namespace DialogueSystem;
public class DialogueGraph
{
    public List<QuestionNode> nodes;

    public DialogueGraph(List<QuestionNode> nodes)
    {
        this.nodes = nodes;
    }
    
    internal void RemoveQuestions(List<QuestionNode> questionsToRemove)
    {
        nodes.RemoveAll(node => questionsToRemove.Contains(node));
    }

    internal void AddQuestions(List<QuestionNode> questionsToAdd)
    {
        nodes.AddRange(questionsToAdd);
    }
    
    internal void OnQuestionChosen(AnswerNode chosenAnswerNode, Conversation convo)
    {
        if(chosenAnswerNode.PerformActionIfChosen != null) chosenAnswerNode.PerformActionIfChosen(chosenAnswerNode.Ped);
        if(chosenAnswerNode.RemoveTheseQuestionsIfChosen.Count != 0) RemoveQuestions(chosenAnswerNode.RemoveTheseQuestionsIfChosen);
        if(chosenAnswerNode.AddTheseQuestionsIfChosen.Count != 0) AddQuestions(chosenAnswerNode.AddTheseQuestionsIfChosen);
        if(convo.Graph.nodes.Count == 0) convo.DisplayDialogueEnd();
    }
    
    internal bool IsValidIndex(int index) => index < nodes.Count;

    
    internal string DisplayQuestions()
    {
        string displaystr = "";
        for(int i = 0; i< nodes.Count; i++)
        {
            displaystr += $"[{i}]: {nodes[i].Value}\n";
        }
        return displaystr;
    }
    public void ClearGraph() => nodes.Clear();


}