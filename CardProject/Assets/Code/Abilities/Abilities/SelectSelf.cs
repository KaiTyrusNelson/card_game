using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SelectSelf : Ability
{
    // DOES NOT HAVE A CUSTOM CRITERIA
    [SerializeField] Effect[] effects;
    public override IEnumerator Active(){
        foreach (Effect e in effects)
        {
            yield return e.Activation(AssociatedCard);
        }

        yield return null;
    }  
}