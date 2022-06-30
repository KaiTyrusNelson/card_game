/// <summary> THE CONDITION CHECKS TO SEE IF THE TARGET HAS A CURRENTLY CHAINABLE BOARD ABILITY<summary>
using UnityEngine;
public class HasHandAbilityChainable:TargettingCondition
{
    public override bool Check(Character c)
    {
        if (c.HandAbility != null)
        {
            if (c.HandAbility.CheckConditions())
                return true;
        }
        return false;
    }
}