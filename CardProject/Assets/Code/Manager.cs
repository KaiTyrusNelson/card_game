using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TurnPlayer{
    Player1 = 1,
    Player2
}
public class Manager : MonoBehaviour
{
    /*
        This is the manager class, this runs the actual game loop, including event stack, gameboard order, keeps track of mana
        and references the other containers methods to control gameflow.
    */
    ///<summary> Contains the singleton code, as we do not want to create multiple game managers, we only want one<summary>
    #region SingletonCode
    
    public static Manager _singleton{get; private set;}
    public static Manager Singleton {
        get => _singleton;
        private set
        {
            if(_singleton == null){
                _singleton = value;
            }else{
                Destroy(value);
            }
        }
    }
    #endregion
    ///<summary> The contains basic information regarding the game containers. ie BOARD, HAND, DECK ect. They are refered to via dictionaries<summary>
    #region GameDefinitions
    [SerializeField] Player player1;
    [SerializeField] Player player2;
    public static Dictionary<TurnPlayer, Player> Players; // THIS CAN BE STATIC AS WE HAVE ONLY ONE GAMEMANAGER
    #endregion
    ///<summary> This contains the basic functions for incrementing playerturns and definitions<summary>

    #region PlayerTurnFunctions
    TurnPlayer currentPlayer = TurnPlayer.Player1;
    public void ChangeTurn(){
        currentPlayer = currentPlayer.OppositePlayer();
    }
    // RETURNS THE OPPOSITE PLAYER
    public void BeginTurn(TurnPlayer player)
    {
        Players[player].MaxMana += 2;
        Players[player].CurrentMana = Players[player].MaxMana;
        Players[player].DrawCard();
    }
    #endregion
    ///<summary> This contains information on the event stack<summary>
    /*
    */
    #region EventStackFunctions
    public Stack<StackEvent> EventStack;

    // TO DO: ADDS AN APPROPRIATE MESSAGE TO THE STACK
    public void StackPush(StackEvent evt)
    {
        // PRECONDITION, EVENT IN INSTANTIATED
        EventStack.Push(evt);
        evt.transform.SetParent(this.transform); // AGGREGATES IT TO THE MANAGER
    }

    public void StackRemove(StackEvent evt)
    {
        Destroy(evt.gameObject); 
    }
    #endregion

    public void Awake(){
        Singleton = this;
        Players = new Dictionary<TurnPlayer, Player>();
        Players[TurnPlayer.Player1] = player1;
        Players[TurnPlayer.Player2]=player2;
        EventStack = new Stack<StackEvent>();
    }
    public void Start(){
        StartCoroutine(GameLoop());
    }

    [SerializeField] ChainRequestObject BasicPromptResponseObject;
    IEnumerator GameLoop(){
        yield return null;
        while(NetworkManagerV2.Instance.server.ClientCount < 2)
        {
            yield return null;
        }

        // START OF GAME
        Players[TurnPlayer.Player1].PlayerDeck.BeginGame();
        Players[TurnPlayer.Player2].PlayerDeck.BeginGame(); // INTIALIZES THE CARDS IN THE DECKS

        for (int i =0; i < 10 ; i++){
            Players[TurnPlayer.Player1].DrawCard();
            Players[TurnPlayer.Player2].DrawCard();
        }

        while(true){
            BeginTurn(currentPlayer);
            while(true){
                ///<summary> THE FIRST THING WE DO IS BEGIN TO POP OPEN THE STACK<summary>
                ///<summary> NORMAL ACTIONS (SUMMONING AND ENDING TURNS)<summary>
                yield return StartCoroutine(ResolveStackEvents());

                // INFORMS THE PLAYERS OF WHO IS CURRENTLY EXPECTED TO PLAY
                if (ResponseMessages.currentActingPlayer != currentPlayer){
                    ResponseMessages.SendActingPlayer(currentPlayer);
                }

                // CHECKS IF THEY WANT TO ATTACK
                Tuple attackMessage = Players[currentPlayer].AttackCall();
                // TRY THE ATTACK OUT
                if (attackMessage !=null)
                {
                    TryAttackPosition(currentPlayer, attackMessage.x, attackMessage.y);
                    if (ExistsChainable.Check(currentPlayer.OppositePlayer()))
                    {
                        // MAKE FUNCTION FOR PUSHING EVENT STACK OBJECTS AND REMOVING, PARENT THEM, DELETE THEM ECT.
                        ChainRequestObject pushObject = Instantiate(BasicPromptResponseObject, transform.position, transform.rotation);
                        pushObject.Player = currentPlayer.OppositePlayer();
                        StackPush(pushObject);
                    }
                    goto end;
                }

                // TO DO, CHANGE SO AFTER ONE OF THESE NORMAL EVENTS ARE CALLED, IT WILL JUMP IMMEDIATLEY TO STACK RESOLUTION
                if( Players[currentPlayer].SummonCall() == true)
                {
                    Debug.Log("summon call triggered");
                    // If the other player is capable of chaining in the current moment, allow them;
                    if (ExistsChainable.Check(currentPlayer.OppositePlayer()))
                    {
                        // MAKE FUNCTION FOR PUSHING EVENT STACK OBJECTS AND REMOVING, PARENT THEM, DELETE THEM ECT.
                        ChainRequestObject pushObject = Instantiate(BasicPromptResponseObject, transform.position, transform.rotation);
                        pushObject.Player = currentPlayer.OppositePlayer();
                        StackPush(pushObject);
                    }
                    goto end;
                }

                end:
                if(Players[currentPlayer].EndCall())
                {
                    Debug.Log("end turn call triggered");
                    break;
                }
                
                
            }
            ChangeTurn();
        }
    }

    public IEnumerator ResolveStackEvents()
    {
        while(EventStack.Count > 0)
        {
            // STARTS THE TOP STACKS ROUTINE
            StackEvent evt = EventStack.Pop();
            yield return StartCoroutine(evt.Activate());
            StackRemove(evt);
            // CLEAN UP THE BOARD
            Players[currentPlayer].CleanUpBoard();
            Players[currentPlayer.OppositePlayer()].CleanUpBoard();
        }
    }

    [SerializeField] AttackEvent attackEvent;
    public void TryAttackPosition(TurnPlayer player, int x, int y)
    {
        // THIS NEEDS TO BE A CHAIN EVENT
        Character attacking = Players[player].PlayerBoard.GetAt(x,y);
        if (attacking!=null){
            AttackEvent evt = Instantiate(attackEvent,transform.position, transform.rotation);
            evt.AttackingCharacter = attacking;
            StackPush(evt);
            // TODO: SEND ATTACK MESSAGE
        }
    }


    // CHECKS IF A CARD IS SUMMONABLE FROM A CURRENT LOCATION
    bool checkSummonable(TurnPlayer player, int hand_location, int x, int y)
    {   
        Debug.Log($"Calling Summon Player: {player} Location: {hand_location} x: {x} y: {y}");
        // LOCATION MUST EXIST
        if (!(0<=x && x<=1 && 0<=y && y<=2)){
            return false;
        }
        // PLACE HAS TO HAVE
        if (Players[player].PlayerHand.GetCount() <= hand_location || hand_location < 0){
            Debug.Log("Cannot place card doesnt exist.");
            return false;
        }
        // PLACE HAS TO BE OPEN
        if (Players[player].PlayerBoard.GetAt(x, y) != null)
        {
            Debug.Log("Location is occupied.");
            return false;
        }
        // SUMMONING CANNOT BE DONE VIA CHAIN
        if(EventStack.Count != 0){
            Debug.Log("Cannot perform a summon during a chain.");
            return false;
        }
        // Player MUST HAVE ENOUGH MANA
        if (Players[player].CurrentMana < Players[player].PlayerHand.GetCard(hand_location).ManaCost)
        {
            Debug.Log("Not enough mana to perform summon.");
            return false;
        }
        return true;
    }

    public bool Summon(TurnPlayer player, int hand_location, int x, int y){
        if (!checkSummonable(player, hand_location, x, y))
        {
            return false;
        }
        Players[player].CurrentMana -= Players[player].PlayerHand.GetCard(hand_location).ManaCost;
        Players[player].PlayerHand.PlayCardFromPosition(hand_location, x, y);
        return true;
    }
    


    #region DebugTools
    // EXISTS SOLELY FOR VISUALIZATION DURING DEBUGGING

    [SerializeField]
    TMP_Text playerOneManaField;

    [SerializeField]
    TMP_Text playerTwoManaField;

    public void Update(){
        playerOneManaField.SetText($"{Players[TurnPlayer.Player1].CurrentMana} / {Players[TurnPlayer.Player1].MaxMana}");
        playerTwoManaField.SetText($"{Players[TurnPlayer.Player2].CurrentMana} / {Players[TurnPlayer.Player2].MaxMana}");
    }
    #endregion
}


