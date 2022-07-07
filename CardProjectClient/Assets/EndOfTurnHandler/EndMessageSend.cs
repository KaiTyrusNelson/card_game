using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class EndMessageSend : MonoBehaviour
{
    public static void ChangeTurnMessge()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.end);
        NetworkManager.Singleton.Client.Send(m);
    }
}
