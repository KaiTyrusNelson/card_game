/// <summary> THIS CONDITION COUNTS THE AMOUNT OF CHARACTERS WHICH MEET A CERTAIN CONDITION<summary>
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class CountCondition : Condition
{
    // THE CONDITIIONS THAT SHOULD BE MET
    [SerializeField] TargettingCondition [] conditions;
    // THE OPERATOR WE WANT TO DO
    [SerializeField] Operators o;
    [SerializeField] int number;

    #region locations
    [SerializeField ]bool allyBoard;
    [SerializeField ]bool enemyBoard;
    [SerializeField ]bool allyHand;
    [SerializeField ]bool enemyHand;

    [SerializeField]bool enemyGraveyard;
    [SerializeField]bool allyGraveyard;
    #endregion


    public void AddSelections(SelectionList Selections)
    {
        if (allyBoard){
            Selections.AddBoard(AssociatedCard.Player);
        }
        if (enemyBoard){
            Selections.AddBoard(AssociatedCard.Player.OppositePlayer());
        }
        if (allyHand){
            Selections.AddHand(AssociatedCard.Player);
        }
        if (enemyHand){
            Selections.AddHand(AssociatedCard.Player.OppositePlayer());
        }
        if (allyGraveyard){
            Selections.AddGraveyard(AssociatedCard.Player);
        }
        if (enemyGraveyard){
            Selections.AddGraveyard(AssociatedCard.Player.OppositePlayer());
        }
    }
    public override bool Check()
    {
        // CREATES THE SELECTION LIST
        SelectionList canidates = new SelectionList();
        // ADDS THE CONDITIONS WHICH MUST BE MET
        foreach (TargettingCondition c in conditions)
        {
            canidates.AddCondition(c);
        }

        AddSelections(canidates);

        // COUNTS UP THE CANIDATES
        int count = canidates.Count();
        if (allyGraveyard){
            Debug.Log("GRAVEYARD COUNT SERAFALL");
        }
        switch(o)
        {
            case(Operators.Equal):
            {
                if (count == number)
                {
                    return true;
                }
                break;
            }
            case(Operators.GreaterThan):
            {
                if (count > number)
                {
                    return true;
                }
                break;
            }
            case(Operators.LessThan):
            {
                if (count < number)
                {
                    return true;
                }
                break;
            }
        }
        return false;
    }
}

public enum Operators{
    Equal,
    GreaterThan,
    LessThan
}