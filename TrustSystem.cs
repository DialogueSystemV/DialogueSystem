using System;
using System.Collections.Generic;
using DialogueSystem;
using Rage;

namespace DialogueSystem;

public class TrustSystem
{
    /// <summary>
    /// Conversation the trust system will grab the information from.
    /// </summary>
    public Conversation Conversation { get; set; }
    
    /// <summary>
    /// Number of positive questions asked by the player. The starting value is 0.
    /// </summary>
    public int NumberOfPositive { get; internal set; }
    /// <summary>
    /// Number of negative questions asked by the player. The starting value is 0.
    /// </summary>
    public int NumberOfNegative { get; internal set; }
    /// <summary>
    /// Number of neutral questions asked by the player. The starting value is 0.
    /// </summary>
    public int NumberOfNeutral { get; internal set; }
    
    /// <summary>
    /// A dictionary in order to apply different weightages to the different question effects.
    /// Values should go in the order of: NEGATIVE, NEUTRAL, POSITIVE
    /// </summary>
    public Dictionary<QuestionEffect, int> EffectDict { get; set; }
    
    /// <summary>
    /// The negative threshold will be inclusive and determines whether the final effect of the conversation is negative or neutral.
    /// </summary>
    public int NegativeThreshold { get; set; }
    /// <summary>
    /// The positive threshold will be inclusive and determines whether the final effect of the conversation is positive or neutral.
    /// </summary>
    public int PositiveThreshold { get; set; }
    
    /// <summary>
    /// This is the starting trust level that the player and the person the player is talking to will have.
    /// This will have to be a starting amount that makes context with negativeThreshold and positiveThreshold.
    /// </summary>
    public int TrustLevel { get; set; }
    
    public QuestionEffect TrustEffect { get; internal set; }

    public event EventHandler<QuestionEffect> OnTrustLevelChanged;

    public int InitialTrustLevel { get; private set; }
    
    public QuestionEffect InitialTrustEffect { get; private set; }

    /// <summary>
    /// Initializes an instance of the TrustSystem Object.
    /// </summary>
    /// <param name="conversation">Conversation the trust system will grab the information from.</param>
    /// <param name="effectDict">A dictionary in order to apply different weightages to the different question effects.</param>
    /// <param name="positiveThreshold">The positive threshold will be inclusive and determines whether the final effect of the conversation is positive or neutral.</param>
    /// <param name="negativeThreshold">The negative threshold will be inclusive and determines whether the final effect of the conversation is negative or neutral.</param>
    /// <param name="trustLevel">This is the starting trust level that the player and the person the player is talking to will have.
    /// This will have to be a starting amount that makes context with negativeThreshold and positiveThreshold.</param>
    public TrustSystem(Conversation conversation, Dictionary<QuestionEffect,int> EffectDict, int positiveThreshold, int negativeThreshold, int trustLevel)
    {
        Conversation = conversation;
        TrustLevel = trustLevel;
        NumberOfNegative = 0;
        NumberOfNeutral = 0;
        NumberOfPositive = 0;
        InitialTrustLevel = trustLevel;
        PositiveThreshold = positiveThreshold;
        NegativeThreshold = negativeThreshold;
        this.EffectDict = EffectDict;
        TrustEffect = CalculateEffect();
        InitialTrustEffect = TrustEffect;
        Conversation.OnQuestionSelect += ConversationOnOnQuestionSelect;
    }

    private void ConversationOnOnQuestionSelect(object sender, (QuestionNode, AnswerNode) e)
    {
        UpdateNumbers(e.Item1.Effect);
        CalculateTrust();
        if (e.Item1.EndsConversation)
        {
            Conversation.OnQuestionSelect -= ConversationOnOnQuestionSelect;
        }
    }

    private void CalculateTrust()
    {
        int pos = NumberOfPositive;
        int neg = NumberOfNegative;
        int neutral = NumberOfNeutral;
        int trust = InitialTrustLevel;
        trust += (pos * EffectDict[QuestionEffect.Positive]);
        trust += (neutral * EffectDict[QuestionEffect.Neutral]);
        trust += (neg * EffectDict[QuestionEffect.Negative]);
        TrustLevel = trust;
        
        
        QuestionEffect oldTrust = TrustEffect;
        TrustEffect = CalculateEffect();
        if (oldTrust != TrustEffect)
        {
            OnTrustLevelChanged?.Invoke(this, TrustEffect);
        }
    }
    
    /// <summary>
    /// This will return the overall QuestionEffect of the conversation by using the weightages and number of positive,negative, and neutral questions that were asked in the conversation.
    /// This should be only ran after the conversation has been ran or else the effect will be wrong.
    /// </summary>
    /// <returns>Overall QuestionEffect of the conversation that the trust system is grabbing information from</returns>
    private QuestionEffect CalculateEffect()
    {
        if (TrustLevel <= NegativeThreshold)
        {
            return QuestionEffect.Negative;
        }
        else if (TrustLevel < PositiveThreshold && TrustLevel > NegativeThreshold)
        {
            return QuestionEffect.Neutral;
        }
        else
        {
            return QuestionEffect.Positive;
        }
    }
    
    private void UpdateNumbers(QuestionEffect effect)
    {
        switch (effect)
        {
            case QuestionEffect.Positive:
                NumberOfPositive++;
                break;
            case QuestionEffect.Neutral:
                NumberOfNeutral++;
                break;
            case QuestionEffect.Negative:
                NumberOfNegative++;
                break;
        }
    }
}

