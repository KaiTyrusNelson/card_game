using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RiptideNetworking;

public class AttackDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
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
        BoardCard d = eventData.pointerDrag.GetComponent<BoardCard>();
        if (d!=null){
          if (d.Location == Placement.board){
            Debug.Log("Card Found");
            SendAttackMessage(d);
          }
        }
   }

   public void SendAttackMessage(BoardCard c)
   {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.attack);
        m.Add(c.x_location);
        m.Add(c.y_location); 
        NetworkManager.Singleton.Client.Send(m);
   }
}
