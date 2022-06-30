/// <summary> THE CONDITION CLASS IS TO CHECK IF AN ABILITY CAN BE ACTIVATED<summary>
using UnityEngine;
public abstract class TargettingCondition : MonoBehaviour
{
    public abstract bool Check(Character c);
}