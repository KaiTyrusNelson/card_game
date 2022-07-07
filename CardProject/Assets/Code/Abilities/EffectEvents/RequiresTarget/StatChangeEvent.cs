using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatChangeEvent: CharacterStackEvent{
    [SerializeField] public ushort _atk;
    [SerializeField] public ushort _maxHp;
    [SerializeField] public ushort _hp;
    public override IEnumerator Activate()
    {
        Char.Attack += _atk;
        Char.MaxHp += _maxHp;
        Char.Hp += _hp;
        yield return null;
    }
}