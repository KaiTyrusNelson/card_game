using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RiptideNetworking;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int x;
    [SerializeField] int y;
    public void OnPointerEnter(PointerEventData eventData){
    }

    public void OnPointerExit(PointerEventData eventData){
    }
   public void OnDrop(PointerEventData eventData)
   {
        Debug.Log("Drop Event Activated");
        Card d = eventData.pointerDrag.GetComponent<Card>();
        if (d!=null){
          if (d.Location == Placement.hand){
            Debug.Log("Card Found");
            SendPlaceMessage(d, x, y);
          }
        }
   }

   public void SendPlaceMessage(Card c ,int x_to, int y_to)
   {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.summonMessage);
        m.Add(c.LocationInHand);
        m.Add(x_to);
        m.Add(y_to);
        NetworkManager.Singleton.Client.Send(m);
   }
}
