/// <summary> CHECKS IF A PLAYER HAS AN ATTRIBUTE<summary>
using UnityEngine;
public class HasAttribute:TargettingCondition
{
    // attribute to search fow
    [SerializeField] characterAttributes attr;
    // CHECK
    public override bool Check(Character c)
    {
        // IF HAS ATTRIBUTE
        if (c.hasAttribute(attr))
        {
            // RETURN TRUE
            return true;
        }
        // IF NOT DONT
        return false;
    }
}