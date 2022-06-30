using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public class StartOfEffectEvent : StackEvent
{
    // THIS CLASS IS MEANT TO TRACK THE BEGINNING OF ABILITIES SO WE KNOW WHAT EFFECTS WERE ACTIVATED LAST

    // ACTIVATION DOES NOTHING
    public override IEnumerator Activate()
    {
        yield break;
    }
}