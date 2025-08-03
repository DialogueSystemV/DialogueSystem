using DialogueSystem.Core;

namespace DialogueSystem
{
    public static class ListExtensions
    {
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
}
