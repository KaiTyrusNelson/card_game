using UnityEngine;
using System.Collections.Generic;


/// <summary> The graveyard is the zone in which the cards go when they die<summary>
public class BanishedZone: CardContainer{
    public void Awake()
    {
        location = CardLocations.Banished;
    }
    [SerializeField] public Player Owner; 
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