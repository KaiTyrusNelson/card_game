using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

abstract public class StackEvent : MonoBehaviour
{
    // WE JUMP OUT OF THE GAME LOOP TO INVOKE THE ACTIVATION FUNCTION
    public abstract IEnumerator Activate();
}

/// <summary> Messages informing the players when the other player is responding to something<summary>
public static class ResponseMessages
{
    public static TurnPlayer currentActingPlayer = TurnPlayer.Player1;
    public static void SendActingPlayer(TurnPlayer id)
    {
        currentActingPlayer = id;
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.sendActingPlayer);
        m.Add((ushort)id);
        NetworkManagerV2.Instance.server.SendToAll(m);
        Debug.Log($"ACTING PLAYER {id}");
    }
}
