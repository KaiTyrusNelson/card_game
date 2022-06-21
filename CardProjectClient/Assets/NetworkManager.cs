using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;
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


    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _singleton;
        public static NetworkManager Singleton
        {
            get => _singleton;
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else if (_singleton != value)
                {
                    Destroy(value);
                }
            }
        }

        [SerializeField] private string ip;
        [SerializeField] private ushort port;

        public Client Client { get; private set; }

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Client = new Client();
            Client.Connected += DidConnect;
            Client.ConnectionFailed += FailedToConnect;
            Client.ClientDisconnected += PlayerLeft;
            Connect();
        }

        
        public void FixedUpdate(){
        Client.Tick();
        }
        private void OnApplicationQuit()
        {
            Client.Disconnect();
            Client.Connected -= DidConnect;
            Client.ConnectionFailed -= FailedToConnect;
            Client.ClientDisconnected -= PlayerLeft;

        }
     

        public void Connect()
        {
            Client.Connect($"{ip}:{port}");
        }

        private void DidConnect(object sender, EventArgs e)
        {
            Debug.Log("Connected Successfully");
        }

        private void FailedToConnect(object sender, EventArgs e)
        {
            Debug.Log("Failed to connect");
        }

        private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            Debug.Log("A player has left");
        }

    }
