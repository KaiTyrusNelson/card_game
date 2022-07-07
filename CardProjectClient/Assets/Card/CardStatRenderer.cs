using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardStatRenderer : MonoBehaviour
{
    [SerializeField] Card c;
    [SerializeField] TMP_Text atk;
    [SerializeField] TMP_Text def;

    public void OnUpdate()
    {
        atk.SetText($"{c.Attack}");
        def.SetText($"{c.Hp}");
    }

    public void Update()
    {
        OnUpdate();
    }
}
