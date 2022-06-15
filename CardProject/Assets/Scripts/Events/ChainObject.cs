/// <summary> Prompt a chain response, giving a list of responses to make<summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChainObject : StackEvent
{
    // TO DO, PROMPT CONDITIONS
    [SerializeField] TurnPlayer _player;
    public TurnPlayer Player {
        get => _player;
        set{
            _player = value;
        }
    }

    // LIST OF CHAINABLE ABILITIES
    [SerializeField] List<Ability> _abl;
    public List<Ability> Abilities;


    public void Awake(){
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
                            _abl.Add(abl);
                    }
                }
            }
        }
    }
    public override IEnumerator Activate()
    {
        Debug.Log($"Asking Player how they would like to respond to the following");
        while(true){
            yield return null;
        }
    }
}
