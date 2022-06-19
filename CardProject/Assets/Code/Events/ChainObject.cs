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
    public List<Ability> Abilities{get => _abl; set { _abl = value; }}

    public override IEnumerator Activate()
    {
        ResponseMessages.SendActingPlayer(_player);
        // FINDS ALL CHAINABLE EFFECTS
        for (int i = 0; i < 2; i++)
        {
            for (int j =0; j<3; j++){
                // TODO: THIS WILL NEED TO BE CHANGES AS MORE ABILITY TYPES ARE ADDED
                Character checkCharacter = Manager.Players[_player].PlayerBoard.GetAt(i, j);
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
        // final check to make sure there is an effect to be chosen
        if (_abl.Count > 0)
        {

        int select;
        Debug.Log($"Asking Player how they would like to respond to the following");
        while(true){
            yield return null;
            select = Manager.Players[_player].SelectionCall(_abl.Count);
            if (select != -1)
            {
                yield return StartCoroutine(_abl[select].Activate());
                break;
            }
        }
        }
        else{
            // if there is none skip this ability
            yield return null;
        }
    }
}
