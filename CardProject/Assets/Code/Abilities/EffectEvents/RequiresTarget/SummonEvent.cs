using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SummonEvent : CharacterStackEvent
{
    public override IEnumerator Activate()
    {
        // REMOVES THE CARD FROM THE CURRENT LOCATION
        Char.RemoveFromCurrentLocation();
        // NOW WE NEED TO BE ABLE TO PLACE THE CARD ON A NEW LOCATION
        List<Tuple> locations = Manager.Players[AssociatedPlayer].PlayerBoard.GetEmptySpaces();

        if (locations.Count == 0)
        {
            Debug.Log("There are no empty spaces");
            yield break;
        }

        locations.SendLocationArgs(AssociatedPlayer);

        while(true){
            yield return null;
            Tuple locationCall = Manager.Players[AssociatedPlayer].LocationSelectionCall();
            if (locationCall != null)
            {
                Debug.Log("Triggered");
                if (locations.Find(m => (m.x == locationCall.x) && (m.y == locationCall.y) ) != null)
                {
                    // THEN THE CALL IS GOOD
                    Debug.Log($"Suceeded Location: {locationCall.x} {locationCall.y}");
                        yield return Manager.Players[AssociatedPlayer].PlayerBoard.SetAt(Char, locationCall.x, locationCall.y);
                    break;
                }else{
                    Debug.Log($"Failed Location: {locationCall.x} {locationCall.y}");
                }
            }
        }
        
        yield break;
    }
}