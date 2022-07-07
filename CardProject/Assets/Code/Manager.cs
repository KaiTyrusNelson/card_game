using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;
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

        foreach (Character c in Players[player].PlayerBoard.ToArray())
        {
            if (c!=null)
            {
                c.Reset();
            }
        }
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

    // OBJECT FOR KEEPING TRACK OF THE EFFECT CHAINS ORDER
    [SerializeField] StartOfEffectEvent StartOfEffectObject;
    public void AddEffectStartToEventStack(string id)
    {
        StartOfEffectEvent pushObject = Instantiate(StartOfEffectObject, transform.position, transform.rotation);
        pushObject.Id = id;
        StackPush(pushObject);
    }

    public void NegateEffect()
    {
        StackEvent t;
        while(EventStack.TryPop(out t) == true)
        {
            if (t is StartOfEffectEvent)
            {
                Debug.Log("Found start of effect");
                Destroy(t.gameObject);
                break;
            }
            Destroy(t.gameObject);
        }
    }
    
    public static void SendStackEventState(){
        // tally up the total events in the event stack
        Debug.Log("Sending stack event state");
        List<string> ids = new List<string>();

        foreach (StackEvent evt in Singleton.EventStack)
        {
            if (evt is StartOfEffectEvent){
                ids.Add(((StartOfEffectEvent)evt).Id);
            }
        }
        // SENDS THE MESSAGE
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.chainBuildMessage);
        m.Add(ids.Count);
        foreach (string str in ids)
        {
            m.Add(str);
        }
        // SENDS THE MESSAGE TO ALL AVALIABLE CLIENTS
        NetworkManagerV2.Instance.server.SendToAll(m);
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
            GameBoard.sendCurrentBoardStateMessage(Players[currentPlayer].PlayerBoard, currentPlayer);
            GameBoard.sendCurrentBoardStateMessage(Players[currentPlayer.OppositePlayer()].PlayerBoard, currentPlayer.OppositePlayer());

            while(true){
                ///<summary> THE FIRST THING WE DO IS BEGIN TO POP OPEN THE STACK<summary>
                ///<summary> NORMAL ACTIONS (SUMMONING AND ENDING TURNS)<summary>
                yield return StartCoroutine(ResolveStackEvents());

                // INFORMS THE PLAYERS OF WHO IS CURRENTLY EXPECTED TO PLAY
                if (ResponseMessages.currentActingPlayer != currentPlayer){
                    ResponseMessages.SendActingPlayer(currentPlayer);
                }

                // CHECKS IF THEY WANT TO ATTACK
                // TODO: MAKE THE ATTACK MESSAGE SCRIPT A BIT SMARTER
                Tuple attackMessage = Players[currentPlayer].AttackCall();
                // TRY THE ATTACK OUT
                if (attackMessage !=null)
                {
                    // IF THE ATTACK CAN GO THOUGH
                    if (TryAttackPosition(currentPlayer, attackMessage.x, attackMessage.y))
                    {
                    // MAKE FUNCTION FOR PUSHING EVENT STACK OBJECTS AND REMOVING, PARENT THEM, DELETE THEM ECT.
                    ChainRequestObject pushObject = Instantiate(BasicPromptResponseObject, transform.position, transform.rotation);
                    pushObject.Player = currentPlayer.OppositePlayer();
                    StackPush(pushObject);
                    goto end;
                    }
                }

                // TO DO, CHANGE SO AFTER ONE OF THESE NORMAL EVENTS ARE CALLED, IT WILL JUMP IMMEDIATLEY TO STACK RESOLUTION
                summonArgs args = Players[currentPlayer].SummonCall();
                if( args != null)
                {
                    Debug.Log("summon call triggered");
                    if (checkSummonable(currentPlayer, args.hand_location, args.x, args.y)){
                        yield return Summon(currentPlayer, args.hand_location, args.x, args.y);
                        // If the other player is capable of chaining in the current moment, allow them;
                        // MAKE FUNCTION FOR PUSHING EVENT STACK OBJECTS AND REMOVING, PARENT THEM, DELETE THEM ECT.
                        ChainRequestObject pushObject = Instantiate(BasicPromptResponseObject, transform.position, transform.rotation);
                        pushObject.Player = currentPlayer.OppositePlayer();
                        StackPush(pushObject);
                        goto end;
                    }
                }

                Tuple abilityBoardArgs = Players[currentPlayer].CastAbilityFromBoardCall();
                if (abilityBoardArgs != null)
                {
                    Character c = Players[currentPlayer].PlayerBoard.GetAt(abilityBoardArgs.x, abilityBoardArgs.y);
                    // IF THE CONDITIONS ARE MET
                    if (c != null)
                    {
                        if (c.BoardAbility != null)
                        {
                            if (c.BoardAbility.CheckConditions())
                            {
                                yield return StartCoroutine(c.BoardAbility.Activate());
                            }
                        }
                    }
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
            GameBoard.sendCurrentBoardStateMessage(Players[currentPlayer].PlayerBoard, currentPlayer);
            GameBoard.sendCurrentBoardStateMessage(Players[currentPlayer.OppositePlayer()].PlayerBoard, currentPlayer.OppositePlayer());
        }
    }

    [SerializeField] AttackEvent attackEvent;
    public bool TryAttackPosition(TurnPlayer player, int x, int y)
    {
        // THIS NEEDS TO BE A CHAIN EVENT
        Character attacking = Players[player].PlayerBoard.GetAt(x,y);
        // IF THE ATTACKING CHARACTER EXISTS
        if (attacking!=null){
            // AND THE ATTACKING CHARACTER HAS ATTACKED
            if (!attacking.HasAttacked){
                AttackEvent evt = Instantiate(attackEvent,transform.position, transform.rotation);
                evt.AttackingCharacter = attacking;
                StackPush(evt);
                SendAttackMessage(player, x, y);
                attacking.HasAttacked = true;

                return true;
            }
        }
        return false;
    }

    public void SendAttackMessage(TurnPlayer player, int x, int y)
    {
        // CREATES A MESSAGE TO SEND TO THE PLAYERS
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.attackDeclareEvent);
        Message m2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.attackDeclareEvent);
        // POPULATES THE MESSAGES
        m.Add(x);
        m.Add(y);
        m2.Add(x);
        m2.Add(y);
        // TELLS THE PLAYERS WHICH BOARD IT CORESPONDS TO
        m.Add((ushort)Board.allyBoard);
        m2.Add((ushort)Board.enemyBoard);
        // SENDS THE MESSAGES
        NetworkManagerV2.Instance.server.Send(m, (ushort)player);
        NetworkManagerV2.Instance.server.Send(m2, (ushort)player.OppositePlayer());
    }
    
    // CHECKS IF A CARD IS SUMMONABLE FROM A CURRENT LOCATION
    public bool checkSummonable(TurnPlayer player, int hand_location, int x, int y)
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
        // Player MUST HAVE ENOUGH MANA
        if (Players[player].CurrentMana < Players[player].PlayerHand.GetCard(hand_location).ManaCost)
        {
            Debug.Log("Not enough mana to perform summon.");
            return false;
        }
        return true;
    }

    public IEnumerator Summon(TurnPlayer player, int hand_location, int x, int y){    
        Players[player].CurrentMana -= Players[player].PlayerHand.GetCard(hand_location).ManaCost;
        yield return Players[player].PlayerHand.PlayCardFromPosition(hand_location, x, y);
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


