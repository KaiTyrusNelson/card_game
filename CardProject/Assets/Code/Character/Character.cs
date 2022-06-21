using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;


public class Character : MonoBehaviour
{   
    #region Definitions
    
    [SerializeField] Ability[] _abilities;
    public Ability[] Abilities {get => _abilities;}

    [SerializeField] string _id;
    public string Id{get=>_id;}
    [SerializeField] ushort _maxHp;
    public ushort MaxHp{get => _maxHp; set { _maxHp = value; }}
    [SerializeField] ushort _hp;
    public ushort Hp {get => _hp; set {_hp = value;}}
    [SerializeField] ushort _attack;
    public ushort Attack {get=>_attack; set{ _attack = value;}}
    [SerializeField] TurnPlayer _player;
    public TurnPlayer Player{ get => _player; set{ _player = value;}}
    [SerializeField] ushort _manaCost;
    public ushort ManaCost {get=> _manaCost; set{ _manaCost  =  value;}}

    // DETERMINES IF THE CHARACTER HAS ATTACKED THIS TURN
    public bool HasAttacked = false;
    #endregion


    #region Functions
    public void TakeDamage(ushort damage){
        _hp = (ushort)Math.Max(0, _hp - damage);
    }

    public bool AttackCharacter(Character other)
    {
        if (!HasAttacked)
        {
            other.TakeDamage(this.Attack);
            this.TakeDamage(other.Attack);
            HasAttacked = true;

            // TODO: ADD ON COMBAT EFFECTS
            return true;
        }
        Debug.Log("This character cannot attack");
        return false;
    }

    public void ResetAttacked()
    {
        HasAttacked = false;
    }

    #endregion

    #region VisualizationFunctions
    [SerializeField] TMP_Text attackTextBox;
    [SerializeField] TMP_Text defenseTextBox;
    


    public void OnValidate(){
      _id = this.gameObject.name;
    }

    void Update(){
        defenseTextBox.SetText(_hp.ToString());
        attackTextBox.SetText(_attack.ToString());
    }
    #endregion




}
