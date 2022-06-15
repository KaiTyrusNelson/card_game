/// <summary> THIS IS A CONDITION WHICH IS ALWAYS TRUE, SO THIS CAN BE USED ALL THE TIME NO MATTER WHAT <summary>

using UnityEngine;
public class AlwaysTrue : Condition
{
    public override bool Check(){
        Debug.Log("Always true condition has been checked.");
        return true;
    }
}