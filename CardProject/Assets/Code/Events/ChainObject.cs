/// <summary> Prompt a chain response, giving a list of responses to make<summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;


public class ChainObject : StackEvent
{
    // TO DO, PROMPT CONDITIONS
    [SerializeField] TurnPlayer _player;
    public TurnPlayer Player {
        
        get => _player;
        set{
            _player = value;
        }
    }

    // LIST OF CHAINABLE ABILITIES
    public SelectionList options;

    public override IEnumerator Activate()
    {
        ResponseMessages.SendActingPlayer(_player);
        // final check to make sure there is an effect to be chosen
        if (options.Count() > 0)
        {
        HashSet<ushort> select;
        Debug.Log($"Asking Player how they would like to respond to the following");
        // SENDS THE REQUEST MESSAGE
        options.SendMesssage(1,1);
        while(true){
            yield return null;
            select = Manager.Players[_player].SelectionCall(options.Count(), 1, 1);
            if (select != null)
            {
                foreach (ushort x in select)
                {
                    // DETERMINES WHICH ABILITY TO CHAIN DEPENDING ON THE SELECTION
                    switch(options.iterationList[x].c.Location)
                    {
                        case(CardLocations.Board):
                        {
                            yield return StartCoroutine(options.iterationList[x].c.BoardAbility.Activate());
                            break;
                        }
                        case(CardLocations.Hand):
                        {
                            yield return StartCoroutine(options.iterationList[x].c.HandAbility.Activate());
                            break;
                        }
                    }
                    
                    break;
                }
                break;
            }
        }
        }
        else{
            // if there is none skip this ability
            yield return null;
        }
    }
}
