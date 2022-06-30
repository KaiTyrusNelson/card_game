/// <summary> This effect will summon a character to the board, as this event is a summon, it is evenless, and will summon the target character<summary>
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SummonCharacterToBoard : Effect
{
    public override IEnumerator Activation(Character c)
    {
        // REMOVES THE CARD FROM THE CURRENT LOCATION
        c.RemoveFromCurrentLocation();
        // NOW WE NEED TO BE ABLE TO PLACE THE CARD ON A NEW LOCATION
        List<Tuple> locations = Manager.Players[AssociatedCard.Player].PlayerBoard.GetEmptySpaces();

        if (locations.Count == 0)
        {
            Debug.Log("There are no empty spaces");
            yield break;
        }

        locations.SendLocationArgs(AssociatedCard.Player);

        while(true){
            yield return null;
            Tuple locationCall = Manager.Players[AssociatedCard.Player].LocationSelectionCall();
            if (locationCall != null)
            {
                Debug.Log("Triggered");
                if (locations.Find(m => (m.x == locationCall.x) && (m.y == locationCall.y) ) != null)
                {
                    // THEN THE CALL IS GOOD
                    Debug.Log($"Suceeded Location: {locationCall.x} {locationCall.y}");
                        yield return Manager.Players[AssociatedCard.Player].PlayerBoard.SetAt(c, locationCall.x, locationCall.y);
                    break;
                }else{
                    Debug.Log($"Failed Location: {locationCall.x} {locationCall.y}");
                }
            }
        }
        
        yield break;
    }
}

