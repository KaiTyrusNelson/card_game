/// <summary> BASIC GAMEFLOW CONDITION WHICH WILL CHECK EXCLUSIVELY FOR THE EXISTENCE OF A CHAINABLE CARD FROM A PLAYER<summary>
using UnityEngine;
public static class ExistsChainable
{   
    // CHECKS TO SEE IF THE PLAYER HAS ANY CHAINABLE ABILITIES TO THE GIVEN EFFECT
    public static bool Check(TurnPlayer _player)
    {
        // CHECKS THROUGH THE PLAYERS GAMEBOARD TO SEE IF THERE IS A CHAINABLE EFFECT
        for (int i = 0; i < 2; i++)
        {
            for (int j =0; j<3; j++){
                // TODO: THIS WILL NEED TO BE CHANGES AS MORE ABILITY TYPES ARE ADDED
                Character checkCharacter = Manager.Singleton.PlayerBoards[_player].GetAt(i, j);
                if (checkCharacter != null )
                {
                    foreach (Ability abl in checkCharacter.Abilities)
                    {
                        if (abl.CheckConditions())
                        {
                            Debug.Log("A true condition has been found.");
                            return true;
                        }
                    }
                }
            }
        }
        Debug.Log("Failed to find a chainable condition");
        return false;
    }
}