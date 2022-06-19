using UnityEngine;
public class DamageEffect:Effect{
    [SerializeField] ushort _damage;
    [SerializeField] DamageEvent _evt;
    // TODO: Change to a stack event
    public override void Activation(Character c){
        DamageEvent evt = Instantiate(_evt, transform.position, transform.rotation);
        evt.InitializeVariables(c, _damage);
        Manager.Singleton.StackPush(evt);
    }
}