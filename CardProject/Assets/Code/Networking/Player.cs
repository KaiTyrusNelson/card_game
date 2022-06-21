using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Player : MonoBehaviour
{


    //TODO: CONVERT THIS TO TURNPLAYER TYPE ENUM
    [SerializeField]TurnPlayer _playerId;
    public TurnPlayer PlayerId {get => _playerId;}

    /// <summary> Contains all properties of the players, their gamespaces, ect. <summary>
    #region Definitions
    [SerializeField] public GameBoard PlayerBoard;
    [SerializeField] public Hand PlayerHand;
    [SerializeField] public Graveyard PlayerGraveyard;
    [SerializeField] public Deck PlayerDeck;
    [SerializeField] public int CurrentMana;
    [SerializeField] public int MaxMana;

    #endregion

    /// <summary> Basic functions controlling the base flow of the game <summary>
    #region GameFlowFunctions

    public void DrawCard(){
        Character c = PlayerDeck.Cards[PlayerDeck.Cards.Count-1];
        if( !PlayerHand.IsFullHand() ){
            PlayerHand.AddCard(c);            
            PlayerDeck.Cards.RemoveAt(PlayerDeck.Cards.Count-1);
        }
    }

    public void CleanUpBoard(){
        for (int i =0; i < 2; i++)
        {
            for (int j=0; j < 3; j++)
            {
                if (PlayerBoard.GetAt(i,j)!=null){
                if (PlayerBoard.GetAt(i, j).Hp <=0){
                    PlayerBoard.SendToGYAt(i,j);
                }
                }
            }
        }
    }
    // TODO: LOOK INTO CREATING AN ITERATOR FOR THIS, (IENUMERABLE)
    public void RefreshAttacks()
    {
        for (int i =0; i < 2; i++)
        {
            for (int j=0; j < 3; j++)
            {
                if (PlayerBoard.GetAt(i,j)!=null){
                if (PlayerBoard.GetAt(i, j).Hp <=0){
                    //TODO: REFRESH ATTACKS
                }
                }
            }
        }
    }
    #endregion
    /// <summary> This region is responsible for processing messages between the clients and the server regarding the player object <summary>
    #region Messages
    // stores the information and awaits results
    ushort mostRecent = 0;
    int hand_location;
    int board_x;
    int board_y;
    int selection;
    public int SelectionCall( int len ){
        // IF THE SELECTION CALL HASNT BEEN CALLED
        if (mostRecent != (ushort)ClientToServer.selectionCall){
            return -1;
        }
        Debug.Log("Called the selection call method.");

        int sel = selection;
        wipeInfo();
        // IF SELECTION IS PAST THE END OF THE SELECTION RANGE
        if (sel >= len || sel < 0)
        {
            Debug.Log("Invalid choice selected");
            return -1;
        }
        // ELSE RETURN THE SELECTION
        return sel;
    }

    public bool SummonCall(){
        if (mostRecent != (ushort)ClientToServer.summonMessage)
        {
            return false;
        }
        Debug.Log("Called summon method");
        // THIS WILL MAKE SURE THE CALL THAT WAS PERFORMED WAS VALID OR NOT
        bool done = Manager.Singleton.Summon(PlayerId, hand_location, board_x, board_y);
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

    public Tuple AttackCall(){
        if (mostRecent != (ushort)ClientToServer.attack)
        {
            return null;
        }
        Debug.Log("Called Attack Message");
        Tuple newTuple = new Tuple();
        newTuple.x = board_x;
        newTuple.y = board_y;
        wipeInfo();
        return newTuple;
    }



    void wipeInfo(){
        mostRecent = 0;
    }

    // ATTACK WITH CARD
    [MessageHandler((ushort)ClientToServer.attack)]
    private static void attack(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED ATTACK MESSAGE FROM CLIENT");
        Manager.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.attack;
        Manager.Players[(TurnPlayer)fromClientId].board_x = message.GetInt();
        Manager.Players[(TurnPlayer)fromClientId].board_y = message.GetInt();
    }

    // SUMMON CARD FROM HAND
    [MessageHandler((ushort)ClientToServer.summonMessage)]
    private static void summonMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED SUMMON MESSAGE FROM CLIENT");
        Manager.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.summonMessage;
        Manager.Players[(TurnPlayer)fromClientId].hand_location = message.GetInt();
        Manager.Players[(TurnPlayer)fromClientId].board_x = message.GetInt();
        Manager.Players[(TurnPlayer)fromClientId].board_y = message.GetInt();
    }

    // END MESSAGES CAN END TURNS
    [MessageHandler((ushort)ClientToServer.end)]
    private static void endTurnMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED END TURN MESSAGE FROM CLIENT");
        Manager.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.end;
    }

    // END MESSAGES CHAIN RESPONSE
    [MessageHandler((ushort)ClientToServer.dontChain)]
    private static void dontChainMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED DON'T CHAIN MESSAGE FROM CLIENT");
        Manager.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.dontChain;
    }

    // CHAIN CONFIRMATION MESSAGE
    [MessageHandler((ushort)ClientToServer.chain)]
    private static void chainMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED CHAIN MESSAGE FROM CLIENT");
        Manager.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.chain;
    }

    [MessageHandler((ushort)ClientToServer.selectionCall)]
    private static void selectionCallMessage(ushort fromClientId, Message message)
    {
        Debug.Log("RECEIVED SELECTION CALL MESSAGE FROM CLIENT");
        Manager.Players[(TurnPlayer)fromClientId].mostRecent = (ushort) ClientToServer.selectionCall;
        Manager.Players[(TurnPlayer)fromClientId].selection = message.GetInt();
    }


    #endregion
}
