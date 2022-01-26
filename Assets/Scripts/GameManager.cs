using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement;
using SimpleJSON;


public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GameController(string msg);

    public Text Balance;
    public Text Player1;
    public Text Player1_1;
    public Text Player2;
    public Text normal;
    public Text split;
    public Sprite[] CardTexture = new Sprite[52];
    public InputField InvestedMoney;
    public Button Plusbutton;
    public Button Minusebutton;
    public Button Hitbutton;
    public Button Standbutton;
    public Button Startbutton;
    public Button Splitbutton;
    public Button Doublebutton;
    public static APIForm apiform;
    public static HitAndStandForm H_S_state;
    public static Globalinitial _global;
    public Transform prefab;
    public Transform prefabTwo;
    public GameObject Anim;
    public Sprite a_Server;
    public Sprite a_Bet;
    public Sprite a_Response;

    private bool normalBool = false;
    private bool splitBool = false;
    private string[] MyCardsName = new string[52];
    private string Token ="";
    private int H_imagecount = 0;
    private int C_imagecount = 0;
    private int S_imagecount = 0;
    private int playeronePoint = 0;
    private int playeronePoint_1 = 0;
    private int playertwoPoint = 0;
    private float myBal;
    private float myBet;
    private Sprite First;
    private Transform CardChild;
    // Start is called before the first frame update
    void Start()
    {
        myBal = 10000;
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        GameController("Ready");
#endif
        for (int i = 0; i < 52; i++)
        {
            MyCardsName[i] = CardTexture[i].name;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {
        if (myBal >= Single.Parse(InvestedMoney.GetComponent<InputField>().text))
        {
            normalBool = false;
            splitBool = false;
            playeronePoint = 0;
            playertwoPoint = 0;
            playeronePoint_1 = 0;
            Player1.text = "0";
            Player1_1.text = "0";
            Player2.text = "0";
            H_imagecount = 0;
            C_imagecount = 0;
            defalt();
            CardDestroy();
            StartCoroutine(newStart());
        }
    }
    private void defalt()
    {
        Startbutton.interactable = false;
        Hitbutton.interactable = false;
        Standbutton.interactable = false;
        Plusbutton.interactable = false;
        Splitbutton.interactable = false;
        Doublebutton.interactable = false;
        Minusebutton.interactable = false;
        InvestedMoney.interactable = false;
    }

    public void HitPush()
    {
        StartCoroutine(Hit());
    }

    public void StandPush()
    {
        StartCoroutine(Stand());
    }

    public void DoublePush()
    {
        StartCoroutine(Double());
    }

    public void SplitPush()
    {
        StartCoroutine(Split());
    }
    public void InvestPlus()
    {
        if(Single.Parse(InvestedMoney.GetComponent<InputField>().text) + 100f > Single.Parse(Balance.GetComponent<Text>().text))
        {
            InvestedMoney.GetComponent<InputField>().text = Balance.GetComponent<Text>().text;
            Plusbutton.interactable = false;
            Minusebutton.interactable = true;
        }
        else
        {
            InvestedMoney.GetComponent<InputField>().text = (Single.Parse(InvestedMoney.GetComponent<InputField>().text) + 100f).ToString();
            Minusebutton.interactable = true;
        }
    }
    public void InvestMinuse()
    {
        if (Single.Parse(InvestedMoney.GetComponent<InputField>().text) > 100)
        {
            InvestedMoney.GetComponent<InputField>().text = (Single.Parse(InvestedMoney.GetComponent<InputField>().text) - 100f).ToString();
            Plusbutton.interactable = true;
        }
        if (Single.Parse(InvestedMoney.GetComponent<InputField>().text) < 100)
        {
            Minusebutton.interactable = false;
        }
    }
    public void ChangeInput()
    {
        if (Single.Parse(InvestedMoney.GetComponent<InputField>().text) < 100)
        {
            InvestedMoney.GetComponent<InputField>().text = "100";
            Minusebutton.interactable = false;
        }
        if (Single.Parse(InvestedMoney.GetComponent<InputField>().text) >= Single.Parse(Balance.GetComponent<Text>().text))
        {
            InvestedMoney.GetComponent<InputField>().text = (Single.Parse(Balance.GetComponent<Text>().text)).ToString();
            Plusbutton.interactable = false;
            Minusebutton.interactable = true;
        }
        else
        {
            Plusbutton.interactable = true;
        }
        if (Single.Parse(InvestedMoney.GetComponent<InputField>().text) > 100)
        {
            Minusebutton.interactable = true;
        }
    }


    private IEnumerator CardDeploy(){
        playeronePoint = apiform.Playone;
        playeronePoint_1 = apiform.Playone_1;
        for (int i = H_imagecount;i<apiform.humancount;i++){
            CardChild = Instantiate(prefab,Vector2.zero,Quaternion.identity);
            CardChild.transform.SetParent(GameObject.FindGameObjectWithTag("Human").transform);
            CardChild.tag = "Card";
            CardChild.name = "Cards_H_"+i;
            CardChild.GetComponent<Image>().sprite = CardTexture[Array.IndexOf(MyCardsName, apiform.humancards[i])];
            CardChild.GetComponent<RectTransform>().anchorMax = new Vector2((i + 1) * 0.12f, 0.95f);
            CardChild.GetComponent<RectTransform>().anchorMin = new Vector2(0.03f + i * 0.12f, 0.05f);
            CardChild.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            CardChild.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            CardChild.localScale = new Vector2(1f, 1f);
            yield return new WaitForSeconds(0.5f);
        }
        Player1.text = playeronePoint.ToString();

        for (int i = S_imagecount;i<apiform.splitcount;i++){
            CardChild = Instantiate(prefab,Vector2.zero,Quaternion.identity);
            CardChild.transform.SetParent(GameObject.FindGameObjectWithTag("Human_two").transform);
            CardChild.tag = "Card";
            CardChild.name = "Cards_H_S"+i;
            CardChild.GetComponent<Image>().sprite = CardTexture[Array.IndexOf(MyCardsName, apiform.splitcards[i])];
            CardChild.GetComponent<RectTransform>().anchorMax = new Vector2((i + 1) * 0.12f, 0.95f);
            CardChild.GetComponent<RectTransform>().anchorMin = new Vector2(0.03f + i * 0.12f, 0.05f);
            CardChild.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            CardChild.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            CardChild.localScale = new Vector2(1f, 1f);
            yield return new WaitForSeconds(0.5f);
        }
        Player1_1.text = playeronePoint_1.ToString();
        playertwoPoint = apiform.Playtwo;
        for (int i =C_imagecount;i<apiform.computercount;i++){
            if(i==0){
                CardChild = Instantiate(prefabTwo, Vector2.zero, Quaternion.identity);
                if(apiform.gameState !=0){
                    CardChild.GetComponent<Image>().sprite = CardTexture[Array.IndexOf(MyCardsName, apiform.computercards[i])];
                    Player2.text = playertwoPoint.ToString();
                }
            }else{
                CardChild = Instantiate(prefab, Vector2.zero, Quaternion.identity);
                CardChild.GetComponent<Image>().sprite = CardTexture[Array.IndexOf(MyCardsName, apiform.computercards[i])];
            }
            CardChild.transform.SetParent(GameObject.FindGameObjectWithTag("Computer").transform);
            CardChild.name = "Cards_C-"+i;
            CardChild.tag = "Card";
            CardChild.GetComponent<RectTransform>().anchorMax = new Vector2((i + 1) * 0.12f, 0.95f);
            CardChild.GetComponent<RectTransform>().anchorMin = new Vector2(0.03f + i * 0.12f, 0.05f);
            CardChild.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            CardChild.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            CardChild.localScale = new Vector2(1f, 1f);
            yield return new WaitForSeconds(0.5f);
        }

        winState();
        if (apiform.SplitBool)
        {
            if(normalBool && splitBool)
            {
                GameObject.Find("Cards_C-0").GetComponent<Image>().sprite = CardTexture[Array.IndexOf(MyCardsName, apiform.computercards[0])];
                Player2.text = playertwoPoint.ToString();
                setButton();
            }
            else
            {
                Hitbutton.interactable = true;
                Standbutton.interactable = true;
            }
        }
        else
        {
            if (normalBool)
            {
                Player2.text = playertwoPoint.ToString();
                GameObject.Find("Cards_C-0").GetComponent<Image>().sprite = CardTexture[Array.IndexOf(MyCardsName, apiform.computercards[0])];
                setButton();
            }
            else
            {
                Debug.Log(1);
                Hitbutton.interactable = true;
                Standbutton.interactable = true;
            }
        }
        H_imagecount = apiform.humancount;
        S_imagecount = apiform.splitcount;
        C_imagecount = apiform.computercount;
    } 
//     // you can request to server here
    private IEnumerator newStart()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", Token);
        form.AddField("betValue", InvestedMoney.GetComponent<InputField>().text);
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/start-BlackJack", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Anim.GetComponent<Image>().sprite = a_Response;
            Anim.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            Anim.SetActive(false);
            setButton();
        }
        else
        {
            string strData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strData);
            MessageCheck();
            yield return new WaitForSeconds(2);
            if(apiform.gameState==0){
                buttonActive();
                ButtonState();
            }
        }
    }

    private IEnumerator Hit(){
        defalt();
        WWWForm form = new WWWForm();
        form.AddField("token", Token);
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/Hit", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            string strData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strData);
            StartCoroutine(CardDeploy());
        }
        else
        {
            Anim.GetComponent<Image>().sprite = a_Response;
            Anim.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            Anim.SetActive(false);
            buttonActive();
        }
    }
    private IEnumerator Stand()
    {
        defalt();
        WWWForm form = new WWWForm();
        form.AddField("token", Token);
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/Stand", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            string strData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strData);
            StartCoroutine(CardDeploy());
        }
        else
        {
            Anim.GetComponent<Image>().sprite = a_Response;
            Anim.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            Anim.SetActive(false);
            buttonActive();
        }
    }
    private IEnumerator Split(){
        defalt();
        if (apiform.SplitBool){
            Hitbutton.interactable = true;
            Standbutton.interactable = true;
            WWWForm form = new WWWForm();
            form.AddField("token", Token);
            _global = new Globalinitial();
            UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/Split", form);
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                GameObject.Find("Cards_H_1").transform.SetParent(GameObject.FindGameObjectWithTag("Human_two").transform);
                GameObject.Find("Cards_H_1").GetComponent<RectTransform>().anchorMax = new Vector2(0.12f, 0.95f);
                GameObject.Find("Cards_H_1").GetComponent<RectTransform>().anchorMin = new Vector2(0.03f, 0.05f);
                GameObject.Find("Cards_H_1").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                GameObject.Find("Cards_H_1").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                H_imagecount = 1;
                S_imagecount = 1;
                playeronePoint_1 = apiform.Playone/2;
                Player1_1.text = playeronePoint_1.ToString();
                playeronePoint = apiform.Playone/2;
                Player1.text = playeronePoint.ToString();
                Debug.Log(myBet);
                myBet = myBet / 2;
                Debug.Log(myBet);
            }else{
                Anim.GetComponent<Image>().sprite = a_Response;
                Anim.SetActive(true);
                yield return new WaitForSeconds(2.4f);
                Anim.SetActive(false);
                buttonActive();
            }
        }
    }
    private IEnumerator Double()
    {
        defalt();
        Hitbutton.interactable = false;
        Standbutton.interactable = false;
        WWWForm form = new WWWForm();
        form.AddField("token", Token);
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/Double", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            string strData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strData);
            Doublebutton.interactable = false;
            StartCoroutine(UpdateCoinsAmount());
            myBet = myBet * 2;
            StartCoroutine(CardDeploy());
        }
        else
        {
            Anim.GetComponent<Image>().sprite = a_Response;
            Anim.SetActive(true);
            yield return new WaitForSeconds(2.4f);
            Anim.SetActive(false);
            buttonActive();
        }
    }

    private void MessageCheck()
    {
        if (apiform.myMessage == 0){
            normal.text = "";
            split.text = "";
            myBet = Single.Parse(InvestedMoney.text);
            StartCoroutine(UpdateCoinsAmount());
            StartCoroutine(CardDeploy());
        }else if(apiform.myMessage == 1){
            StartCoroutine(Alerts(a_Server));
            setButton();
        }else if(apiform.myMessage == 2){
            StartCoroutine(Alerts(a_Bet));
            setButton();
        }
    }
    private void winState(){
        if(apiform.gameState == 1){
            normal.text = "Lose";
            normalBool = true;
        }
        else if(apiform.gameState == 2){
            myBet = -myBet;
            StartCoroutine(UpdateCoinsAmount());
            normal.text = "Tie";
            normalBool = true;
        }
        else if(apiform.gameState == 3)
        {
            myBet = -2*myBet;
            StartCoroutine(UpdateCoinsAmount());
            normal.text = "Win";
            normalBool = true;
        }
        if (apiform.s_gameState == 1)
        {
            split.text = "Lose";
            splitBool = true;
        }
        else if (apiform.s_gameState == 2)
        {
            myBet = -myBet;
            StartCoroutine(UpdateCoinsAmount());
            split.text = "Tie";
            splitBool = true;

        }
        else if (apiform.s_gameState == 3)
        {
            myBet = -2*myBet;
            StartCoroutine(UpdateCoinsAmount());
            split.text = "Win";
            splitBool = true;
        }
    }

    private void setButton()
    {
        Plusbutton.interactable = true;
        Minusebutton.interactable = true;
        Hitbutton.interactable = false;
        Standbutton.interactable = false;
        Splitbutton.interactable = false;
        Doublebutton.interactable = false;
        Startbutton.interactable = true;
    }

private void buttonActive(){
    Hitbutton.interactable = true;
    Standbutton.interactable = true;
}

    private void ButtonState(){
        if (apiform.SplitBool)
        {
            Splitbutton.interactable = true;
        }
        Doublebutton.interactable = true;
    }

    private IEnumerator Alerts(Sprite n_sprite){
        Anim.GetComponent<Image>().sprite = n_sprite;
        Anim.SetActive(true);
        yield return new WaitForSeconds(2.4f);
        Anim.SetActive(false);
    }

    private void CardDestroy()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Card").Count(); i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Card")[i], 0f);
        }
    }

    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        Token = usersInfo["token"];
        Balance.GetComponent<Text>().text = (usersInfo["amount"]).ToString();
        myBal = usersInfo["amount"];
    }

    //     //This is balance effect
    private IEnumerator UpdateCoinsAmount()
    {
        const float seconds = 1.5f;
        float elapsedTime = 0;
        float newBal = myBal;
        myBal -= myBet;
        while (elapsedTime < seconds)
        {
            Balance.GetComponent<Text>().text = Mathf.Floor(Mathf.Lerp(newBal, newBal - myBet, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        Balance.GetComponent<Text>().text = (myBal).ToString();
    }
}




