using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSingle : Ability
{

    /// <Summary> This ability selects a target, and then applies the following effect to the target
    [SerializeField] Effect[] _efct;
    public Effect[] effects{get => _efct; set {_efct = value;}}

    public override bool CheckSelfCondition(){
        TurnPlayer[] Players = {TurnPlayer.Player1, TurnPlayer.Player2};
        // AS LONG AS THERE IS A CHARACTER ON THE BOARD
        foreach (TurnPlayer player in Players)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j =0; j<3; j++){
                    Character checkCharacter = Manager.Players[player].PlayerBoard.GetAt(i, j);
                    if (checkCharacter != null )
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public override IEnumerator Active(){
        List<Character> targets = new List<Character>();

        TurnPlayer[] Players = {TurnPlayer.Player1, TurnPlayer.Player2};
        // AS LONG AS THERE IS A CHARACTER ON THE BOARD
        foreach (TurnPlayer player in Players)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j =0; j<3; j++){
                    if (Manager.Players[player].PlayerBoard.GetAt(i, j) != null )
                    {
                        targets.Add(Manager.Players[player].PlayerBoard.GetAt(i, j));
                    }
                }
            }
        }

        int select;
        Debug.Log($"Asking Player who they would like to target {targets}");
        while(true){
            yield return null;
            select = Manager.Players[AssociatedCard.Player].SelectionCall(targets.Count);
            if (select != -1)
            {
                // APPLIES EFFECT TO DESIRED TARGETS
                foreach (Effect e in effects)
                {
                    e.Activation(targets[select]);
                }
                break;
            }
        }


        yield return null;
    }  
}