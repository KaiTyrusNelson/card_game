using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public class EndOfEffectEvent : StackEvent
{
    // SENDS THE EFFECT RESOLVE MESSAGE
    public string Id;
    public override IEnumerator Activate()
    {
        SendEffectActivationMessage();
        yield break;
    }

    public void SendEffectActivationMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.effectResolve);
        m.Add(Id);
        NetworkManagerV2.Instance.server.SendToAll(m);
    }
}