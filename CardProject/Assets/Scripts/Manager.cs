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
    
    [SerializeField] Deck playerOneDeck;
    [SerializeField] Deck playerTwoDeck;
    public Dictionary<TurnPlayer, Deck> PlayerDecks;
    // HAND OBJECTS CONTAINING INFORMATION ON THE HANDS OF BOTH PLAYERS
    [SerializeField] Hand player1Hand;
    [SerializeField] Hand player2Hand;
    public Dictionary<TurnPlayer, Hand> PlayerHands;
    // BOARD INFORMATION
    [SerializeField] GameBoard player1Board;
    [SerializeField] GameBoard player2Board;
    public Dictionary<TurnPlayer, GameBoard> PlayerBoards;
    // PLAYER OBJECTS (THESE ARE IMPORTANT FOR NETWORKING)
    [SerializeField] Player player1;
    [SerializeField] Player player2;
    public Dictionary<TurnPlayer, Player> Players;

    public Dictionary<TurnPlayer, ushort> maxMana;
    public Dictionary<TurnPlayer, ushort> currentMana;

    
    #endregion
    ///<summary> This contains the basic functions for incrementing playerturns and definitions<summary>
    #region PlayerTurnFunctions
    TurnPlayer currentPlayer = TurnPlayer.Player1;
    public void ChangeTurn(){
        currentPlayer = OppositePlayer(currentPlayer);
    }
    // RETURNS THE OPPOSITE PLAYER
    public TurnPlayer OppositePlayer(TurnPlayer p)
    {
        switch(p){
            case(TurnPlayer.Player1):
                return TurnPlayer.Player2;
            case(TurnPlayer.Player2):
                return TurnPlayer.Player1;
        }
        return TurnPlayer.Player1;
    }
    public void BeginTurn(TurnPlayer player)
    {
        maxMana[player] += 2;
        currentMana[player] = maxMana[player];
        PlayerDecks[player].DrawCard();
    }
    #endregion
    ///<summary> This contains information on the event stack<summary>
    /*
    */
    #region EventStackFunctions
    Stack<StackEvent> eventStack;
    #endregion

    public void Awake(){
        Singleton = this;
        Players = new Dictionary<TurnPlayer, Player>();
        Players[TurnPlayer.Player1] = player1;
        Players[TurnPlayer.Player2]=player2;

        PlayerHands = new Dictionary<TurnPlayer, Hand>();
        PlayerHands[TurnPlayer.Player1] = player1Hand;
        PlayerHands[TurnPlayer.Player2]=player2Hand;

        PlayerBoards = new Dictionary<TurnPlayer, GameBoard>();
        PlayerBoards[TurnPlayer.Player1] = player1Board;
        PlayerBoards[TurnPlayer.Player2] = player2Board;

        maxMana = new Dictionary<TurnPlayer, ushort>();
        maxMana[TurnPlayer.Player1] = 0;
        maxMana[TurnPlayer.Player2] = 0;
        currentMana = new Dictionary<TurnPlayer, ushort>();
        currentMana[TurnPlayer.Player1] = 0;
        currentMana[TurnPlayer.Player2] = 0;

        PlayerDecks = new Dictionary<TurnPlayer, Deck>();
        PlayerDecks[TurnPlayer.Player1] = playerOneDeck;
        PlayerDecks[TurnPlayer.Player2] = playerTwoDeck;

        eventStack = new Stack<StackEvent>();
    }

    public void Start()
    {
        // START OF GAME
        playerOneDeck.BeginGame();
        playerTwoDeck.BeginGame(); // INTIALIZES THE CARDS IN THE DECKS
        for (int i =0; i < 10 ; i++){
            playerOneDeck.DrawCard();
            playerTwoDeck.DrawCard();
        }
        StartCoroutine(GameLoop());
    }


    [SerializeField] PromptResponse BasicPromptResponseObject;
    IEnumerator GameLoop(){
        while(true){
            BeginTurn(currentPlayer);
            while(true){
                ///<summary> THE FIRST THING WE DO IS BEGIN TO POP OPEN THE STACK<summary>
                while(eventStack.Count > 0)
                {
                    // STARTS THE TOP STACKS ROUTINE
                    yield return StartCoroutine(eventStack.Pop().Activate());
                }
                ///<summary> NORMAL ACTIONS (SUMMONING AND ENDING TURNS)<summary>
                yield return null;


                // TO DO, CHANGE SO AFTER ONE OF THESE NORMAL EVENTS ARE CALLED, IT WILL JUMP IMMEDIATLEY TO STACK RESOLUTION
                if( Players[currentPlayer].SummonCall() == true)
                {
                    Debug.Log("summon call triggered");
                    // If the other player is capable of chaining in the current moment, allow them;
                    if (ExistsChainable.Check(OppositePlayer(currentPlayer)))
                    {

                        // MAKE FUNCTION FOR PUSHING EVENT STACK OBJECTS AND REMOVING, PARENT THEM, DELETE THEM ECT.
                        PromptResponse pushObject = Instantiate(BasicPromptResponseObject, transform.position, transform.rotation);
                        pushObject.Player = OppositePlayer(currentPlayer);
                        eventStack.Push(pushObject);
                    }
                }
                if(Players[currentPlayer].EndCall())
                {
                    Debug.Log("end turn call triggered");
                    break;
                }
            }
            ChangeTurn();
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
        if (PlayerHands[player].GetCount() <= hand_location || hand_location < 0){
            Debug.Log("Cannot place card doesnt exist.");
            return false;
        }
        // PLACE HAS TO BE OPEN
        if (PlayerBoards[player].GetAt(x, y) != null)
        {
            Debug.Log("Location is occupied.");
            return false;
        }
        // SUMMONING CANNOT BE DONE VIA CHAIN
        if(eventStack.Count != 0){
            Debug.Log("Cannot perform a summon during a chain.");
            return false;
        }
        // Player MUST HAVE ENOUGH MANA
        if (currentMana[player] < PlayerHands[player].GetCard(hand_location).ManaCost)
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
        currentMana[player] -= PlayerHands[player].GetCard(hand_location).ManaCost;
        PlayerHands[player].PlayCardFromPosition(hand_location, x, y);
        return true;
    }
    


    #region DebugTools
    // EXISTS SOLELY FOR VISUALIZATION DURING DEBUGGING

    [SerializeField]
    TMP_Text playerOneManaField;

    [SerializeField]
    TMP_Text playerTwoManaField;

    public void Update(){
        playerOneManaField.SetText($"{currentMana[TurnPlayer.Player1]} / {maxMana[TurnPlayer.Player1]}");
        playerTwoManaField.SetText($"{currentMana[TurnPlayer.Player2]} / {maxMana[TurnPlayer.Player2]}");
    }
    #endregion
}


