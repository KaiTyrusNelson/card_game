/// <summmary> This ability will change the current location of a card <summary>
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class ChangeZoneEvent : CharacterStackEvent
{
    [SerializeField] CardLocations locationTo;

    public override IEnumerator Activate(){
        // REMOVES THE CARD FROM ITS CURRENT LOCATION
        Char.RemoveFromCurrentLocation();
        // MOVES THE CARD TO A NEW LOCATION
        switch(locationTo)
        {
            case(CardLocations.Hand):
            {
                Manager.Players[Char.Player].PlayerHand.AddCard(Char);
                break;
            }
            case(CardLocations.Banished):
            {
                Manager.Players[Char.Player].Banished.AddCard(Char);
                break;
            }
            case(CardLocations.Graveyard):
            {
                Manager.Players[Char.Player].PlayerGraveyard.AddCard(Char);
                break;
            }
            case(CardLocations.Deck):
            {
                Manager.Players[Char.Player].PlayerDeck.AddCard(Char);
                break;
            }
        }
        yield break;
    }
}