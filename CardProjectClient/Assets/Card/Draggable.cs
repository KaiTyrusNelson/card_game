using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    // FOR OUR DRAGGABLE WE WANT IT TO TRY AND ATTEMPT TO SEND A MESSAGE FOR A SET AMOUNT OF TIME AND THEN IF IT FAILS WE JUST WAIT
    
    public Transform returnTo;
    public void OnBeginDrag(PointerEventData eventData)
    {
        returnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts=false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(returnTo);
        GetComponent<CanvasGroup>().blocksRaycasts=true;
        Hand.Singleton.RedrawHand();
    }
}
