using UnityEngine;
using RiptideNetworking;
using System.Collections;
public class AttackMessageIndicator : MonoBehaviour
{
    public static AttackMessageIndicator Singleton;
    public void Awake(){
        Singleton = this;
    }
    [SerializeField] GameObject allyAttackingArrow;
    [SerializeField] GameObject enemyAttackingArrow;
    [SerializeField] GameObject allyAttackingArrowR;
    [SerializeField] GameObject enemyAttackingArrowR;

    [MessageHandler((ushort) ServerToClient.attackDeclareEvent)]
    public static void AttackMessage(Message message)
    {
        int x_location = message.GetInt();
        int y_location = message.GetInt();
        Board boardSelection = (Board)message.GetUShort();

        switch (boardSelection)
        {
            case(Board.allyBoard):
            {
                AnimationManager.Singleton.ADD_ANIMATION(Singleton.AllyAttackIndicator(x_location, y_location));
                break;
            }
            case(Board.enemyBoard):
            {
                AnimationManager.Singleton.ADD_ANIMATION(Singleton.EnemyAttackIndicator(x_location, y_location));
                break;
            }
        }
    }
    [MessageHandler((ushort) ServerToClient.attackResolveEvent)]
    public static void AttackMessageResolve(Message message)
    {
        int x_location = message.GetInt();
        int y_location = message.GetInt();
        Board boardSelection = (Board)message.GetUShort();

        switch (boardSelection)
        {
            case(Board.allyBoard):
            {
                AnimationManager.Singleton.ADD_ANIMATION(Singleton.AllyAttackResolve(x_location, y_location));
                break;
            }
            case(Board.enemyBoard):
            {
                AnimationManager.Singleton.ADD_ANIMATION(Singleton.EnemyAttackResolve(x_location, y_location));
                break;
            }
        }
    }

    public IEnumerator AllyAttackIndicator(int x, int y)
    {
        Transform t = GameBoard.AllyBoard.GetTransformAt(x, y);
        Debug.Log(t.position);
        GameObject temp = Instantiate(allyAttackingArrow, t.position, allyAttackingArrow.GetComponent<Transform>().rotation);
        temp.transform.SetParent(GameBoard.AllyBoard.transform);
        yield return new WaitForSeconds(2);
        Destroy(temp);
    }

    public IEnumerator EnemyAttackIndicator(int x, int y)
    {
        Transform t = GameBoard.OpponentBoard.GetTransformAt(x, y);
        Debug.Log(t.position);
        GameObject temp = Instantiate(enemyAttackingArrow, t.position, enemyAttackingArrow.GetComponent<Transform>().rotation);
        temp.transform.SetParent(GameBoard.OpponentBoard.transform);
        yield return new WaitForSeconds(2);
        Destroy(temp);
    }
    public IEnumerator AllyAttackResolve(int x, int y)
    {
        Transform t = GameBoard.AllyBoard.GetTransformAt(x, y);
        Debug.Log(t.position);
        GameObject temp = Instantiate(allyAttackingArrowR, t.position, allyAttackingArrowR.GetComponent<Transform>().rotation);
        temp.transform.SetParent(GameBoard.AllyBoard.transform);
        yield return new WaitForSeconds(2);
        Destroy(temp);
    }

    public IEnumerator EnemyAttackResolve(int x, int y)
    {
        Transform t = GameBoard.OpponentBoard.GetTransformAt(x, y);
        Debug.Log(t.position);
        GameObject temp = Instantiate(enemyAttackingArrowR, t.position, enemyAttackingArrowR.GetComponent<Transform>().rotation);
        temp.transform.SetParent(GameBoard.OpponentBoard.transform);
        yield return new WaitForSeconds(2);
        Destroy(temp);
    }


}