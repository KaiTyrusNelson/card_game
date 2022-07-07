using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class StartOfEffectReceiver : MonoBehaviour
{
    [SerializeField] Card cardPrefab;
    public static StartOfEffectReceiver Singleton;
    public void Start()
    {
        Singleton = this;
    }


    [MessageHandler((ushort) ServerToClient.effectBegin)]
    public static void EffectBegin(Message message)
    {
        string id = message.GetString();
        AnimationManager.Singleton.ADD_ANIMATION(Singleton.CardActivationAnim(id));
    }

    // WE WILL CHANGE THESE ANIMATION LATER
    [MessageHandler((ushort) ServerToClient.effectResolve)]
    public static void EffectResolve(Message message)
    {
        string id = message.GetString();
        AnimationManager.Singleton.ADD_ANIMATION(Singleton.CardActivationAnim(id));
    }

    [MessageHandler((ushort) ServerToClient.chainBuildMessage)]
    public static void ChainMessageBuild(Message message)   
    {
        Message m = System.ObjectExtensions.Copy(message);
        AnimationManager.Singleton.ADD_ANIMATION(Singleton.ChainStackAnim(m));
    }

    public IEnumerator ChainStackAnim(Message m)
    {
        Debug.Log("Triggered chain stack anim");
        // GETS AN INT
        int size = m.GetInt();
        Card[] cards = new Card[size];
        // FOR EACH OF THE CARDS
        for (int i = 0; i < size; i++)
        {
            Card c = Instantiate(cardPrefab, transform.position + new Vector3(5f*i,0,5f*i), transform.rotation);
            c.transform.SetParent(this.transform);
            c.Id = m.GetString();
            cards[i] = c;
        }
        yield return new WaitForSeconds(2);
        
        // REMOVE ALL THE CARDS
        foreach (Card c in cards){
            Destroy(c.gameObject);
        }
    }

    public IEnumerator CardActivationAnim(string id)
    {
        Debug.Log("CardActivationAnimCalled");
        Card c = Instantiate(cardPrefab, transform.position, transform.rotation);
        c.transform.SetParent(this.transform);
        c.Id = id;
        yield return new WaitForSeconds(2);
        Destroy(c.gameObject);
    }
}
