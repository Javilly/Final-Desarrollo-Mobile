using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //UI
    public Slider sliderPlayers;
    public Text textPlayers;
    public GameObject sliderGO;
    public GameObject inputGO;
    public Text textNames;
    public Text listPlayers;

    public GameObject nowPlayingText;
    public Text playingTextScript;

    public GameObject winnerText;
    public Text winnerTextScript;

    //GameManager
    public GameObject gameManager;
    GameSystem GSScript;

    //state counter
    int counter = 0;

    //Misc
    private bool quantPlayersSelected = false;
    MyPlayersInfo[] playersArray;
    public static UIManager instance = null;
    bool gameStarted = false;


    private void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GSScript = gameManager.GetComponent<GameSystem>();
    }

    public void UpdateUI(int actualTurn)
    {
        playingTextScript.text = playersArray[actualTurn].playerName.ToString() + " (" + playersArray[actualTurn].playerPoints.ToString() + ")";
    }

    public void PassTurn(int actualTurn)
    {
        print("Actualizacion de UI para el jugador en el UIManager.\t Actual turn: " + actualTurn);
        playingTextScript.text = playersArray[actualTurn].playerName.ToString() + " (" + playersArray[actualTurn].playerPoints.ToString() + ")";
    }

    public void EndGame(string winner)
    {
        winnerText = GameObject.Find("Winner Name");
        winnerTextScript = winnerText.GetComponent<Text>();
        winnerTextScript.text = winner;
    }

    public void ButtonPressed()
    {
        if (!quantPlayersSelected)
        {
            playersArray = new MyPlayersInfo[(int)sliderPlayers.value];
            sliderGO.SetActive(false);
            inputGO.SetActive(true);
            listPlayers.text = "Players list:";
            quantPlayersSelected = true;
        }
        else
        {
            if (counter < playersArray.Length)
            {
                playersArray[counter] = new MyPlayersInfo(counter, textNames.text, 0);
                print("Counter: " + counter + ".   ArrayL: " + playersArray.Length);
                listPlayers.text += "\n\t -" + playersArray[counter].playerName;
                textNames.text = "";
                counter++;
            }
            else
            {
                GSScript.StartGame(playersArray);
                gameStarted = true;
            }
        }
    }

    void Update()
    {
        textPlayers.text = sliderPlayers.value.ToString();
        if (gameStarted)
        {
            nowPlayingText = GameObject.Find("Actual Player");
            playingTextScript = nowPlayingText.GetComponent<Text>();
            playingTextScript.text = playersArray[0].playerName;
            gameStarted = false;
        }
    }
}
