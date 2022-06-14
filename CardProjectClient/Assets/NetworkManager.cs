using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;

public enum ClientToServer{
    summonMessage=1,
    end,
    chain,
    dontChain
};


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
