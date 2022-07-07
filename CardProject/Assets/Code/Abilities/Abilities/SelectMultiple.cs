using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SelectMultiple : Ability
{

    /// <Summary> This ability selects a target, and then applies the following effect to the target
    [SerializeField] Effect[] _efct;
    public Effect[] effects{get => _efct; set {_efct = value;}}

    [SerializeField] int minSelections;
    [SerializeField] int maxSelections;
    // GETS UPDATED TO BE THE MINIMUM OF THE MAX SELECTIONS AND THE TOTAL OPTIONS
    int selectMax;

    #region SelectionOptions
    [SerializeField ]bool allyBoard;
    [SerializeField ]bool enemyBoard;
    [SerializeField ]bool allyHand;
    [SerializeField ]bool enemyHand;

    [SerializeField]bool enemyGraveyard;
    [SerializeField]bool allyGraveyard;

    [SerializeField] bool enemyDeck;
    [SerializeField] bool allyDeck;
    #endregion

    SelectionList Selections = new SelectionList();

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
        if (allyDeck){
            Selections.AddDeck(AssociatedCard.Player);
        }
        if (enemyDeck){
            Selections.AddDeck(AssociatedCard.Player.OppositePlayer());
        }
    }

    public override bool CheckSelfCondition(){
        Selections._ownerplayer = AssociatedCard.Player;
        Selections.Clear();
        AddSelections(Selections);

        return Selections.Count() >= minSelections;
    }
    public override IEnumerator Active(){
        // ASSEMBLE THE VIABLE SELECTIONS
        Selections._ownerplayer = AssociatedCard.Player;
        Selections.Clear();
        AddSelections(Selections);

        // DETERMINES HOW MANY SELECTIONS THE PLAYER CAN HAVE
        selectMax = Math.Min(maxSelections, Selections.Count());
        if (Selections.Count() <= 0){
            yield break;
        }
        // SENDS THE VIABLE SELECTIONS TO THAT CHARACTER
        Selections.SendMesssage(minSelections, selectMax);

        HashSet<ushort> selections;

        while(true){
            yield return null;
            // PROBES THE SELECTIONS FROM THE PLAYER
            selections = Manager.Players[AssociatedCard.Player].SelectionCall(Selections.Count(), minSelections, selectMax);
            if (selections != null)
            {
                foreach(ushort x in selections){
                // APPLIES EFFECT TO DESIRED TARGETS
                    foreach (Effect e in effects)
                    {
                        yield return e.Activation(Selections.iterationList[x].c);
                    }
                }
                break;
            }
        }
        
        Selections.SendEndOfSelectionMessage();

        yield return null;
    }  
}