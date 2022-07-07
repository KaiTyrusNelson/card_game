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

    public static bool selectionDone = false;
    
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
        Message m = System.ObjectExtensions.Copy(message);
        AnimationManager.Singleton.ADD_ANIMATION(HandleSelectionRequest(m));
    }

    [MessageHandler((ushort) ServerToClient.confirmSelectionEnd)]
    public static void selectionEnd(Message message)
    {
        Debug.Log("SelectionEndMessage has been received");
        selectionDone = true;
         AnimationManager.Singleton.ADD_ANIMATION(NextSelectionRequestPrep());
    }
    public static IEnumerator NextSelectionRequestPrep()
    {
        selectionDone = false;
        yield break;
    }
    public static IEnumerator HandleSelectionRequest(Message message)
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

        // HOLDS THIS UNTIL THE SELECTION HAS BEEN MADE
        while (!selectionDone)
        {
            yield return null;
        }
        Singleton.SelectionPanel.SetActive(false);
        yield break;
    }
    


}
