using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

public enum ClientToServer{
    // MESSAGE FOR WHEN A CLIENT REQUESTS A SUMMON
    summonMessage=1,
    // MESSAGE FOR WHEN A CLIENT REQUESTS TO END THEIR TURN
    end,
    // MESSAGE FOR WHEN A CLIENT WISHES TO CHAIN
    chain,
    // MESSAGE FOR WHEN A CLIENT DOESN'T WISH TO CHAIN
    dontChain,
    // MESSAGE FOR WHEN A CLIENT MAKES A SELECTION FROM A LIST
    selectionCall,
    // ATTACK MESSAGE
    attack
};

public enum ServerToClient{
    // MESSAGE FOR WHEN A CARD IS SUMMONED ONTO THE BOARD
    summonMessage = 1,
    summonMessageOpponent,
    // MESSAGE WHEN YOUR OPPONENT DRAWS A CARD TODO
    opponentDrawCard,
    // MESSAGE WHEN YOU DRAW A CARD TODO
    selfDrawCard,
    // MESSAGE FOR WHEN YOUR OPPONENT PLAYS A CARD FROM THEIR HAND TODO
    opponentLoseCard,
    // MESSAGE FOR WHEN YOU PLAY A CARD FROM YOUR HAND TODO
    selfLoseCard,
    // MESSAGE FOR WHEN WE REQUEST IF A CLIENT WISHES TO CHAIN AN EFFECT TODO
    chainRequest,
    // MESSAGE TO BE SENT TO OTHER CLIENT INFORMING THEY ARE WAITING FOR A RESPONSE FROM THE OTHER PLAYER
    sendActingPlayer,
    // MESSAGE FOR WHEN WE REQUEST THE CLIENT TO MAKE A SELECTION TODO
    selectionRequest,
    // MESSAGE CONFIRMING PLAYER IDENTITY
    playerIdentityPacket,
}

public class NetworkManagerV2 : MonoBehaviour
{
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClients = 2;


    public Server server {get; private set;}

    ///<summary>"Allows only one instance of the network manager to exist within the code"<summary>
    public static NetworkManagerV2 _instance; 
    public static NetworkManagerV2 Instance{
        get => _instance;
        private set
        {
            if(_instance == null){
                _instance = value;
            }else{
                Debug.Log("Cannot spawn more than one network manager");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void FixedUpdate(){
        server.Tick();
    }

    public void Start(){
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        server = new Server();
        server.ClientConnected += NewPlayerConnected;
        server.ClientDisconnected += PlayerLeft;
        server.Start(port, maxClients );
    }

    private void OnApplicationQuit()
    {
        server.Stop();
        server.ClientConnected -= NewPlayerConnected;
        server.ClientDisconnected -= PlayerLeft;
    }

    private void NewPlayerConnected(object sender, ServerClientConnectedEventArgs e)
    {
        Debug.Log("New Player Connected");
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Debug.Log("Player has left");
    }

    
}
