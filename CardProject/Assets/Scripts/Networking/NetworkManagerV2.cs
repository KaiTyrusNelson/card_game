using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;



public enum ClientToServer{
    summonMessage=1,
    end,
    chain,
    dontChain
};


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
