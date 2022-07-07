using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NegationEvent : TargetlessStackEvent
{
    public override bool CheckCondition(){
        // IF THERE IS AN EFFECT TO REMOVE WE WILL REMOVE IT
        foreach (StackEvent e in Manager.Singleton.EventStack)
        {
            if (e is StartOfEffectEvent)
            {
                return true;
            }
        }
        return false;
    }
    public override IEnumerator Activate(){
        // removes this card own begin effect
        StackEvent t;
        while(Manager.Singleton.EventStack.TryPop(out t) == true)
        {
            if (t is StartOfEffectEvent)
            {
                Destroy(t.gameObject);
                break;
            }
            Destroy(t.gameObject);
        }
        // THEN PROCEEDS TO REMOVE THE NEXT CARDS ACTING EFFECT
        Manager.Singleton.NegateEffect();
        yield break;
    }  
}