using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class OpponentIsMovingGUI : MonoBehaviour
{
    public static OpponentIsMovingGUI Singleton;
    public static bool Acting;
    [SerializeField] GameObject indicator;
    public void Start()
    {
        indicator.SetActive(false);
        Singleton = this;
    }



    [MessageHandler((ushort) ServerToClient.sendActingPlayer)]
    public static void ActingMessage(Message m)
    {
        Acting = true;
    }

    [MessageHandler((ushort) ServerToClient.sendWaitingPlayer)]
    public static void WaitingMessage(Message m)
    {
        Acting = false;
        AnimationManager.Singleton.ADD_ANIMATION(WaitForActing());
    }


    public static IEnumerator WaitForActing()
    {
        Singleton.indicator.SetActive(true);
        while(!Acting && (AnimationManager.Singleton.animationOrder.Count == 0)){
            yield return null;
        }
        Singleton.indicator.SetActive(false);
        yield break;
    } 


}
