using System;
using System.Collections.Generic;
using DialogueSystem;

namespace DialogueSystem;

public class QuestionPool
{
    public List<QuestionAndAnswer> Pool { get; set; }
    public QuestionPool(List<QuestionAndAnswer> pool, params string[] stringsToBeFormattedIn)
    {
        Pool = pool;
    }

    public string DisplayQuestions()
    {
        string displaystr = "";
        for(int i = 0; i< Pool.Count; i++)
        {
            displaystr += $"[{i}]: {Pool[i].Question}\n";
        }
        return displaystr;
    }

    private void FormatDialogue(string[] toBeFormattedIn)
    {
        for(int i = 0; i < toBeFormattedIn.Length; i++)
        {
            
        }
    }
    
    public string GetAnswer(int index)
    {
        return IsValidIndex(index) ? Pool[index].Answer : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {Pool.Count - 1}");
    }
            
    public QuestionEffect GetEffect(int index)
    {
        return IsValidIndex(index) ? Pool[index].Effect : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {Pool.Count - 1}");
    }

    internal void RemoveQuestionAnswer(int index)
    {
        Pool.RemoveAt(index);
    }
    public bool IsValidIndex(int index) => index < Pool.Count;
}