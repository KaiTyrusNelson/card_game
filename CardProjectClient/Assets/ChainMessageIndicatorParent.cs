using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class ChainMessageIndicatorParent : MonoBehaviour
{
    [SerializeField] GameObject _indicatorPanel;
    public static GameObject IndicatorPanel;
    public void Start(){
        IndicatorPanel = _indicatorPanel;
    }

    [MessageHandler((ushort) ServerToClient.chainRequest)]
    public static void chainRequest(Message message)
    {
        IndicatorPanel.SetActive(true);
    }
}
