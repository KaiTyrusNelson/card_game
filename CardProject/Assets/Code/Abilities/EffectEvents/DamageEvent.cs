using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageEvent: StackEvent{
    public ushort _damage;
    public Character _c;

    public void InitializeVariables(Character c, ushort damage)
    {
        _damage = damage;
        _c = c;
    }
    public override IEnumerator Activate()
    {
        if (CharacterFunctions.IsOnBoard(_c))
        {
            _c.TakeDamage(_damage);
        }
        yield return null;
    }
}