using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public abstract class Effect : MonoBehaviour{
    public Character AssociatedCard;
    public abstract IEnumerator Activation(Character c);
}