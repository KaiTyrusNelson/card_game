using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public static class FUNCTIONS{
    public static TurnPlayer OppositePlayer(this TurnPlayer p)
    {
        switch(p){
            case(TurnPlayer.Player1):
                return TurnPlayer.Player2;
            case(TurnPlayer.Player2):
                return TurnPlayer.Player1;
        }
        return TurnPlayer.Player1;
    }

    public static void SendLocationArgs(this List<Tuple> args, TurnPlayer player)
    {
        // CREATES THE LOCATION SELECTION REQUEST
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.locationSelectionRequest);
        // ADDS THE ARGUMENTS TO THE LIST
        m.Add(args.Count);
        foreach (Tuple arg in args)
        {
            m.Add(arg.x);
            m.Add(arg.y);
        }
        NetworkManagerV2.Instance.server.Send(m, (ushort)player);
    }
}
public class Tuple
{
    public int x, y;
}

/// <summary> Messages informing the players when the other player is responding to something<summary>
public static class ResponseMessages
{
    public static TurnPlayer currentActingPlayer = TurnPlayer.Player1;
    public static void SendActingPlayer(TurnPlayer id)
    {
        currentActingPlayer = id;
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.sendActingPlayer);
        Message m2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.sendWaitingPlayer);
        NetworkManagerV2.Instance.server.Send(m, (ushort)id);
        NetworkManagerV2.Instance.server.Send(m2, (ushort)id.OppositePlayer());
        Debug.Log($"ACTING PLAYER {id}");
    }
}
