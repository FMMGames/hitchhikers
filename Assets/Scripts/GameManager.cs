﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainScreen, InGame, EndScreen};

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] CamFollow camHolder;
    public PlayerController player;
    public LevelGenerator generator;
    public Transform[] carTargets;
    public Transform carHolder;
    public Transform[] racerSpawnPoints;

    public GameState currentGameState;
    public int currentLevel = 1;
    [Range(0,1)]
    public float AIDifficulty;

    [SerializeField] Camera mainCam;
    [SerializeField] Transform startCamPos, inGameCamPos, endCamPos;
    [SerializeField] float camTransitionSpeed;

    public float timeSinceLevelStart;

    public Racer[] racers;
    public int[] racerScores;
    public string[] racerNames;
    public string playerName;
    [SerializeField] int[] scoreRewardTiers;
    public int playerMoney;

    public bool levelGenerated;
    [SerializeField] string[] allNames;

    [ContextMenu("Load Bot Names")]
    public void LoadBotNames()
    {
        string n = "Assets/Resources/botNames.txt";
        allNames = System.IO.File.ReadAllLines(n);
    }

    private void Awake()
    {
        if (!instance)
            instance = this;

        LoadProgress();
    }

    private void Start()
    {
        ChangeGameState(0);
        generator.GenerateLevel();
        SetupRacers();        
    }

    void SetupRacers()
    {
        for (int i = 0; i < racerNames.Length; i++)
        {
            if (i == 0)
            {
                racerNames[i] = playerName;
            }
            else
            {
                racerNames[i] = allNames[Random.Range(0, allNames.Length)];
            }

            racers[i].racerIndex = i;
        }

        for (int i = 0; i < racerScores.Length; i++)
        {
            racerScores[i] = 0;
        }
    }

    private void Update()
    {
        if(currentGameState == GameState.InGame)
        timeSinceLevelStart += Time.deltaTime;

        ChangeCameraAngle();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetProgress();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ChangeCameraAngle()
    {
        if(currentGameState == GameState.MainScreen)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, startCamPos.position, camTransitionSpeed * Time.deltaTime);
            mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, startCamPos.rotation, camTransitionSpeed * Time.deltaTime);
        }
        else if (currentGameState == GameState.InGame)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, inGameCamPos.position, Time.deltaTime);
            mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, inGameCamPos.rotation, Time.deltaTime);
        }
        else if (currentGameState == GameState.EndScreen)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, endCamPos.position, camTransitionSpeed*Time.deltaTime);
            mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, endCamPos.rotation, camTransitionSpeed*Time.deltaTime);
        }
    }

    void ChangeGameState(int i)
    {
        switch (i)
        {
            case 0:
                currentGameState = GameState.MainScreen;
                break;
            case 1:
                currentGameState = GameState.InGame;
                break;
            case 2:
                currentGameState = GameState.EndScreen;
                break;
        }

        UIManager.instance.UpdateScreens();
    }

    public void LevelStart()
    {
        ChangeGameState(1);
    }

    public void LevelEnd()
    {
        ChangeGameState(2);

        SortRacersByDescendingScore();

        if (racerNames[0] == playerName)
            UIManager.instance.UpdateResultScreen(true);
        else
            UIManager.instance.UpdateResultScreen(false);

        PassScores();
    }

    public void NextLevel()
    {
        currentLevel++;
        ChangeGameState(0);
        player.ResetPlayer();
        ResetAI();
        ResetLevel();
        generator.GenerateLevel();
        SetupRacers();

        SaveProgress();
        UIManager.instance.EnableLoadingScreen();
    }

    void LoadProgress()
    {
        if (PlayerPrefs.GetInt("CurrentMoney") > 0)
            playerMoney = PlayerPrefs.GetInt("CurrentMoney");

        if (PlayerPrefs.GetInt("CurrentLevel") > 0)
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentMoney", playerMoney);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
    }

    void ResetProgress()
    {
        PlayerPrefs.SetInt("CurrentMoney", 0);
        PlayerPrefs.SetInt("CurrentLevel", 0);
    }

    void ResetLevel()
    {
        levelGenerated = false;

        foreach (Transform child in carHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in generator.level)
        {
            Destroy(child.gameObject);
        }

        camHolder.transform.position = player.transform.position;
    }

    void ResetAI()
    {
        for (int i = 1; i < racers.Length; i++)
        {
            if (i > 0)
            {
                racers[i].transform.position = GameManager.instance.racerSpawnPoints[i].position;
                racers[i].transform.rotation = GameManager.instance.racerSpawnPoints[i].rotation;

                racers[i].GetComponent<AIRacerController>().currentCar = null;
                racers[i].GetComponent<AIRacerController>().transform.parent = null;

                racers[i].GetComponent<AIRacerController>().anim.SetTrigger("Reset");
                racers[i].GetComponent<AIRacerController>().ResetAppearance();
            }
        }
    }

    /*
    void SortRacersByScore()
    {
        int min;
        int temp;
        string nameTemp;

        for (int i = 0; i < racerScores.Length; i++)
        {
            min = i;

            for (int j = i+1; j < racerScores.Length; j++)
            {
                if (racerScores[j] < racerScores[min])
                    min = j;
            }

            if(min != i)
            {
                temp = racerScores[i];
                racerScores[i] = racerScores[min];
                racerScores[min] = temp;

                nameTemp = racerNames[i];
                racerNames[i] = racerNames[min];
                racerNames[min] = nameTemp;
            }
        }
    }
    */

    void SortRacersByDescendingScore()
    {
        int max;
        int temp;
        string nameTemp;

        for (int i = 0; i < racerScores.Length; i++)
        {
            max = i;

            for (int j = i + 1; j < racerScores.Length; j++)
            {
                if (racerScores[j] > racerScores[max])
                    max = j;
            }

            if (max != i)
            {
                temp = racerScores[i];
                racerScores[i] = racerScores[max];
                racerScores[max] = temp;

                nameTemp = racerNames[i];
                racerNames[i] = racerNames[max];
                racerNames[max] = nameTemp;
            }
        }
    }

    void PassScores()
    {
        for (int i = 0; i < racerNames.Length; i++)
        {
            UIManager.instance.SpawnScoreElement(i+1, racerNames[i], racerScores[i]);
        }
    }

    public void EarnScore(int who, int tier)
    {
        racerScores[who] += scoreRewardTiers[tier];

        if (who == 0)
            playerMoney += scoreRewardTiers[tier];
    }
}
