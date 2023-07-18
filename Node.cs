using System;
using Rage;

namespace DialogueSystem;

public class Node
{
    public string Value { get; set; }
    public bool EndsConversation { get; set; }
    public Action<Ped>? PerformActionIfChosen{ get; set; }
    public Ped? Ped { get; set; }

    public Node(string Value)
    {
        this.Value = Value;
        EndsConversation = false;
        PerformActionIfChosen = null;
        Ped = null;
    }
    
    public Node(string Value, bool EndsConversation)
    {
        this.Value = Value;
        this.EndsConversation = EndsConversation;
        PerformActionIfChosen = null;
        Ped = null;
    }
    
    public Node(string Value, Action<Ped> PerformActionIfChosen, Ped Ped)
    {
        this.Value = Value;
        EndsConversation = false;
        this.PerformActionIfChosen = PerformActionIfChosen;
        this.Ped = Ped;
        if (Ped == null)
        {
            throw new ArgumentNullException("Ped cannot be null");
        }
    }
    
    public Node(string Value,Action<Ped> PerformActionIfChosen,Ped Ped,bool EndsConversation)
    {
        this.Value = Value;
        this.EndsConversation = EndsConversation;
        this.PerformActionIfChosen = PerformActionIfChosen;
        this.Ped = Ped;
        if (Ped == null)
        {
            throw new ArgumentNullException("Ped cannot be null");
        }
    }

}