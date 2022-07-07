using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class StackEventEffect:Effect{
    [SerializeField] CharacterStackEvent _evt;
    [SerializeField] bool instant;
    public override IEnumerator Activation(Character c){
        CharacterStackEvent evt = Instantiate(_evt, transform.position, transform.rotation);
        // ESTABLISHED THE BASIC INFORMATION ABOUT THE SUMMON CONDITION
        evt.Char = c;
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