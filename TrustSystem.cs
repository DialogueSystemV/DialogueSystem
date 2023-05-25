using System;
using System.Collections.Generic;
using DialogueSystem;

namespace DialogueSystem;

public class TrustSystem
{
    /// <summary>
    /// Conversation the trust system will grab the information from.
    /// </summary>
    public Conversation conversation { get; set; }
    
    /// <summary>
    /// A dictionary in order to apply different weightages to the different question effects.
    /// </summary>
    public Dictionary<QuestionEffect, int> effectValues { get; set; }
    
    /// <summary>
    /// The negative threshold will be inclusive and determines whether the final effect of the conversation is negative or neutral.
    /// </summary>
    public int negativeThreshold { get; set; }
    /// <summary>
    /// The positive threshold will be inclusive and determines whether the final effect of the conversation is positive or neutral.
    /// </summary>
    public int positiveThreshold { get; set; }
    
    /// <summary>
    /// This is the starting trust level that the player and the person the player is talking to will have.
    /// This will have to be a starting amount that makes context with negativeThreshold and positiveThreshold.
    /// </summary>
    public int trustLevel { get; set; }

    
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
        this.conversation = conversation;
        this.trustLevel = trustLevel;
        this.positiveThreshold = positiveThreshold;
        this.negativeThreshold = negativeThreshold;
        effectValues = new Dictionary<QuestionEffect, int>()
        {
            { QuestionEffect.NEGATIVE, effectArray[0] },
            { QuestionEffect.NEUTRAL, effectArray[1] },
            { QuestionEffect.POSITIVE, effectArray[2] },
        };
    }
    
    private void CalculateFinalTrustLevel()
    {
        int pos = conversation.numberOfPositive;
        int neg = conversation.numberOfNegative;
        int neutral = conversation.numberOfNeutral;

        trustLevel += (pos * effectValues[QuestionEffect.POSITIVE]);
        trustLevel += (neutral * effectValues[QuestionEffect.NEUTRAL]);
        trustLevel += (neg * effectValues[QuestionEffect.NEGATIVE]);
    }
    
    /// <summary>
    /// This will return the overall QuestionEffect of the conversation by using the weightages and number of positive,negative, and neutral questions that were asked in the conversation.
    /// This should be only ran after the conversation has been ran or else the effect will be wrong.
    /// </summary>
    /// <returns>Overall QuestionEffect of the conversation that the trust system is grabbing information from</returns>
    public QuestionEffect CalculateFinalEffect()
    {
        CalculateFinalTrustLevel();
        if (trustLevel <= negativeThreshold)
        {
            return QuestionEffect.NEGATIVE;
        }
        else if (trustLevel < positiveThreshold && trustLevel > negativeThreshold)
        {
            return QuestionEffect.NEUTRAL;
        }
        else
        {
            return QuestionEffect.POSITIVE;
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
        if (trustLevel <= negativeThreshold)
        {
            calculatedEffect = QuestionEffect.NEGATIVE;
        }
        else if (trustLevel < positiveThreshold && trustLevel > negativeThreshold)
        {
            calculatedEffect = QuestionEffect.NEUTRAL;
        }
        else
        {
            calculatedEffect = QuestionEffect.POSITIVE;
        }
        
        switch (calculatedEffect)
        {
            case QuestionEffect.POSITIVE:
                arrayOfEffects[2]();
                break;
            case QuestionEffect.NEUTRAL:
                arrayOfEffects[1]();
                break;
            case QuestionEffect.NEGATIVE:
                arrayOfEffects[0]();
                break;
        }
    }
}