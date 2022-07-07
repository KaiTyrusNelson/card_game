using UnityEngine;
public class HandCard : Card
{
    [SerializeField] public int LocationInHand;
    public void Awake()
    {
        Location = Placement.hand;
    }
}
