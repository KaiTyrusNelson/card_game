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
        // IF WE ARE DEALING WITH A HAND CARD
        Debug.Log("Drop Event Activated");
        HandCard d = eventData.pointerDrag.GetComponent<HandCard>();
        if (d!=null){
          if (d.Location == Placement.hand){
            Debug.Log("Card Found");
            SendPlaceMessage(d, x, y);
          }
        }

        // IF WE ARE DEALING WITH A BOARD CARD
        BoardCard b = eventData.pointerDrag.GetComponent<BoardCard>();
        if (b!=null){
          if (b.Location == Placement.board){
            Debug.Log("Card Found");
            SendSwapMessage(b.x_location, b.y_location, x, y);
          }
        }
   }

   public void SendPlaceMessage(HandCard c ,int x_to, int y_to)
   {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.summonMessage);
        m.Add(c.LocationInHand);
        m.Add(x_to);
        m.Add(y_to);
        NetworkManager.Singleton.Client.Send(m);
   }
   public void SendSwapMessage(int x1, int y1, int x2, int y2)
   {
      Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.swapMessage);
      // SENDS THE AVALIABLE SWAP MESSAGES
      Debug.Log("Swap message sent");
      m.Add(x1);
      m.Add(y1);
      m.Add(x2);
      m.Add(y2);
      // SEMDS THE MESSAGE
      NetworkManager.Singleton.Client.Send(m);
   }
}
