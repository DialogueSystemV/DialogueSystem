using System;
using Rage;

namespace DialogueSystem;

public class Node
{
    public string Value { get; set; }
    public bool EndsConversation { get; set; }
    public Action<Ped>? PerformActionIfChosen{ get; set; }

    public Node(string Value)
    {
        this.Value = Value;
        EndsConversation = false;
        PerformActionIfChosen = null;
    }
    
    public Node(string Value, bool EndsConversation)
    {
        this.Value = Value;
        this.EndsConversation = EndsConversation;
        PerformActionIfChosen = null;
    }
    
    public Node(string Value, Action<Ped> PerformActionIfChosen)
    {
        this.Value = Value;
        EndsConversation = false;
        this.PerformActionIfChosen = PerformActionIfChosen;
    }
    
    public Node(string Value,Action<Ped> PerformActionIfChosen,bool EndsConversation)
    {
        this.Value = Value;
        this.EndsConversation = EndsConversation;
        this.PerformActionIfChosen = PerformActionIfChosen;
    }

}