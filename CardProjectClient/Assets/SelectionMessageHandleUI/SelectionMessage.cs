using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class SelectionMessage : MonoBehaviour
{
    [SerializeField] GameObject SelectionPanel;
    public static ushort minSelections;
    public static ushort maxSelections;
    public static HashSet<ushort> SelectionBuffer = new HashSet<ushort>();
    [SerializeField] SelectionCard CardObject;
    // THE OBJECT WHICH WE PLACE THESE ON
    [SerializeField] GameObject Target;
    
    List<SelectionCard> Cards = new List<SelectionCard>();

    public static SelectionMessage Singleton;
    // TODO: IMPLEMENT PROPER SINGLETON CODE
    public void Start()
    {
        Singleton = this;
        SelectionPanel.SetActive(false);
    }


    public void Clear(){
        Cards.Clear();
        foreach (Transform c in Target.transform){
            Destroy(c.gameObject);
        }
    }

    public void AddCard(string id, ushort loc)
    {
        SelectionCard newCard = Instantiate(CardObject, transform.position, transform.rotation);
        newCard.transform.SetParent(Target.transform);
        newCard.Id = id;

        newCard.SelectionNumber=loc;
        Cards.Add(newCard);
    }

    public void SendMessage()
    {
        
        if (SelectionBuffer.Count < minSelections || SelectionBuffer.Count > maxSelections){
            Debug.Log($"Count {SelectionBuffer.Count} min {minSelections} max {maxSelections}");
            return;
        }


        SelectionPanel.SetActive(false);
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.selectionCall);
        m.Add((ushort)SelectionBuffer.Count);
        foreach (ushort x in SelectionBuffer)
        {
            m.Add(x);
        }

        NetworkManager.Singleton.Client.Send(m);
    }

    [MessageHandler((ushort) ServerToClient.selectionRequest)]
    public static void selectionRequest(Message message)
    {
        Singleton.SelectionPanel.SetActive(true);
        Singleton.Clear();
        
        ushort max = message.GetUShort();
        SelectionBuffer.Clear();
        minSelections = message.GetUShort();
        maxSelections = message.GetUShort();
        Debug.Log($"minSelections {minSelections} maxSelections {maxSelections}");
        for (ushort i=0; i < max; i++)
        {
            string id = message.GetString();
            Singleton.AddCard(id, i);
        }
    }
    


}
