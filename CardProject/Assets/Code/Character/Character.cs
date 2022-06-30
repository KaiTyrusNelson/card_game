using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;


public enum CardLocations
{
    Deck = 1,
    Hand,
    Board,
    Graveyard,
    // WILL BE ADDED IN FUTURE
    Banished,
    ExtraDeck
}

public class Character : MonoBehaviour
{   
    #region Definitions
    
    // THE ABILITY WHICH IS ACTIVATED BY THE PLAYER ON THE BOARD (CHAINABLE EFFECT)
    [SerializeField] Ability _boardAbility;
    public Ability BoardAbility {get => _boardAbility;}

    // THE SUMMON ABILITY
    [SerializeField] public Ability summonAbility;
    // IF THE SUMMON ABILITY MUST BE ACTIVATED WHEN THE CHARACTER IS SUMMONED
    [SerializeField] public bool optionalSummonAbility;

    [SerializeField] public Ability HandAbility;

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

    #region CardProperties
    [SerializeField] public characterElements element;
    [SerializeField] public characterAttributes[] attributes;
    #endregion

    // DETERMINES IF THE CHARACTER HAS ATTACKED THIS TURN
    public bool HasAttacked = false;
    #endregion


    public CardLocations Location = CardLocations.Deck;
    #region Functions

    public bool IsElement(characterElements elm)
    {
        return (elm == element);
    }
    public bool hasAttribute(characterAttributes atr)
    {
        foreach (characterAttributes attribute in attributes){
            if (atr == attribute)
                return true;
        }
        return false;
    }
    public IEnumerator OnSummon()
    {
        
        Debug.Log("ON SUMMON CALLED");
        if(summonAbility != null){
            Debug.Log("Summon ability has been triggered");
            // IF THE ABILITY CANNOT BE ACTIVATED
            if (!summonAbility.CheckConditions())
            {
                yield break;
            }

            if (!optionalSummonAbility)
            {
                yield return summonAbility.Activate();
            }
            else
            {
                ResponseMessages.SendActingPlayer(_player);
                // REQUESTS THE ABILITY TRIGGER
                Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.chainRequest);
                NetworkManagerV2.Instance.server.Send(m ,(ushort)_player);
                while(true){
                    yield return null;
                    // IF AN END MESSAGE IS SENT, THEY DO NOT WISH TO CHAIN
                    if(Manager.Players[_player].DontChainCall())
                    {
                        Debug.Log("Player has chosen not to chain");
                        break;
                    }
                    // IF THE CHAIN CALL MESSAGE IS SENT WE WILL ENTER A CHAIN EVENT
                    if(Manager.Players[_player].ChainCall())
                    {
                        Debug.Log("Player has chosen to chain");
                        // CREATES A CHAIN CALL AWAITING THAT PLAYERS RESPONSE
                        yield return summonAbility.Activate();
                        break;
                    }
                }
            }
        }
        yield break;
    }

    public void RemoveFromCurrentLocation()
    {
        switch(Location)
        {
            case(CardLocations.Deck):
            {
                Manager.Players[Player].PlayerDeck.RemoveCard(this);
                break;
            }
            case(CardLocations.Board):
            {
                Manager.Players[Player].PlayerBoard.RemoveCard(this);
                break;
            }
            case(CardLocations.Hand):
            {
                Manager.Players[Player].PlayerHand.RemoveCard(this);
                break;
            }
            case(CardLocations.Graveyard):
            {
                Manager.Players[Player].PlayerGraveyard.RemoveCard(this);
                break;
            }
            case(CardLocations.Banished):
            {
                Manager.Players[Player].Banished.RemoveCard(this);
                break;
            }
        }
    }
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


public enum characterElements
{
    dark
}
public enum characterAttributes
{
    warrior   
}