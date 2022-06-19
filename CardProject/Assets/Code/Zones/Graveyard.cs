using UnityEngine;
using System.Collections.Generic;


/// <summary> The graveyard is the zone in which the cards go when they die<summary>
public class Graveyard: MonoBehaviour{
    [SerializeField] List<Character> _cards;
    public List<Character> Cards {get=>_cards; set{_cards=value;}}

    public void AddCard(Character c)
    {
        _cards.Add(c);
        c.transform.SetParent(this.transform);
        // if the card is dead it should be set back to max hp
        c.Hp = c.MaxHp;
    }
    

    #region DisplayFunctions
    public void Update()
    {
        Display();
    }
    public void Display() // FOR TESTING PURPOSES
    {
        int i =0;
        foreach (Transform c in this.transform){
            c.transform.position = transform.position +new Vector3(.1f*i++, 0 ,0);
        }
    }
    #endregion
}