using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class SelectionList
{
    public TurnPlayer _ownerplayer;
    public List<Iteratable> iterationList = new List<Iteratable>();
    public List<TargettingCondition> conditions = new List<TargettingCondition>();

    public void AddCondition(TargettingCondition c)
    {
        conditions.Add(c);
    }
    public class Iteratable
    {
        public Character c;
        public Placement location;
        public bool ally;
    }
    
    public void ClearConditions(){
        conditions.Clear();
    }
    public void Clear(){
        iterationList.Clear();
    }
    public int Count(){
        return iterationList.Count;
    }
    public bool CheckConditions(Character c){
        foreach (TargettingCondition x in conditions){
            if (!x.Check(c))
            {
                return false;
            }
        }
        return true;
    }
    public void AddDeck(TurnPlayer _player)
    {
        bool ally;
        if (_player == _ownerplayer){
            ally = true;
        }else{
            ally=false;
        }

        foreach (Character c in Manager.Players[_player].PlayerDeck.Cards)
        {
            if (c != null)
            {
                if (CheckConditions(c)){
                    Iteratable iter = new Iteratable();
                    iter.c =  c;
                    iterationList.Add(iter);                                                            
                }
            }
        }
    }

    public void AddHand(TurnPlayer _player)
    {
        bool ally;
        if (_player == _ownerplayer){
            ally = true;
        }else{
            ally=false;
        }

        foreach (Character c in Manager.Players[_player].PlayerHand.Cards)
        {
            if (c != null)
            {
                if (CheckConditions(c)){
                    Iteratable iter = new Iteratable();
                    iter.c =  c;
                    iterationList.Add(iter);                                                            
                }
            }
        }
    }

    // ADDS THINGS TO THE GAMEBOARD DEPENDING ON THE CONDITIONS SET IN PLACE
    public void AddBoard(TurnPlayer _player)
    {
        // DETERMINES IF IT IS OPPONENT BOARD OR NOT
        bool ally;
        if (_player == _ownerplayer){
            ally = true;
        }else{
            ally=false;
        }
        // CYCLES THROUGH THE CHARACTERS TO FIND THE TARGET
        foreach (Character c in Manager.Players[_player].PlayerBoard.ToArray())
        {
            if (c != null)
            {
                if (CheckConditions(c)){
                    Iteratable iter = new Iteratable();
                    iter.c =  c;
                    iterationList.Add(iter);                                                            
                }
            }
        }
    }

    // ADDS CHARACTERS FROM THE GRAVEYARD TO OUR SELECTIONS
    public void AddGraveyard(TurnPlayer _player)
    {
        // DETERMINES IF IT IS OPPONENT BOARD OR NOT
        bool ally;
        if (_player == _ownerplayer){
            ally = true;
        }else{
            ally=false;
        }
        // CYCLES THROUGH THE CHARACTERS TO FIND THE TARGET
        foreach (Character c in Manager.Players[_player].PlayerGraveyard.Cards)
        {
            if (c != null)
            {
                if (CheckConditions(c)){
                    Iteratable iter = new Iteratable();
                    iter.c =  c;
                    iterationList.Add(iter);                                                            
                }
            }
        }
    }


    #region Messages

    public void SendEndOfSelectionMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.confirmSelectionEnd);
        NetworkManagerV2.Instance.server.Send(m ,(ushort)_ownerplayer);
    }
    public void SendMesssage(int minSelections, int maxSelections)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.selectionRequest);
        // FIRST ADDS THE COUNT OBJECT
        m.Add((ushort)iterationList.Count);
        // ADDS THE AMOUNT OF SELECTIONS WHICH ARE POSSIBLE
        m.Add((ushort)minSelections);
        m.Add((ushort)maxSelections);
        // ADDS ALL OF THE INFORMATION
        foreach (Iteratable iter in iterationList)
        {
            // FOR NOW WE WILL JUST SEND THE ASSOCIATED CARDS ID
            // SOON WE WILL BE MORE SPECIFIC
            m.Add(iter.c.Id);
        }
        NetworkManagerV2.Instance.server.Send(m ,(ushort)_ownerplayer);
    }

    #endregion
}

public enum Placement
{
    Deck =1,
    Hand,
    Board,
    Graveyard,
}