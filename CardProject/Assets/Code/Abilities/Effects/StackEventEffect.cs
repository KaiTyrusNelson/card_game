using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class StackEventEffect:Effect{
    [SerializeField] CharacterStackEvent _evt;
    public override IEnumerator Activation(Character c){
        CharacterStackEvent evt = Instantiate(_evt, transform.position, transform.rotation);
        // ESTABLISHED THE BASIC INFORMATION ABOUT THE SUMMON CONDITION
        evt.Char = c;
        evt.AssociatedPlayer = AssociatedCard.Player;
        Manager.Singleton.StackPush(evt);
        yield break;
    }
}