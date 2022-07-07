/// <summary> Prompt a chain response, giving a list of responses to make<summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;


public class ChainObject : StackEvent
{
    // TO DO, PROMPT CONDITIONS
    [SerializeField] TurnPlayer _player;
    public TurnPlayer Player
    {

        get => _player;
        set
        {
            _player = value;
        }
    }

    // LIST OF CHAINABLE ABILITIES
    public SelectionList options;

    public override IEnumerator Activate()
    {
        ResponseMessages.SendActingPlayer(_player);
        // final check to make sure there is an effect to be chosen
        if (options.Count() <= 0)
        {
            yield break;
        }

        HashSet<ushort> select;
        Debug.Log($"Asking Player how they would like to respond to the following");
        // SENDS THE REQUEST MESSAGE
        options.SendMesssage(1, 1);
        while (true)
        {
            yield return null;
            // TESTS TO SEE IF THE PLAYER WISHES TO SELECTED A CARD
            select = Manager.Players[_player].SelectionCall(options.Count(), 1, 1);
            if (select != null)
            {
                foreach (ushort x in select)
                {
                    // DETERMINES WHICH ABILITY TO CHAIN DEPENDING ON THE SELECTION
                    switch (options.iterationList[x].c.Location)
                    {
                        case (CardLocations.Board):
                            {
                                options.SendEndOfSelectionMessage();
                                yield return StartCoroutine(options.iterationList[x].c.BoardAbility.Activate());
                                break;
                            }
                        case (CardLocations.Hand):
                            {
                                options.SendEndOfSelectionMessage();
                                yield return StartCoroutine(options.iterationList[x].c.HandAbility.Activate());
                                break;
                            }
                    }

                    break;
                }
                break;
            }
            // ALSO TESTS IF THE PLAYER WISHES TO SUMMON
            summonArgs args = Manager.Players[Player].SummonCall();
            if (args != null)
            {
                Debug.Log("summon call triggered");
                if (Manager.Singleton.checkSummonable(Player, args.hand_location, args.x, args.y))
                {
                    options.SendEndOfSelectionMessage();
                    yield return Manager.Singleton.Summon(Player, args.hand_location, args.x, args.y);
                    
                    break;
                }
            }

            int[] swapArgs = Manager.Players[Player].SwapMessageCall();

            if (swapArgs != null)
            {
                if (Manager.Players[Player].PlayerBoard.CheckSwapable(swapArgs[0],swapArgs[1],swapArgs[2],swapArgs[3]))
                {
                    Debug.Log($"Swap called: {swapArgs[0]} ,{swapArgs[1]} ,{swapArgs[2]},{swapArgs[3]}");
                    options.SendEndOfSelectionMessage();
                    yield return StartCoroutine(Manager.Players[Player].PlayerBoard.SwapPositions(swapArgs[0],swapArgs[1],swapArgs[2],swapArgs[3]));
                    break;
                }
            }
        }
        // AFTER THE LOOP, WE WILL GO AND CONFIRM TO THE CLIENT THAT THE SELECTION WINDOW IS NOW OVER
        

    }



}
