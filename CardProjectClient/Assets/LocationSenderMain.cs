using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class LocationSenderMain : MonoBehaviour
{
    public static LocationSenderMain Singleton;
    [SerializeField] GameObject[] row0Proxies;
    [SerializeField] GameObject[] row1Proxies;

    List<Tuple> validEntries = new List<Tuple>();

    public GameObject GetObjectAt(int x, int y)
    {
        x = x%2;
        if (x==0) return row0Proxies[y];
        return row1Proxies[y];
    } 

    public void TrySendMessage(int x, int y)
    {
        if (validEntries.Find(m => (m.x == x) && (m.y == y) ) != null)
        {
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.clientLocationSelectionMessage);
            m.Add(x);
            m.Add(y);
            NetworkManager.Singleton.Client.Send(m);
            Clear();
        }
    }

    public void Clear(){
        validEntries.Clear();
        foreach (GameObject o in row0Proxies)
        {
            o.SetActive(false);
        }
        foreach (GameObject o in row1Proxies)
        {
            o.SetActive(false);
        }
    }
    public void Start()
    {
        Singleton = this;
        Clear();
    }

    #region MessageReception
    [MessageHandler((ushort) ServerToClient.locationSelectionRequest)]
    public static void RemovalMessage(Message message)
    {
        Debug.Log("Location message received");
        int count = message.GetInt();
        for (int i =0; i < count; i++)
        {
            Tuple newTup = new Tuple();
            newTup.x = message.GetInt();
            newTup.y = message.GetInt();
            Singleton.GetObjectAt(newTup.x,newTup.y).SetActive(true);
            Singleton.validEntries.Add(newTup);
        }
    }
    #endregion
}

public class Tuple
{
    public int x, y;
}