using UnityEngine;
using RiptideNetworking;
using UnityEngine.EventSystems;
public class BoardCard : Card, IPointerClickHandler
{
    [SerializeField] public int x_location;
    [SerializeField] public int y_location;

    bool _boardAbl;
    public bool BoardAbility { get=>_boardAbl; 
    set {
        activatableAbilityRenderer.SetActive(value);
        _boardAbl = value;
    }}

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.castAbilityFromBoard);
        m.Add(x_location);
        m.Add(y_location);
        NetworkManager.Singleton.Client.Send(m);
    }
    
    [SerializeField] GameObject activatableAbilityRenderer; 
    public void Awake()
    {
        activatableAbilityRenderer.SetActive(false);
        Location = Placement.board;
    }
}
