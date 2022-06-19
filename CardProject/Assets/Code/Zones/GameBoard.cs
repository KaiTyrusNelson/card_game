using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

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
    public void SetAt(Character c, int x, int y){
        x = x%2;
        if (x==0) row0[y] = c;
        if (x==1) row1[y] = c;
        c.transform.SetParent(this.transform);
        sendSummonMessage(c, x, y, _player);
    }

    public void RemoveAt(int x, int y)
    {
        x = x%2;
        if (x==0) row0[y] = null;
        if (x==1) row1[y] = null;
        // TODO: ADD MESSAGE HANDLER FOR THIS EVENT
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
        m.Add((ushort)player);
        NetworkManagerV2.Instance.server.SendToAll(m);
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
