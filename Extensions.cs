using System;
using System.Collections.Generic;
using csharpdsa;

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
    public static void PrintNodes<T>(this List<T> list) where T : Node
    {
        for (var index = 0; index < list.Count; index++)
        {
            var node = list[index];
            String s = (index == list.Count - 1) ? "" : "\n";
            Console.Write($"{index + 1}: {node.value}{s}");
        }
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