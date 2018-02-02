using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LevelEventSys : MonoBehaviour
{

    private bool[] shapesEntered;
    private bool pauseMenuActive;
    private bool levelWonBool;
    private bool levelLostBool;

    private int[] levelData;
    private int[] allShapeHits;
    private int lives;
    private int shapesLeftVal;

    private float[] shapeSizes;

    private Rect screenRect;
    private Rect exitRect;

    private GameObject[] levelShapes;

    private Text shapesLeftText;
    private Text scoreText;

    // Use this for initialization
    void Start()
    {
        // Intializes level data.
        int curLevel = GameManager.instance.curLevel;
        LevelManager.level_manager.InitLevel(curLevel, out levelShapes, out levelData, out allShapeHits);
        print("Initialized Level: " + curLevel);

        InitScene();
    }

    void InitScene()
    {
        // Inititalizes scene var values.
        Time.timeScale = 1;
        TimeScaleSetToOne();
        pauseMenuActive = false;
        lives = 3; 
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        exitRect = new Rect(-400, -400, Screen.width + 800, Screen.height + 800);
        shapesEntered = new bool[levelShapes.Length];

        shapesLeftText = GameObject.Find("shapesLeftVal").GetComponent<Text>();
        shapesLeftVal = levelData[0];
        shapesLeftText.text = shapesLeftVal.ToString();

        scoreText = GameObject.Find("scoreVal").GetComponent<Text>();
        int scoreVal = 0;
        scoreText.text = scoreVal.ToString();

        //Image mainCanvas = GameObject.Find("MainCanvas").GetComponent<Image>();
    }

    void WinScene()
    {
        // Save Data
        if (levelWonBool == false)
        {
            int curLevel = GameManager.instance.curLevel;
            int curLevelHighscore = GameManager.instance.levelHighscores[curLevel - 1];
            int levelScore = int.Parse(scoreText.text);
            if (curLevelHighscore < levelScore)
            {
                GameManager.instance.levelHighscores[curLevel - 1] = curLevelHighscore;
            }
            GameManager.instance.levelsCompleted[curLevel - 1] = true;

            curLevel += 1;
            GameManager.instance.curLevel = curLevel;
            GameManager.instance.Save();

            Canvas winCanvas = GameObject.Find("WinCanvas").GetComponent<Canvas>();
            winCanvas.enabled = true;

            Time.timeScale = 0;
            TimeScaleSetToZero();
            levelWonBool = true;
        }
    }

    void PauseScene()
    {
        Time.timeScale = 0;
        TimeScaleSetToZero();
        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        pauseCanvas.enabled = true;

        pauseMenuActive = true;
    }

    void UnPauseScene()
    {
        Time.timeScale = 1;
        TimeScaleSetToOne();
        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        Canvas achievCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();

        pauseCanvas.enabled = false;
        achievCanvas.enabled = false;
        optionsCanvas.enabled = false;

        pauseMenuActive = false;
    }

    void LoseScene()
    {
        Time.timeScale = 0;
        TimeScaleSetToZero();
        levelLostBool = true;

        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        Canvas achievCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();
        Canvas gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();

        pauseCanvas.enabled = false;
        achievCanvas.enabled = false;
        optionsCanvas.enabled = false;
        gameOverCanvas.enabled = true;
    }

    public void TimeScaleSetToZero()
    {
        Toggle ffToggle = GameObject.Find("fastForwardToggle").GetComponent<Toggle>();
        ffToggle.interactable = false;
    }

    public void TimeScaleSetToOne()
    {
        Toggle ffToggle = GameObject.Find("fastForwardToggle").GetComponent<Toggle>();
        ffToggle.interactable = true;
    }

    public void RetryLevelFromPause()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void RetryLevelFromWin()
    {
        int levelNum = GameManager.instance.curLevel;
        if (levelNum > 1)
        {
            levelNum -= 1;
        }
        GameManager.instance.curLevel = levelNum;
        GameManager.instance.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(2, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void EscPressed()
    {
        if (pauseMenuActive)
        {
            UnPauseScene();
        }
        else
        {
            PauseScene();
        }
    }

    void RPressed(string fromMenu)
    {
        if (fromMenu == "pause")
        {
            RetryLevelFromPause();   
        }
        else if (fromMenu == "win")
        {
            RetryLevelFromWin();
        }
        else if (fromMenu == "lose")
        {
            RetryLevelFromPause();   
        }
    }

    void Update()
    {
        // Sequential checks on shape parameters.
        for (int i = 0; i < levelShapes.Length; i++)
        {
            if (levelShapes[i] != null)
            {
                // Checks for shapes entering the screen
                Vector3 shapePos = levelShapes[i].GetComponent<RectTransform>().position;
                if (screenRect.Contains(shapePos) && shapesEntered[i] == false)
                {
                    shapesEntered[i] = true;
                }

                // Checks to see if life was lost.
                if (!(exitRect.Contains(shapePos)) && shapesEntered[i])
                {
                    lives -= 1;
                    int tempVal = int.Parse(shapesLeftText.text);
                    tempVal -= 1;
                    shapesLeftText.text = tempVal.ToString();
                    switch (lives)
                    {
                        case 2:
                            GameObject heart3 = GameObject.Find("heart3");
                            Destroy(heart3);
                            break;
                        case 1:
                            GameObject heart2 = GameObject.Find("heart2");
                            Destroy(heart2);
                            break;
                        case 0:
                            GameObject heart1 = GameObject.Find("heart1");
                            Destroy(heart1);
                            LoseScene();
                            break;
                    }

                    Destroy(levelShapes[i]);
                }
            }
        }

        // Wins the level.
        shapesLeftVal = int.Parse(shapesLeftText.text);
        if (shapesLeftVal == 0)
        {
            WinScene();
        }
    }

    private void OnGUI()
    {
        // Pauses/unpauses level
        if ((Event.current.Equals(Event.KeyboardEvent("escape"))) && (levelWonBool == false))
        {
            EscPressed();
        }
        // Retries level on loss/restarts level on win
        if ((Event.current.Equals(Event.KeyboardEvent("r"))))
        {
            print("RPressed");
            if (pauseMenuActive) 
            {
                RPressed("pause");
            }
            else if (levelWonBool)
            {
                RPressed("win");
            }
            else if (levelLostBool)
            {
                RPressed("lose");
            }
        }
    }
}
