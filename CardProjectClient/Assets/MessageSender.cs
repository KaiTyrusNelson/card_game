using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;
using System;



public class MessageSender : MonoBehaviour
{
    #region summoningCommands
    [SerializeField] TMP_InputField _inputSummon1;
    [SerializeField] TMP_InputField _inputSummon2;
    [SerializeField] TMP_InputField _inputSummon3;
    [SerializeField] TMP_InputField _selectionMessage;

    [SerializeField] TMP_InputField _attackMessage1;
    [SerializeField] TMP_InputField _attackMessage2;

    static TMP_InputField attackMessage1;
    static TMP_InputField attackMessage2;
    
    static TMP_InputField selectionMessage;
    static TMP_InputField inputSummon1;
    static TMP_InputField inputSummon2;
    static TMP_InputField inputSummon3;

    public static void summonMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.summonMessage);
        m.Add(Int32.Parse(inputSummon1.text.ToString()));
        m.Add(Int32.Parse(inputSummon2.text.ToString()));
        m.Add(Int32.Parse(inputSummon3.text.ToString()));
        NetworkManager.Singleton.Client.Send(m);
    }
    #endregion

    public void Awake(){
        inputSummon1 = _inputSummon1;
        inputSummon2 = _inputSummon2;
        inputSummon3 = _inputSummon3;
        selectionMessage = _selectionMessage;
        attackMessage1 = _attackMessage1;
        attackMessage2 = _attackMessage2;
    }


    public static void ChangeTurnMessge()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.end);
        NetworkManager.Singleton.Client.Send(m);
    }
    
    public static void DontChainMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.dontChain);
        NetworkManager.Singleton.Client.Send(m);
    }

    public static void ChainMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.chain);
        NetworkManager.Singleton.Client.Send(m);
    }

    public static void SelectionMessage(){
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.selectionCall);
        m.Add(Int32.Parse(selectionMessage.text.ToString()));
        NetworkManager.Singleton.Client.Send(m);
    }

    public static void AttackMessage(){
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.attack);
        m.Add(Int32.Parse(attackMessage1.text.ToString()));
        m.Add(Int32.Parse(attackMessage2.text.ToString()));
        NetworkManager.Singleton.Client.Send(m);
    }
}
