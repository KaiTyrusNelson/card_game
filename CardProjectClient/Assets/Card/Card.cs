using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Placement
{
    hand = 1,
    board,
}
public class Card : MonoBehaviour
{
    [SerializeField] public string Id;
    [SerializeField] public ushort Hp;
    [SerializeField] public ushort Attack;
    [SerializeField] public ushort ManaCost;
    [SerializeField] public int LocationInHand;

    [SerializeField] public int x_location;
    [SerializeField] public int y_location;

    [SerializeField] public Placement Location;
}
