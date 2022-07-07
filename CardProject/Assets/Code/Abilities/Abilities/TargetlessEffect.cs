using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TargetlessEffect : Ability
{
    [SerializeField] TargetlessStackEvent _evt;
    [SerializeField] bool instant;

    public override bool CheckSelfCondition()
    {
        // TESTS TO SEE IF THE EVENT IS VALID
        TargetlessStackEvent evt = Instantiate(_evt, transform.position, transform.rotation);
        evt.AssociatedPlayer = AssociatedCard.Player;
        
        bool valid = evt.CheckCondition();
        Destroy(evt.gameObject);

        return valid;
    }
    public override IEnumerator Active(){
        TargetlessStackEvent evt = Instantiate(_evt, transform.position, transform.rotation);
        // ESTABLISHED THE BASIC INFORMATION ABOUT THE SUMMON CONDITION
        evt.AssociatedPlayer = AssociatedCard.Player;
        if (!instant){
            Manager.Singleton.StackPush(evt);
        }else{
            // if is instant trigger immediatley
            yield return StartCoroutine(evt.Activate());
            Destroy(evt.gameObject);
        }
        
        yield break;
    }
}