using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocationSender : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int x;
    [SerializeField] int y;
    public void OnPointerClick(PointerEventData eventData)
    {
        LocationSenderMain.Singleton.TrySendMessage(x, y);
    }

}
