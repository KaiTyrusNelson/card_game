using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Hand : MonoBehaviour
{
    [SerializeField] Card card;
    public List<Card> cardsInHand;

    public static Hand Singleton;
    public void Awake(){
        Singleton = this;
    }
    public void HandleDrawCard(string id, ushort hp, ushort attack, ushort manaCost)
    {
        // READS ALL OF THE CARD DATA AND POPULATES THE NEW OBJECT
        Card newCard = Instantiate(card);
        newCard.Id = id;
        newCard.Hp = hp;
        newCard.Attack = attack;
        newCard.ManaCost = manaCost;
        newCard.LocationInHand = cardsInHand.Count;
        newCard.transform.SetParent(this.transform);
        newCard.Location = Placement.hand;
        cardsInHand.Add(newCard);
    }

    public void RemoveCard(int location)
    {
        // REMOVES CARD FROM HAND
        Transform cardTransform = cardsInHand[location].transform;
        cardsInHand.RemoveAt(location);
        Destroy(cardTransform.gameObject);
        // RELABELS THE HAND IN ORDER
        RedrawHand();
    }
    
    public void RedrawHand()
    {
        for(int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetSiblingIndex(i);
            cardsInHand[i].LocationInHand = i;
        }
    }

    #region Messages
    [MessageHandler((ushort) ServerToClient.selfDrawCard)]
    public static void DrawCardMessage(Message message)
    {
        string id = message.GetString();
        ushort hp = message.GetUShort();
        ushort attack = message.GetUShort();
        ushort manaCost = message.GetUShort();
        Singleton.HandleDrawCard(id, hp, attack, manaCost);
    }

    [MessageHandler((ushort) ServerToClient.selfLoseCard)]
    public static void RemoveCardMessage(Message message)
    {
        int loseCard = message.GetInt();
        Singleton.RemoveCard(loseCard);
    }
    #endregion
}
