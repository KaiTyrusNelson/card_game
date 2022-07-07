using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
public abstract class CardContainer : MonoBehaviour
{
    public List<Character> Cards;
    public static CardLocations location;

    public virtual bool RemoveCard(Character c)
    {
        return Cards.Remove(c);
    }
    public int GetCount(){
        return Cards.Count;
    }
    public Character GetCard(int loc){
        if (loc >= Cards.Count || loc<0)
            return null;
        return Cards[loc];
    }

    public virtual void AddCard(Character c)
    {
        Cards.Add(c);
        c.transform.SetParent(this.transform);
        c.Location = location;
    }

    public virtual void RemoveCard(int position)
    {
        Cards.RemoveAt(position);
    }
}