using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardRenderer : MonoBehaviour
{
    [SerializeField] Image target;
    [SerializeField] Card card;
    Sprite sprite;

    public void OnUpdate()
    {
        // GETS THE ID AND THEN THE SPRITE
        if (card.Id != null && card.Id != ""){
            sprite = Resources.Load <Sprite>($"CardImages/{card.Id}");
            target.sprite = sprite;
        }
    }

    public void Update()
    {
        OnUpdate();
    }
}
