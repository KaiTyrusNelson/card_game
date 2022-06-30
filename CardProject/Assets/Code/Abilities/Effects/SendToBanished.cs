/// <summary> THIS SENDS THE CARD TO THE BANISHED PILE <summary>
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SendToBanished : Effect
{

    public override IEnumerator Activation(Character c)
    {
        c.RemoveFromCurrentLocation();
        Manager.Players[c.Player].Banished.AddCard(c);
        yield break;
    }
}