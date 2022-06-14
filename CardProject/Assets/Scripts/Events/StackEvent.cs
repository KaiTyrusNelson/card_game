using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class StackEvent : MonoBehaviour
{
    // WE JUMP OUT OF THE GAME LOOP TO INVOKE THE ACTIVATION FUNCTION
    public abstract IEnumerator Activate();
}
