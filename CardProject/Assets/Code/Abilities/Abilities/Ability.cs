using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: CREATE MULTIPLE KINDS OF EFFECTS
public abstract class Ability : MonoBehaviour
{
    #region Parameters
    // DETERMINES THE PLAYER WHOM THE ABILITY IS TRIGGERED FROM
    [SerializeField] Character _associatedCard;
    public Character AssociatedCard{get => _associatedCard; set{_associatedCard = value;}}
    // CONDITIONS FOR THE ABILITIES COROUTINE TO BE ACTIVATED
    [SerializeField] Condition[] _conditions;
    // DETERMINES WHETHER THE CHAIN FOR THE NEXT ABILITY IS MANDATORY OR NOT, IF IT IS MANDATORY, THEN THE FOLLOWING CONDITIONS MUST BE SATISFIES ASWELL 
    [SerializeField] bool _mandatoryChain;
    // THE ABILITY WHICH LINKS TO THIS ONE
    [SerializeField] Ability _followUp;
    [SerializeField] ushort manaCost;
    #endregion
    
    /// <summary> This is the active effect of the ability
    public abstract IEnumerator Active();

    /// <summary> This is what happens when the ability is activated<summary>
    // TODO: ADD MULTICHAINING
    // TODO: ADD TWO PART EFFECTS / CONDITIONAL EFFECTS
    public IEnumerator Activate(){
        // INFORMS THE CLIENTS WHO IS PLAYING THEIR CARD
        ResponseMessages.SendActingPlayer(AssociatedCard.Player);
        // SUBTRACTS THE MANA COST
        Manager.Players[AssociatedCard.Player].CurrentMana-=manaCost;
        // ACTIVATES THE ABILITY
        yield return StartCoroutine(Active());
        // INFORMS USERS OF THE RESOLVE EFFECT
        Debug.Log("Effect has resolved");
    }  

    /// <summary> This is the innate condition required to activate the ability<summary>
    public virtual bool CheckSelfCondition()
    {
        return true;
    }

    ///<summary> Checks if the condition to cast the ability are activated<summary>
    public virtual bool CheckConditions(){
    // if each of the condeitions is met
    // TO DO, VERIFY MORE CONDITIONS
        if (!CheckSelfCondition())
        {
            Debug.Log("Self condition failed");
            return false;
        }
        foreach (Condition condition in _conditions){
            if (!condition.Check())
            {
                return false;
            }
        }
        // CHECKS IF THERE IS ENOUGH MANA TO CAST THE ABILITY
        if(Manager.Players[_associatedCard.Player].CurrentMana - manaCost < 0)
        {
            Debug.Log("Mana condition failed");
            return false;
        }
        if (_mandatoryChain && _followUp != null){
            return _followUp.CheckConditions();
        }
        Debug.Log("Ability passes all checks");
        return true;
    }
}
