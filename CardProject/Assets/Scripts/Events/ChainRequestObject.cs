using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainRequestObject : StackEvent
{
    // TO DO, PROMPT CONDITIONS
    [SerializeField] TurnPlayer _player;
    public TurnPlayer Player {
        get => _player;
        set{
            _player = value;
        }
    }
    public override IEnumerator Activate()
    {
        Debug.Log($"Asking Player {_player} if they would like to respond to the following");
        while(true){
            yield return null;
            // IF AN END MESSAGE IS SENT, THEY DO NOT WISH TO CHAIN
            if(Manager.Singleton.Players[_player].DontChainCall())
            {
                Debug.Log("Player has chosen not to chain");
                break;
            }
            // IF THE CHAIN CALL MESSAGE IS SENT WE WILL ENTER A CHAIN EVENT
            if(Manager.Singleton.Players[_player].ChainCall())
            {
                Debug.Log("Player has chosen to chain");
                break;
            }
        }
    }
}
