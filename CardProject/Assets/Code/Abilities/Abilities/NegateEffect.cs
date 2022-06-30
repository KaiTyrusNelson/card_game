using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NegateEffect : Ability
{
    public override bool CheckSelfCondition(){
        // IF THERE IS AN EFFECT TO REMOVE WE WILL REMOVE IT
        return Manager.Singleton.EventStack.Count > 0;
    }
    public override IEnumerator Active(){
        // removes this card own begin effect
        StackEvent t;
        while(Manager.Singleton.EventStack.TryPop(out t) == true)
        {
            if (t is StartOfEffectEvent)
            {
                break;
            }
        }
        // THEN PROCEEDS TO REMOVE THE NEXT CARDS ACTING EFFECT
        Manager.Singleton.NegateEffect();
        yield break;
    }  
}