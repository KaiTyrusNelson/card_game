using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Hand : MonoBehaviour
{
    [SerializeField] List<Character> cards;
    [SerializeField] public int MaxSize;
    [SerializeField] public Player Owner; 
    [SerializeField] public TurnPlayer Player;

    public int GetCount(){
        return cards.Count;
    }
    public Character GetCard(int loc){
        if (loc >= cards.Count || loc<0)
            return null;
        return cards[loc];
    }
    public bool IsFullHand(){
        if (cards.Count <MaxSize){
            return false;
        }else{
            return true;
        }
    }
    public void AddCard(Character c){
        c.transform.SetParent(this.transform);
        cards.Add(c);
        // INFORMS THE CLIENTS ABOUT THE DRAWN CARDS--
        SendSelfDrawMessage(Player, c);
        SendOpponentDrawMessage(Player.OppositePlayer());
    }

    public void RemoveCard(int position)
    {
        cards.RemoveAt(position);
        // INFORMS THE CLIENTS ABOUT THE REMOVED CARD
        SendSelfRemoveCardMessage(Player, position);
        SendOpponentRemoveCardMessage(Player.OppositePlayer(), position);
    }
    public void PlayCardFromPosition(int position, int x, int y)
    {
        Character c = cards[position];
        RemoveCard(position);
        Owner.PlayerBoard.SetAt(c,x,y);
    }   

    #region Messages
        /// <summary> informs a client that their opponent has added a card to their hand <summary>
        public static void SendOpponentDrawMessage(TurnPlayer opponentId){
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.opponentDrawCard);
            NetworkManagerV2.Instance.server.Send(m, (ushort)opponentId);
        }
        /// <summary> informs a client that they have drawn a character <summary>
        public static void SendSelfDrawMessage(TurnPlayer id, Character c){
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.selfDrawCard);
            m.Add(c.Id);
            m.Add(c.Hp);
            m.Add(c.Attack);
            m.Add(c.ManaCost);
            NetworkManagerV2.Instance.server.Send(m, (ushort)id);
        }
        /// <summmary> informs the client a card has been removed from their hand <summary>
        public static void SendSelfRemoveCardMessage(TurnPlayer id, int position)
        {
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.selfLoseCard);
            m.Add(position);
            NetworkManagerV2.Instance.server.Send(m, (ushort)id);
        }
        /// <summary> informs the client a card has been removed from their hand <summary>
        public static void SendOpponentRemoveCardMessage(TurnPlayer opponentId, int position)
        {
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.opponentLoseCard);
            m.Add(position);
            NetworkManagerV2.Instance.server.Send(m, (ushort)opponentId);
        }
    #endregion

    #region VisualizationFunctions
    public void Display(){ // PURELY FOR VISUALIZATION PURPOSES WHILE WORKING ON SERVER
        int i =0;
        foreach (Transform c in this.transform){
            c.transform.position = transform.position +new Vector3(1.5f*i++, 0 ,0);
        }
    }

    public void Update(){
        Display(); // We can keep visualizing whats happening will remove later
    }
    #endregion

}
