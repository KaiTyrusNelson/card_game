/// <summary> THE CONDITION CLASS IS TO CHECK IF AN ABILITY CAN BE ACTIVATED<summary>
using UnityEngine;
public abstract class Condition : MonoBehaviour
{
    // THE CARD THIS OBJECT IS BOUND TO
    public Character AssociatedCard;
    public abstract bool Check();
}