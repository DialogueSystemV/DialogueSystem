using System.IO;
using DialogueSystem.Core;
using DialogueSystem.Engine;
using DialogueSystem.UI;
using Rage;
using RAGENativeUI;

namespace DialogueSystem.API;

public static class Loader
{
    public static Conversation LoadDialogue(string filePath, UIMenu menu)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception("Dialogue file doesn't exist!");
        }

        string jsonContent;
        try
        {
            jsonContent = File.ReadAllText(filePath);
            Game.LogTrivial($"Successfully read JSON from {filePath}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading file {filePath}");
        }
        Graph graph = DialogueLoader.ParseGraphManually(jsonContent);
        return new Conversation(graph, menu, graph.nodesToStartConversation);
    }
}
