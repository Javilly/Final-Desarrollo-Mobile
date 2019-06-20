using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    //UI Manager
    public GameObject UIManager;
    UIManager UIScript;


    //SceneManager
    string sceneToLoad;

    public bool gameStarted = false;
    public int actualTurn;
    public MyPlayersInfo[] playersArray;
    public GameObject[] ballsArray;
    public int actualQuantBalls;
    public int pointsToAdd;

    public static GameSystem instance = null;

    //Gyro
    public GameObject myGyro;
    Giroscopio gyroScript;

    private void Start()
    {
        sceneToLoad = "Scene1";
        UIScript = UIManager.GetComponent<UIManager>();
    }

    private void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
        StartCoroutine(AScene(sceneName));
    }

    IEnumerator AScene(string sceneToLoad)
    {
        AsyncOperation loadingProgress = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!loadingProgress.isDone)
        {
            //UISlider.value = loadingProgress.progress;
            print("Progress: " + loadingProgress.progress);
            yield return null;
        }
    }
    
    public void StartGame(MyPlayersInfo[] arrayPasada)
    {
        LoadScene(sceneToLoad);
        playersArray = arrayPasada;
        gameStarted = true;
        actualTurn = 0;
        actualQuantBalls = 0;
    }

    public void StickPicked()
    {
        print("El Giroscopio llamó al Stick Picked del GS");

        int testQuantBalls = 0;

        if (AllBallsQuiet())
        {
            print("Ciclo del StickPicked en el GS");


            for (int i = 0; i < ballsArray.Length; i++)
            {
                if (ballsArray[i] != null)
                {
                    testQuantBalls++;
                }
            }

            print("Bolas actuales: " + actualQuantBalls + "\t Bolas contadas en el test: " + testQuantBalls);

            if (testQuantBalls == 0)
            {
                EndGame();
            }



            if (testQuantBalls <= actualQuantBalls)
            {
                pointsToAdd = actualQuantBalls - testQuantBalls;
                actualQuantBalls = testQuantBalls;
                PassTurn();
            }
        }
    }

    bool AllBallsQuiet()
    {
        print("Entré al AllBallsQuiet en el GS");
        bool notMoving = false;
        int notMovingBallsCounter = 0;

        while(!notMoving)
        {
            print("Checkeando si las bolas están quietas en el GS");
            for (int i = 0; i < ballsArray.Length; i++)
            {
                if (ballsArray[i] != null && ballsArray[i].GetComponent<Rigidbody>().velocity.magnitude > 0)
                {
                    print("Movimiento de la bola actual: " + ballsArray[i].GetComponent<Rigidbody>().velocity.magnitude);
                    notMoving = false;
                    break;
                }
                notMovingBallsCounter++;
            }

            if (notMovingBallsCounter == ballsArray.Length)
            {
                print("Todas las bolas quietas");
                UIScript.UpdateUI(actualTurn);
                return true;
            }
        }

        return false;
    }

    void EndGame()
    {
        int bestScore = -1;
        string winner = "";

        for (int i = 0; i < playersArray.Length; i++)
        {
            if(playersArray[i].playerPoints > bestScore)
            {
                bestScore = playersArray[i].playerPoints;
                winner = playersArray[i].playerName;
            }

            if(playersArray[i].playerPoints == bestScore)
            {
                winner += " + " + playersArray[i].playerName;
            }
        }

        LoadScene("EndScene");

        UIScript.EndGame(winner);
    }

    public void PassTurn()
    {
        print("Pasando turno en el GS");
        playersArray[actualTurn].playerPoints += pointsToAdd;
        pointsToAdd = 0;
        actualTurn++;

        if(actualTurn == playersArray.Length)
        {
            actualTurn = 0;
        }

        UIScript.PassTurn(actualTurn);
        gyroScript.canSwipeStick = true;
    }

    void Update()
    {
        if (gameStarted)
        {
            if(myGyro == null)
            {
                myGyro = GameObject.Find("Main Camera");
                gyroScript = myGyro.GetComponent<Giroscopio>();
                ballsArray = GameObject.FindGameObjectsWithTag("Ball");

                for (int i = 0; i < ballsArray.Length; i++)
                {
                    actualQuantBalls++;
                }
            }


        }
    }
}


public class MyPlayersInfo
{
    //define all of the values for the class
    public int playerPosition;
    public string playerName;
    public int playerPoints;

    //define a constructor for the class
    public MyPlayersInfo(int plPos, string plName, int plPoints)
    {
        playerPosition = plPos;
        playerName = plName;
        playerPoints = plPoints;
    }
}