using UnityEngine;
using System.Collections;
public class AttackEvent : StackEvent
{
    [SerializeField] public Character AttackingCharacter;

    /// <summary> LOCATES THE OPPOSING CHARACTER AT THE TIME OF THE EVENT TRIGGER<summary>
    public Character FindOpponent(Character c){
        
        Tuple location = c.FindLocationOnBoard();

        if (location == null){
            return null;
        }

        int y = location.y;
        // DEFINES THE BOARD WE ARE ATTACKING TO
        GameBoard targetBoard = Manager.Players[c.Player.OppositePlayer()].PlayerBoard;
        // CYCLES THROUGH THE X VALUES TO FIND THE TARGET
        for (int i =0; i < 2; i++)
        {
            if (targetBoard.GetAt(i, y) != null)
            {
                return targetBoard.GetAt(i, y);
            }
        }
        // IF NOT WE WILL RETURN A NULL VALUE
        return null;
    }
    public override IEnumerator Activate()
    {
        Character opponent = FindOpponent(AttackingCharacter);
        // IF THERE IS AN OPPONENT ATTACK IT
        if (opponent != null)
        {
            AttackingCharacter.AttackCharacter(opponent);
            opponent.AttackCharacter(AttackingCharacter);
        }
        yield return null;


    }



}