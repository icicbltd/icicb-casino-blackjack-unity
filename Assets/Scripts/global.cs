public class APIForm
{
    public int myMessage;
    public bool dealButton;
    public bool hitButton;
    public bool standButton;
    public bool splitButton;
    public bool doubleButton;
    public bool insuranceButton;
    public int[] playerCards;
    public int playerCount;
    public int playertotalWeight;
    public int[] dealerCards;
    public int dealerCount;
    public int dealertotalWeight;
    public int[] splitCards;
    public int splitCount;
    public int splittotalWeight;
    public int winState;
    public int s_winState;
    public float winMoney;
    public float insuranceMoney;
}
public class HitAndStandForm
{
    public string states;
}

public class Globalinitial
{
    //public string BaseUrl = "http://192.168.115.178:1026/";
    public string BaseUrl = "http://31.220.49.238:80/";
}