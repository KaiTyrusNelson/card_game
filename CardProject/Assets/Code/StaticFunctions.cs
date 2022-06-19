public static class FUNCTIONS{
    public static TurnPlayer OppositePlayer(this TurnPlayer p)
    {
        switch(p){
            case(TurnPlayer.Player1):
                return TurnPlayer.Player2;
            case(TurnPlayer.Player2):
                return TurnPlayer.Player1;
        }
        return TurnPlayer.Player1;
    }
}