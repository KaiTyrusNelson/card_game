using UnityEngine;
using System.Collections;
using RiptideNetworking;
public class AttackEvent : StackEvent
{
    [SerializeField] public Character AttackingCharacter;

    /// <summary> LOCATES THE OPPOSING CHARACTER AT THE TIME OF THE EVENT TRIGGER<summary>
    public Character FindOpponent(Character c){
        Tuple location = c.FindLocationOnBoard();
        if (location == null){
            return null;
        }
        int y = location.y;
        // DEFINES THE BOARD WE ARE ATTACKING TO
        GameBoard targetBoard = Manager.Players[c.Player.OppositePlayer()].PlayerBoard;
        // CYCLES THROUGH THE X VALUES TO FIND THE TARGET
        for (int i =0; i < 2; i++)
        {
            if (targetBoard.GetAt(i, y) != null)
            {
                return targetBoard.GetAt(i, y);
            }
        }
        // IF NOT WE WILL RETURN A NULL VALUE
        return null;
    }
    public override IEnumerator Activate()
    {
        Character opponent = FindOpponent(AttackingCharacter);
        // IF THERE IS AN OPPONENT ATTACK IT
        if (opponent != null)
        {
            Tuple location = AttackingCharacter.FindLocationOnBoard();
            SendAttackMessage(AttackingCharacter.Player, location.x, location.y);
            AttackingCharacter.AttackCharacter(opponent);
            opponent.AttackCharacter(AttackingCharacter);
        }
        yield return null;


    }

    public void SendAttackMessage(TurnPlayer player, int x, int y)
    {
        // CREATES A MESSAGE TO SEND TO THE PLAYERS
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.attackResolveEvent);
        Message m2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.attackResolveEvent);
        // POPULATES THE MESSAGES
        m.Add(x);
        m.Add(y);
        m2.Add(x);
        m2.Add(y);
        // TELLS THE PLAYERS WHICH BOARD IT CORESPONDS TO
        m.Add((ushort)Board.allyBoard);
        m2.Add((ushort)Board.enemyBoard);
        // SENDS THE MESSAGES
        NetworkManagerV2.Instance.server.Send(m, (ushort)player);
        NetworkManagerV2.Instance.server.Send(m2, (ushort)player.OppositePlayer());
    }
    



}