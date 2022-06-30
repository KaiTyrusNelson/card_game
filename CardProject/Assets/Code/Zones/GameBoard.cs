using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public enum Board
{
    allyBoard = 1,
    enemyBoard
}

public class GameBoard : MonoBehaviour
{
    [SerializeField] public Player Owner; 
    [SerializeField] TurnPlayer _player;
    [SerializeField] Character[] row0 = new Character[3];
    [SerializeField] Character[] row1 = new Character[3];

    public Character GetAt(int x, int y){
        if (x == 0) return row0[y];
        return row1[y];
    }
    public List<Tuple> GetEmptySpaces()
    {
        List<Tuple> emptySpaces = new List<Tuple>();
        for (int i =0; i < 2; i++){
            for (int j=0; j<3; j++){
                if (GetAt(i,j) == null)
                {
                    Tuple newTuple = new Tuple();
                    newTuple.x = i;
                    newTuple.y = j;
                    emptySpaces.Add(newTuple);
                }
            }
        }
        return emptySpaces;
    }
    public bool RemoveCard(Character c)
    {
        for (int i =0; i < 2; i++){
            for (int j=0; j<3; j++){
                if (GetAt(i,j) == c)
                {
                    RemoveAt(i,j);
                    return true;
                }
            }
        }
        return false;
    }

    // CONVERTS TO AN ARRAY FOR EASY HANDLING
    public Character[] ToArray()
    {
        Character[] arr = new Character[6];
        int m=0;
        for(int i = 0; i < 2; i++){
            for(int j =0; j < 3; j++){
                arr[m]=GetAt(i,j);
                m++;
            }
        }
        return arr;
    }

    // SWITCHING SET AT TO BE AN IENUMERATOR SO THAT THE SUMMON ABILITY CAN BE CALLED
    public IEnumerator SetAt(Character c, int x, int y, bool isSummon = true){
        x = x%2;
        if (x==0) row0[y] = c;
        if (x==1) row1[y] = c;
        c.transform.SetParent(this.transform);
        sendSummonMessage(c, x, y, _player);
        c.Location = CardLocations.Board;
        Debug.Log("SET AT CALLED");


        if (isSummon)
        {
            Debug.Log("IS SUMMON CALLED");
            yield return c.OnSummon();
        }
    }

    public ushort CountEmptySpaces()
    {
        ushort  i =0;
        Character[] characterArray = ToArray();
        foreach (Character c in characterArray){
            if (c == null)
            {
                i++;
            }
        }
        return i;
    }
    public void RemoveAt(int x, int y)
    {
        x = x%2;
        if (x==0) row0[y] = null;
        if (x==1) row1[y] = null;
        // TODO: ADD MESSAGE HANDLER FOR THIS EVENT
        sendRemoveMessage(x,y,_player);
    }

    public void SendToGYAt(int x, int y)
    {
        if (GetAt(x,y)!=null)
        {
            Owner.PlayerGraveyard.AddCard(GetAt(x,y));
            RemoveAt(x, y);
        }
    }
    
    #region Messages
    /// <summary> Designed to communicate when a card has been summoned <summary>
    public static void sendSummonMessage(Character c, int x, int y, TurnPlayer player){
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.summonMessage);
        m.Add(c.Hp);
        m.Add(c.Attack);
        m.Add(c.Id);
        m.Add(x);
        m.Add(y);
        m.Add((ushort)Board.allyBoard);
        Message m2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.summonMessage);
        m2.Add(c.Hp);
        m2.Add(c.Attack);
        m2.Add(c.Id);
        m2.Add(x);
        m2.Add(y);
        m2.Add((ushort)Board.enemyBoard);
        NetworkManagerV2.Instance.server.Send(m, (ushort)player);
        NetworkManagerV2.Instance.server.Send(m2, (ushort)player.OppositePlayer());
    }

    public static void sendRemoveMessage(int x, int y, TurnPlayer player){
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.removeMessage);
        m.Add(x);
        m.Add(y);
        m.Add((ushort)Board.allyBoard);
        Message m2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.removeMessage);
        m2.Add(x);
        m2.Add(y);
        m2.Add((ushort)Board.enemyBoard);
        NetworkManagerV2.Instance.server.Send(m, (ushort)player);
        NetworkManagerV2.Instance.server.Send(m2, (ushort)player.OppositePlayer());
    }
    #endregion

    #region VisualizationFunctions
    [SerializeField]
    Transform[] row0Proxies; 
    [SerializeField]
    Transform[] row1Proxies; 

    public void Update(){
        for (int i = 0; i < row0.Length; i++){
            if (row0[i]!=null)
                row0[i].transform.position = row0Proxies[i].position + new Vector3(0,0,1);
        }
        for(int i =0; i < row1.Length; i++){
            if (row1[i]!=null)
                row1[i].transform.position = row1Proxies[i].position + new Vector3(0,0,1);
        }
    }
    #endregion

    
}
