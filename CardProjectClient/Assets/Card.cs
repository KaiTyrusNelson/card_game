using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public enum Placement
{
    hand = 1,
    board,
}
public class Card : MonoBehaviour
{
    [SerializeField] public ushort Hp;
    [SerializeField] public ushort Attack;
    [SerializeField] public ushort ManaCost;
    [SerializeField] public Placement Location;

    [SerializeField] string _id;
     public string Id {
        get => _id;
        set{
            try{
                Sprite sprite = Resources.Load <Sprite>($"CardImages/{value}");
                target.sprite = sprite;
            }catch (Exception e){
                Debug.Log($"Couldn't load sprite {e}");
            }
            _id = value;
        }
        }
    [SerializeField] Image target;
}
