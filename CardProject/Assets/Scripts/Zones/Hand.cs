using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    List<Character> cards;

    [SerializeField]
    public int maxSize;

    [SerializeField]
    public GameBoard board;


    public int GetCount(){
        return cards.Count;
    }
    public Character GetCard(int loc){
        if (loc >= cards.Count || loc<0)
            return null;
        return cards[loc];
    }
    public bool IsFullHand(){
        if (cards.Count <maxSize){
            return false;
        }else{
            return true;
        }
    }
    public void AddCard(Character c){
            c.transform.SetParent(this.transform);
            cards.Add(c);
    }

    public void PlayCardFromPosition(int position, int x, int y)
    {
        Character c = cards[position];
        cards.RemoveAt(position);
        board.SetAt(c,x,y);
        c.transform.SetParent(board.transform);
    }   



    #region VisualizationFunctions
    public void Display(){ // PURELY FOR VISUALIZATION PURPOSES WHILE WORKING ON SERVER
        int i =0;
        foreach (Transform c in this.transform){
            c.transform.position = transform.position +new Vector3(1.5f*i++, 0 ,0);
        }
    }

    public void Update(){
        Display(); // We can keep visualizing whats happening will remove later
    }
    #endregion

}
