using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

namespace DialogueSystem;

public class QuestionNode : Node
{
    public QuestionEffect Effect { get; set; }
    public List<AnswerNode> PossibleAnswers { get; set; }

    internal AnswerNode chosenAnswer = null;
    internal bool QuestionAskedAlready = false;
    public bool RemoveAfterAsked { get; set; }
    
    internal Random rndm = new(DateTime.Now.Millisecond);
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, QuestionEffect Effect, bool removeAfterAsked) : base(Value)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, bool removeAfterAsked) : base(Value)
    {
        Effect = QuestionEffect.Neutral;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, QuestionEffect Effect, bool EndsConversation, bool removeAfterAsked) : base(Value, EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    public QuestionNode(string Value, List<AnswerNode> possibleAnswers, bool EndsConversation, bool removeAfterAsked) : base(Value, EndsConversation)
    {
        Effect = QuestionEffect.Neutral;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, QuestionEffect Effect, bool removeAfterAsked) : base(Value, PerformActionIfChosen)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, bool removeAfterAsked) : base(Value, PerformActionIfChosen)
    {
        Effect = QuestionEffect.Neutral;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, QuestionEffect Effect, bool EndsConversation, bool removeAfterAsked) : base(Value, PerformActionIfChosen, EndsConversation)
    {
        this.Effect = Effect;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    public QuestionNode(string Value,List<AnswerNode> possibleAnswers, Action<Ped> PerformActionIfChosen, bool EndsConversation, bool removeAfterAsked) : base(Value, PerformActionIfChosen, EndsConversation)
    {
        Effect = QuestionEffect.Neutral;
        PossibleAnswers = possibleAnswers;
        RemoveAfterAsked = removeAfterAsked;
    }
    
    internal AnswerNode ChooseAnswer(Conversation convo)
    {
        if (chosenAnswer != null) return chosenAnswer;
        List<AnswerNode> EnabledAnswers = new List<AnswerNode>();
        EnabledAnswers = PossibleAnswers.FindAll(PA => PA.Condition == null || PA.Condition(convo.Ped));
        if (EnabledAnswers.Count == 0)
        {
            throw new NoValidAnswerException($"No Valid Answer for Question Node: {Value}");
        }
        double maxProbability = EnabledAnswers.Max(node => node.Probability);
        List<AnswerNode> nodesWithHighestProbability = EnabledAnswers.Where(node => node.Probability == maxProbability).ToList();
        chosenAnswer = nodesWithHighestProbability[rndm.Next(nodesWithHighestProbability.Count)];
        return chosenAnswer;
    }


    public void InsertVariablesIntoString(string[] replacements)
    {
        for (int i = 0; i < PossibleAnswers.Count;i++)
        {
            AnswerNode node = PossibleAnswers[i];
            for (int j = 0; j < replacements.Length; j++)
            {
                if (i == 0)
                {
                    Value = Value.Replace(j.ToString(), replacements[j]); //replacing the question node if it is the first iteration of the outer loop
                }
                node.Value = node.Value.Replace(j.ToString(), replacements[j]); //replace the ansewr node no matter what iteration of the outer loop we are on
            }     
        }
    }
    
    public void InsertVariablesIntoString(string[] wordsToReplace,string[] replacements)
    {
        for (int i =0; i < PossibleAnswers.Count;i++)
        {
            AnswerNode node = PossibleAnswers[i];
            for (int j = 0; j < replacements.Length; j++)
            {
                if (i == 0)
                {
                    Value = Value.Replace(wordsToReplace[j], replacements[j]); //same as above. we are just using the array "wordsToReplace if the dev does not like using numbers 0 to arrayLength
                }
                node.Value = node.Value.Replace(wordsToReplace[j], replacements[j]);
            }     
        }
    }

}
