using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    List<Character> cards;
    [SerializeField]
    Hand hand;
    

    public void DrawCard(){
        Character c = cards[cards.Count-1];

        if( !hand.IsFullHand() ){
            hand.AddCard(c);            
            cards.RemoveAt(cards.Count-1);
        }
    }
    public void BeginGame(){
        cards.Shuffle();
        // ON THE START ALL PREFAB OBJECTS NEED TO REPLACED WITH REAL OBJECTS
        for(int i =0; i < cards.Count; i++){
            cards[i] = Instantiate(cards[i], transform.position, Quaternion.identity);
            cards[i].transform.SetParent(this.transform);
        }
    }

    public void Display() // FOR TESTING PURPOSES
    {
        int i =0;
        foreach (Transform c in this.transform){
            c.transform.position = transform.position +new Vector3(.1f*i++, 0 ,0);
        }
    }

    public void Update(){
        Display(); // We can keep visualizing whats happening will remove later
    }
    
}

static class ExtensionsClass
    {

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0,n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
}