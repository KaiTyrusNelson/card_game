using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

// TODO: MOVE TO DEDICATED MANAGER FILE
public enum TurnPlayer{
    Player1 = 1,
    Player2
}

public enum Board
{
    allyBoard = 1,
    enemyBoard
}
public class GameBoard : MonoBehaviour
{
    public static GameBoard AllyBoard;
    public static GameBoard OpponentBoard;
    [SerializeField] Board status;
    public void Start()
    {
        switch(status){
            case(Board.allyBoard):
            {
                AllyBoard = this;
                break;
            }
            case(Board.enemyBoard):
            {
                OpponentBoard = this;
                break;
            }
        }
        NonDraggable = NonDraggableCard;
    }
    [SerializeField] Card NonDraggableCard;
    public static Card NonDraggable;
    [SerializeField] Card[] row0 = new Card[3];
    [SerializeField] Card[] row1 = new Card[3];

    [SerializeField]
    Transform[] row0Proxies;

    [SerializeField]
    Transform[] row1Proxies; 
  
    public void SetAt(Card c, int x, int y){
        x = x%2;
        if (x==0) row0[y] = c;
        if (x==1) row1[y] = c;
        // KEEP THIS DATA UP TO DATE
        c.x_location = x;
        c.y_location = y;
        c.transform.SetParent(GetTransformAt(x,y));
    }
    public Transform GetTransformAt(int x, int y)
    {
        x = x%2;
        if (x==0) return row0Proxies[y];
        return row1Proxies[y];
    }
    #region Messages
    [MessageHandler((ushort) ServerToClient.summonMessage)]
    public static void SummonMessage(Message message)
    {
        Card c = Instantiate(NonDraggable, Vector3.zero, Quaternion.identity);
        c.Hp = message.GetUShort();
        c.Attack = message.GetUShort();
        c.Id = message.GetString();
        c.Location = Placement.board;
        int x_location = message.GetInt();
        int y_location = message.GetInt();
        AllyBoard.SetAt(c, x_location, y_location);
    }
    [MessageHandler((ushort) ServerToClient.summonMessageOpponent)]
    public static void SummonMessageOpponent(Message message)
    {
        Card c = Instantiate(NonDraggable, Vector3.zero, Quaternion.identity);
        c.Hp = message.GetUShort();
        c.Attack = message.GetUShort();
        c.Id = message.GetString();
        c.Location = Placement.board;
        int x_location = message.GetInt();
        int y_location = message.GetInt();
        OpponentBoard.SetAt(c, x_location, y_location);
    }
    #endregion

}
