using System;
using System.Collections.Generic;
using DialogueSystem;

namespace DialogueSystem;

public class QuestionPool
{
    public List<QuestionAndAnswer> pool { get; set; }
    public QuestionPool(List<QuestionAndAnswer> pool)
    {
        this.pool = pool;
    }

    public string DisplayQuestions()
    {
        string displaystr = "";
        for(int i = 0; i< pool.Count; i++)
        {
            displaystr += $"[{i}]: {pool[i].question}\n";
        }
        return displaystr;
    }

    public string GetAnswer(int index)
    {
        return IsValidIndex(index) ? pool[index].answer : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {pool.Count - 1}");
    }
            
    public QuestionEffect GetEffect(int index)
    {
        return IsValidIndex(index) ? pool[index].effect : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {pool.Count - 1}");
    }

    public bool IsValidIndex(int index) => index < pool.Count;
}