using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RiptideNetworking;

public class SelectionCard : Card, IPointerClickHandler
{
    // DETERMINES WHAT IT IS THE SELECTION QUEUE
    [SerializeField] public ushort SelectionNumber = 0;
    // THE SELECTION MESSAGE WHICH CONTROLS THIS OBJECT
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectionMessage.SelectionBuffer.Contains(SelectionNumber))
        {
            Debug.Log("Removing Selection Number");
            SelectionMessage.SelectionBuffer.Remove(SelectionNumber);
        }else{
            Debug.Log("Adding Selection Number");
            SelectionMessage.SelectionBuffer.Add(SelectionNumber);
        }
        //this.transform.parent.gameObject.SetActive(false);
    }

    CanvasGroup component;
    public void Start()
    {
        component = GetComponent<CanvasGroup>();
    }
    public void Update(){
        if(SelectionMessage.SelectionBuffer.Contains(SelectionNumber))
        {
            component.alpha = 1f;
        }
        else{
            component.alpha = 0.5f;
        }
    }
}
