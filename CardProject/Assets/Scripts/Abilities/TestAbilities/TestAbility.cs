///<summary> This is a useless ability which will literally just expend mana for no effect, this allows us to test conditions<summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAbility : Ability
{
    public override IEnumerable Activate(){
        Debug.Log("CHAINED EFFECT ACIVATED");
        yield return null;
    }  
}
