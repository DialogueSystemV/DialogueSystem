using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static List<T> RemoveWhere<T>(this List<T> list, Predicate<T> condition)
    {
        List<T> filteredList = new List<T>();

        foreach (var item in list)
        {
            if (!condition(item))
            {
                filteredList.Add(item);
            }
        }
        return filteredList;
    }
}

public class NoValidAnswerException : Exception
{
    public NoValidAnswerException()
    {
        
    }
    public NoValidAnswerException(string message)
        : base(message)
    {
    }

    public NoValidAnswerException(string message, Exception inner)
        : base(message, inner)
    {
    }
}