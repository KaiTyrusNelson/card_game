using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    // DETERMINES THE PLAYER WHOM THE ABILITY IS TRIGGERED FROM
    [SerializeField] TurnPlayer _player;
    // CONDITIONS FOR THE ABILITIES COROUTINE TO BE ACTIVATED
    [SerializeField] Condition[] _conditions;
    // DETERMINES WHETHER THE CHAIN FOR THE NEXT ABILITY IS MANDATORY OR NOT, IF IT IS MANDATORY, THEN THE FOLLOWING CONDITIONS MUST BE SATISFIES ASWELL 
    [SerializeField] bool _mandatoryChain;
    // THE ABILITY WHICH LINKS TO THIS ONE
    [SerializeField] Ability _followUp;
    public abstract IEnumerable Activate();  
    [SerializeField] ushort manaCost;

    public virtual bool CheckConditions(){
    // if each of the condeitions is met
    // TO DO, VERIFY MORE CONDITIONS
        foreach (Condition condition in _conditions){
            if (!condition.Check())
            {
                return false;
            }
        }
        // CHECKS IF THERE IS ENOUGH MANA TO CAST THE ABILITY
        if(Manager.Singleton.currentMana[_player] - manaCost < 0)
        {
            return false;
        }
        if (_mandatoryChain && _followUp != null){
            return _followUp.CheckConditions();
        }
        Debug.Log("Ability passes all checks");
        return true;
    }
}
