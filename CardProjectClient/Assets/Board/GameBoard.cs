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
    }
    [SerializeField] BoardCard NonDraggableCard;
    [SerializeField] BoardCard[] row0 = new BoardCard[3];
    [SerializeField] BoardCard[] row1 = new BoardCard[3];

    [SerializeField]
    Transform[] row0Proxies;

    [SerializeField]
    Transform[] row1Proxies; 

    public void RemoveAt(int x, int y)
    {
        BoardCard c;
        x = x%2;
        if (x==0)
        {
            c = row0[y]; 
            row0[y] = null;   
        }else{
            c =row1[y]; 
            row1[y] = null;
        }
        if (c!=null){
            Destroy(c.gameObject);
        }
    }

    public void Clear()
    {
        for (int i =0; i<2; i++){
            for(int j=0; j<3;j++){
                RemoveAt(i,j);
            }
        }
    }
  
    public void SetAt(BoardCard c, int x, int y){
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
        ushort Hp = message.GetUShort();
        ushort Attack = message.GetUShort();
        string Id = message.GetString();
        int x_location = message.GetInt();
        int y_location = message.GetInt();
        Board boardSelection = (Board)message.GetUShort();

        AnimationManager.Singleton.ADD_ANIMATION(HandleSummonMessage(Hp, Attack, Id, x_location, y_location, boardSelection));
    }

    public static IEnumerator HandleSummonMessage(ushort hp, ushort attack, string id, int x_location, int y_location, Board boardSelection)
    {
        // TODO: DIFFERENT CARDS PER PLAYER
        BoardCard c = Instantiate(AllyBoard.NonDraggableCard, Vector3.zero, Quaternion.identity);
        // ALLOCATE THE STATS
        c.Attack = attack;
        c.Hp = hp;
        c.Id = id;
        c.Location = Placement.board;
        // PLACES THE CHARACTER TO BOARD
        // TODO: ANIMATION
        switch(boardSelection){
            case(Board.allyBoard):
            {
                AllyBoard.SetAt(c, x_location, y_location);
                break;
            }
            case(Board.enemyBoard):
            {
                OpponentBoard.SetAt(c, x_location, y_location);
                break;
            }
        }
        yield return new WaitForSeconds(1);
    }

    [MessageHandler((ushort) ServerToClient.removeMessage)]
    public static void RemovalMessage(Message message)
    {
        int x_location = message.GetInt();
        int y_location = message.GetInt();
        Board boardSelection = (Board)message.GetUShort();

        AnimationManager.Singleton.ADD_ANIMATION(HandleRemovalMessage(x_location, y_location, boardSelection));
    }

    public static IEnumerator HandleRemovalMessage(int x_location, int y_location, Board boardSelection)
    {
        BoardCard c = Instantiate(AllyBoard.NonDraggableCard, Vector3.zero, Quaternion.identity);

        switch(boardSelection){
            case(Board.allyBoard):
            {
                AllyBoard.RemoveAt(x_location, y_location);
                break;
            }
            case(Board.enemyBoard):
            {
                OpponentBoard.RemoveAt(x_location, y_location);
                break;
            }
        }
        yield return new WaitForSeconds(1);
    }

    
    [MessageHandler((ushort) ServerToClient.boardUpdate)]
    public static void BoardUpdateSync(Message message)
    {
        Message m = System.ObjectExtensions.Copy(message);
        AnimationManager.Singleton.ADD_ANIMATION(HandleBoardUpdateSync(m));
    }  
    public static IEnumerator HandleBoardUpdateSync(Message message)
    {
        AllyBoard.Clear();
        for (int i =0; i<2; i++){
            for(int j=0; j<3; j++)
            {
                if (message.GetBool() == true)
                {
                    BoardCard c = Instantiate( AllyBoard.NonDraggableCard, AllyBoard.transform.position, AllyBoard.transform.rotation);
                    c.Id = message.GetString();
                    c.Attack = message.GetUShort();
                    c.Hp = message.GetUShort();
                    c.BoardAbility = message.GetBool();
                    AllyBoard.SetAt(c,i,j);
                }
            }
        }
        yield break;
    }


    [MessageHandler((ushort) ServerToClient.boardUpdateEnemy)]
    public static void BoardUpdateSyncOpponent(Message message)
    {
        Message m = System.ObjectExtensions.Copy(message);
        AnimationManager.Singleton.ADD_ANIMATION(HandleBoardUpdateSyncOpponent(m));
    }

    public static IEnumerator HandleBoardUpdateSyncOpponent(Message message)
    {
        OpponentBoard.Clear();
        for (int i =0; i<2; i++){
            for(int j=0; j<3; j++)
            {
                if (message.GetBool() == true)
                {
                    BoardCard c = Instantiate( OpponentBoard.NonDraggableCard, OpponentBoard.transform.position, OpponentBoard.transform.rotation);
                    c.Id = message.GetString();
                    c.Attack = message.GetUShort();
                    c.Hp = message.GetUShort();

                    c.x_location=i;
                    c.x_location=j;

                    

                    OpponentBoard.SetAt(c,i,j);
                }
            }
        }
        yield break;
    }
    
    #endregion

}
