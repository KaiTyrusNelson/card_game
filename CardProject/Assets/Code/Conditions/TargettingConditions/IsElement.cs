/// <summary> CHECKS IF A PLAYER HAS AN ATTRIBUTE<summary>
using UnityEngine;
public class IsElement:TargettingCondition
{
    // attribute to search fow
    [SerializeField] characterElements elm;
    // CHECK
    public override bool Check(Character c)
    {
        // IF HAS ATTRIBUTE
        if (c.IsElement(elm))
        {
            // RETURN TRUE
            return true;
        }
        // IF NOT DONT
        return false;
    }
}