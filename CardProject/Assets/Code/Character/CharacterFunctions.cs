using UnityEngine;
public static class CharacterFunctions
{
    public static bool IsOnBoard(Character c)
    {
        GameBoard playerBoard = Manager.Players[c.Player].PlayerBoard;
        // CYCLES TO SEE IF THE CHARACTER IS ON THE BOARD 
        for (int i = 0; i < 2; i++){
            for (int j = 0; j<3; j++){
                if(playerBoard.GetAt(i,j) == c)
                {
                    Debug.Log("Character has been found on the board");
                    return true;
                }
            }
        }
        Debug.Log("Character has not been found on board");
        return false;
    }
}