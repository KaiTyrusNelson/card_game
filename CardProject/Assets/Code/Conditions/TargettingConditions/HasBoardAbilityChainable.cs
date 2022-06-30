/// <summary> THE CONDITION CHECKS TO SEE IF THE TARGET HAS A CURRENTLY CHAINABLE BOARD ABILITY<summary>
using UnityEngine;
public class HasBoardAbilityChainable:TargettingCondition
{
    public override bool Check(Character c)
    {
        if (c.BoardAbility != null)
        {
            if (c.BoardAbility.CheckConditions())
                return true;
        }
        return false;
    }
}