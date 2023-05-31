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
    /// A dictionary in order to apply different weightages to the different question effects.
    /// Values should go in the order of: NEGATIVE, NEUTRAL, POSITIVE
    /// </summary>
    public Dictionary<QuestionEffect, int> EffectValues { get; set; }
    
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

    public event EventHandler<QuestionEffect> OnTrustLevelChanged;

    
    /// <summary>
    /// Initializes an instance of the TrustSystem Object.
    /// </summary>
    /// <param name="conversation">Conversation the trust system will grab the information from.</param>
    /// <param name="effectArray">A dictionary in order to apply different weightages to the different question effects.</param>
    /// <param name="positiveThreshold">The positive threshold will be inclusive and determines whether the final effect of the conversation is positive or neutral.</param>
    /// <param name="negativeThreshold">The negative threshold will be inclusive and determines whether the final effect of the conversation is negative or neutral.</param>
    /// <param name="trustLevel">This is the starting trust level that the player and the person the player is talking to will have.
    /// This will have to be a starting amount that makes context with negativeThreshold and positiveThreshold.</param>
    public TrustSystem(Conversation conversation, int[] effectArray, int positiveThreshold, int negativeThreshold, int trustLevel)
    {
        Conversation = conversation;
        TrustLevel = trustLevel;
        PositiveThreshold = positiveThreshold;
        NegativeThreshold = negativeThreshold;
        EffectValues = new Dictionary<QuestionEffect, int>()
        {
            { QuestionEffect.Negative, effectArray[0] },
            { QuestionEffect.Neutral, effectArray[1] },
            { QuestionEffect.Positive, effectArray[2] },
        };
        CheckTrustLevel();
    }
    
    private void CalculateFinalTrustLevel()
    {
        int pos = Conversation.NumberOfPositive;
        int neg = Conversation.NumberOfNegative;
        int neutral = Conversation.NumberOfNeutral;

        TrustLevel += (pos * EffectValues[QuestionEffect.Positive]);
        TrustLevel += (neutral * EffectValues[QuestionEffect.Neutral]);
        TrustLevel += (neg * EffectValues[QuestionEffect.Negative]);
    }
    
    /// <summary>
    /// This will return the overall QuestionEffect of the conversation by using the weightages and number of positive,negative, and neutral questions that were asked in the conversation.
    /// This should be only ran after the conversation has been ran or else the effect will be wrong.
    /// </summary>
    /// <returns>Overall QuestionEffect of the conversation that the trust system is grabbing information from</returns>
    public QuestionEffect CalculateFinalEffect()
    {
        CalculateFinalTrustLevel();
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

    /// <summary>
    /// This will use the weightages and number of positive,negative, and neutral questions that were asked in the conversation in order to figure out the overall QuestionEffect.
    /// After that, it will play the method corresponding in the Action[] arrayOfEffects
    /// This should be only ran after the conversation has been ran or else the effect will be wrong.
    /// </summary>
    /// <param name="arrayOfEffects"> Array of Actions that will be played automatically after the QuestionEffect has been calculated.
    /// The array should be in the order: Negative outcome, neutral outcome, Positive Outcome with indices 0,1,2 respectively </param> 
    public void CalculateFinalEffect(Action[] arrayOfEffects)
    {
        CalculateFinalTrustLevel();
        QuestionEffect calculatedEffect;
        if (TrustLevel <= NegativeThreshold)
        {
            calculatedEffect = QuestionEffect.Negative;
        }
        else if (TrustLevel < PositiveThreshold && TrustLevel > NegativeThreshold)
        {
            calculatedEffect = QuestionEffect.Neutral;
        }
        else
        {
            calculatedEffect = QuestionEffect.Positive;
        }
        
        switch (calculatedEffect)
        {
            case QuestionEffect.Positive:
                arrayOfEffects[2]();
                break;
            case QuestionEffect.Neutral:
                arrayOfEffects[1]();
                break;
            case QuestionEffect.Negative:
                arrayOfEffects[0]();
                break;
        }
    }

    private void CheckTrustLevel()
    {
        GameFiber.StartNew(delegate
        {
            QuestionEffect startingEffect = CalculateFinalEffect();
            while (Conversation.ConversationThread.IsAlive)
            {
                var currEffect= CalculateFinalEffect();
                if (currEffect != startingEffect)
                {
                    OnTrustLevelChanged?.Invoke(this, currEffect);
                }
            }
        });
    }
    
    
}