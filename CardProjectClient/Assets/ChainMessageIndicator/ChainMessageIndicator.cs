using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class ChainMessageIndicator : MonoBehaviour
{
    

    public static ChainMessageIndicator Singleton;
    void Start()
    {
        Singleton = this;
        this.gameObject.SetActive(false);
    }

    public static void DontChainMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.dontChain);
        NetworkManager.Singleton.Client.Send(m);
        Singleton.gameObject.SetActive(false);
    }

    public static void ChainMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.chain);
        NetworkManager.Singleton.Client.Send(m);
        Singleton.gameObject.SetActive(false);
    }
}
