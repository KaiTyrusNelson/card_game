using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class ChainRequestObject : StackEvent
{
    // THE FOLLOWING CHAIN OBJECT, WHICH THIS LINKS TO
    [SerializeField] ChainObject _chainObject;
    // TO DO, PROMPT CONDITIONS
    [SerializeField] TurnPlayer _player;
    // SELECTION LIST FOR PROVIDING TARGETS
    SelectionList avaliableList = new SelectionList();
    [SerializeField] HasBoardAbilityChainable chainFromBoardCondition;
    [SerializeField] HasHandAbilityChainable chainFromHandCondition;

    public TurnPlayer Player {
        get => _player;
        set{
            _player = value;
        }
    }
    public override IEnumerator Activate()
    {
        ResponseMessages.SendActingPlayer(Player);

        avaliableList._ownerplayer = _player;
        avaliableList.AddCondition(chainFromBoardCondition);
        avaliableList.AddBoard(_player);

        avaliableList.ClearConditions();
        avaliableList.AddCondition(chainFromHandCondition);
        avaliableList.AddHand(_player);

        if (avaliableList.Count() == 0)
        {
            Debug.Log("No condition found, not chaining");
            yield break;
        }

        
        SendChainMessage();
        Debug.Log($"Asking Player {_player} if they would like to respond to the following");
        while(true){
            yield return null;
            // IF AN END MESSAGE IS SENT, THEY DO NOT WISH TO CHAIN
            if(Manager.Players[_player].DontChainCall())
            {
                Debug.Log("Player has chosen not to chain");
                break;
            }
            // IF THE CHAIN CALL MESSAGE IS SENT WE WILL ENTER A CHAIN EVENT
            if(Manager.Players[_player].ChainCall())
            {
                Debug.Log("Player has chosen to chain");
                // CREATES A CHAIN CALL AWAITING THAT PLAYERS RESPONSE
                ChainObject obj = Instantiate(_chainObject, transform.position, transform.rotation);
                obj.Player = _player;
                obj.options = avaliableList;
                Manager.Singleton.StackPush(obj);
                break;
            }
        }
    }

    #region Messages
    public void SendChainMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.chainRequest);
        NetworkManagerV2.Instance.server.Send(m, (ushort)_player);
    }
    #endregion
}
