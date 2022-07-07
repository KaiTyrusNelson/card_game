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


    public bool CheckSwapable(int x, int y, int x2, int y2){
        
        if (x >= 2 || x2 >= 2 || x < 0 || x2 < 0)
        {
            return false;
        }
        if (y >= 3 || y >= 3 || y < 0 || y2 < 0)
        {
            return false;
        }
        // IF THE FIRST CHARACTER EXISTS
        if (GetAt(x,y) == null){
            return false;
        }
        // AND HASNT SWAPPED
        if (GetAt(x,y).HasSwitched == true){
            return false;
        }
        // AND THE SECOND CHARACTER DOESNT EXIST / HASNOT SWAPPED
        if (GetAt(x2, y2) != null){
            if (GetAt(x2,y2).HasSwitched == true)
            {
                return false;
            }
        }
        return true;
    }

    public IEnumerator SwapPositions(int x, int y, int x2, int y2)
    {
        Character temp = GetAt(x,y);
        Character temp2 = GetAt(x2,y2);
        RemoveAt(x,y);
        RemoveAt(x2,y2);
        if (temp2 != null)
        {
            yield return StartCoroutine(SetAt(temp2, x, y, isSummon : false));
            temp2.HasSwitched = true;
        }
        if (temp != null)
        {
            yield return StartCoroutine(SetAt(temp, x2, y2, isSummon : false));
            temp.HasSwitched = true;
        }
        yield break;
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
        sendCurrentBoardStateMessage(Manager.Players[player].PlayerBoard, player);
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
        sendCurrentBoardStateMessage(Manager.Players[player].PlayerBoard, player);
    }

    public static void sendCurrentBoardStateMessage(GameBoard b, TurnPlayer player)
    {
        Message m2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.boardUpdateEnemy);
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.boardUpdate);

        // FOR EVERY CHARACTER
        foreach (Character c in b.ToArray())
        {
            if (c == null){
                m.Add(false);
                m2.Add(false);
            }else{
                m.Add(true);
                m2.Add(true);
                m.Add(c.Id);
                m2.Add(c.Id);
                m.Add(c.Attack);
                m2.Add(c.Attack);
                m.Add(c.Hp);
                m2.Add(c.Hp);

                // IF THE BOARD ABILITY HAS VIABLE CONDITIONS CHOSEN
                if (c.BoardAbility != null)
                {
                    if (c.BoardAbility.CheckConditions())
                    {
                        m.Add(true);
                    }else{
                        m.Add(false);
                    }
                }else{
                    m.Add(false);
                }
            }
        }
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
