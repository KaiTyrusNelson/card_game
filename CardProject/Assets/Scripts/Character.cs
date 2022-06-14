using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Character : MonoBehaviour
{
    [SerializeField]
    int maxHp;
    [SerializeField]
    int hp;
    [SerializeField]
    int attack;

    [SerializeField]
    ushort manaCost;
    public ushort ManaCost {get =>manaCost;}
    [SerializeField] TMP_Text attackTextBox;
    [SerializeField] TMP_Text defenseTextBox;


    [SerializeField] Ability[] _abilities;
    public Ability[] Abilities {get => _abilities;}

    void OnValidate(){
        defenseTextBox.SetText(hp.ToString());
        attackTextBox.SetText(attack.ToString());
    }

    void takeDamage(int damage){
        hp = Math.Max(0, hp - damage);
    }


}
