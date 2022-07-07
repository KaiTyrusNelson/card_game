// THESE EVENTS ARE FOR EFFECTS THAT OPERATE INDEPENDENT OF A TARGET
public abstract class TargetlessStackEvent : StackEvent
{
    public TurnPlayer AssociatedPlayer;
    public virtual bool CheckCondition(){
        return true;
    }
}