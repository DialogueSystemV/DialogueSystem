using System;
using System.Collections.Generic;
using DialogueSystem;
using Rage;

namespace DialogueSystem;

public class QuestionPool
{
    public List<QuestionAndAnswer> Pool { get; set; }

    public string[] StringsToFormat { get; set; }

    public QuestionPool(List<QuestionAndAnswer> pool)
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
    public string GetAnswer(int index)
    {
        return IsValidIndex(index) ? Pool[index].Answer : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {Pool.Count - 1}");
    }
            
    public QuestionEffect GetEffect(int index)
    {
        return IsValidIndex(index) ? Pool[index].Effect : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {Pool.Count - 1}");
    }
    public string GetQuestion(int index)
    {
        return IsValidIndex(index) ? Pool[index].Question : throw new ArgumentOutOfRangeException("index",$"Index is invalid. Index wanted: {index}. Max valid index: {Pool.Count - 1}");
    }

    public void FormatStrings()
    {
        for (int i = 0; i < StringsToFormat.Length; i++)
        {
            foreach (QuestionAndAnswer qanda in Pool)
            {
                Game.LogTrivial($"Should be replaced: {i}");
                Game.LogTrivial($"Should be replaced with: {StringsToFormat[i]}");
                qanda.FormatString(StringsToFormat[i],$"{i}");
            }
        }
    }
    internal void RemoveQuestionAnswer(int index)
    {
        Pool.RemoveAt(index);
    }
    public bool IsValidIndex(int index) => index < Pool.Count;
}