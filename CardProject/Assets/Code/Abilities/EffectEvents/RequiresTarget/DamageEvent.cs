using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageEvent: CharacterStackEvent{
    [SerializeField] public ushort _damage;
    public override IEnumerator Activate()
    {
        if (CharacterFunctions.IsOnBoard(Char))
        {
            Char.TakeDamage(_damage);
        }
        yield return null;
    }
}