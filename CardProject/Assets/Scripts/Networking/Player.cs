using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Player : MonoBehaviour
{
    // stores the information and awaits results
    ushort mostRecent = 0;
    int hand_location;
    int board_x;
    int board_y;

    //TODO: CONVERT THIS TO TURNPLAYER TYPE ENUM
    [SerializeField]
    int _playerId;

    TurnPlayer playerId {get => (TurnPlayer)_playerId;}

    public bool SummonCall(){
        if (mostRecent != (ushort)ClientToServer.summonMessage)
        {
            return false;
        }
        Debug.Log("Called summon method");
        // THIS WILL MAKE SURE THE CALL THAT WAS PERFORMED WAS VALID OR NOT
        bool done = Manager.Singleton.Summon(playerId, hand_location, board_x, board_y);
        wipeInfo();
        return done;
    }

    public bool EndCall(){
        if (mostRecent != (ushort)ClientToServer.end)
        {
            return false;
        }
        Debug.Log("Called end turn message");
        wipeInfo();
        return true;
    }

    public bool DontChainCall(){
        if (mostRecent != (ushort)ClientToServer.dontChain)
        {
            return false;
        }
        Debug.Log("Called DontChainMessage message");
        wipeInfo();
        return true;
    }

    public bool ChainCall(){
        if (mostRecent != (ushort)ClientToServer.chain)
        {
            return false;
        }
        Debug.Log("Called ChainMessage message");
        wipeInfo();
        return true;
    }



    void wipeInfo(){
        mostRecent = 0;
    }

    #region Messages
    // SUMMON CARD FROM HAND
    [MessageHandler((ushort)ClientToServer.summonMessage)]
    private static void summonMessage(ushort fromClientId, Message message)
    {

        Debug.Log("RECEIVED SUMMON MESSAGE FROM CLIENT");
        Manager.Singleton.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.summonMessage;
        Manager.Singleton.Players[(TurnPlayer)fromClientId].hand_location = message.GetInt();
        Manager.Singleton.Players[(TurnPlayer)fromClientId].board_x = message.GetInt();
        Manager.Singleton.Players[(TurnPlayer)fromClientId].board_y = message.GetInt();
    }

    // END MESSAGES CAN END TURNS
    [MessageHandler((ushort)ClientToServer.end)]
    private static void endTurnMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED END TURN MESSAGE FROM CLIENT");
        Manager.Singleton.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.end;
    }

    // END MESSAGES CHAIN RESPONSE
    [MessageHandler((ushort)ClientToServer.dontChain)]
    private static void dontChainMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED DON'T CHAIN MESSAGE FROM CLIENT");
        Manager.Singleton.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.dontChain;
    }

    // CHAIN CONFIRMATION MESSAGE
    [MessageHandler((ushort)ClientToServer.chain)]
    private static void chainMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED CHAIN MESSAGE FROM CLIENT");
        Manager.Singleton.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.chain;
    }
    #endregion
}
