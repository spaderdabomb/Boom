using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class LevelEventSys : MonoBehaviour
{

    private bool[] shapesEntered;
    private bool pauseMenuActive;
    private bool levelWonBool;
    private bool levelLostBool;
	private bool movedCloserBool;

    private int[] levelData;
    private int[] allShapeHits;
    private int lives;
    private int shapesLeftVal;
	private int lastShapeCheckedIdx;

    private float[] shapeSizes;
    private float screenScaling;

    private string shapesLeftStripped;
    private string scoreTextStripped;

    private Rect screenRect;
    private Rect exitRect;
	private Rect exitRectLarge;

	private GameObject[] levelShapes;

    private Text shapesLeftText;
    private Text scoreText;

    // Use this for initialization
    void Start()
    {
        // Intializes level data.
        int curLevel = GameManager.instance.curLevel;
        LevelManager.level_manager.InitLevel(curLevel, out levelShapes, out levelData, out allShapeHits);
        screenScaling = Screen.width / 808.0f;
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
        screenRect = new Rect(50 * screenScaling, 50 * screenScaling, Screen.width - 100 * screenScaling, Screen.height - 100 * screenScaling);
		exitRect = new Rect(-50 * screenScaling, -50 * screenScaling, Screen.width + 100 * screenScaling, Screen.height + 100 * screenScaling);
		exitRectLarge = new Rect(-180 * screenScaling, -180 * screenScaling, Screen.width + 360 * screenScaling, Screen.height + 360 * screenScaling);
        shapesEntered = new bool[levelShapes.Length];

        shapesLeftText = GameObject.Find("shapesLeftVal").GetComponent<Text>();
        shapesLeftVal = (levelData[0]);
        shapesLeftText.text = "Shapes Left:   " + shapesLeftVal;
        shapesLeftStripped = shapesLeftText.text;

        scoreText = GameObject.Find("scoreVal").GetComponent<Text>();
        int scoreVal = 0;
        scoreText.text = "Score:   " +  scoreVal.ToString();
        scoreTextStripped = scoreText.text;
        scoreTextStripped.Remove(0, 9);

        //Image mainCanvas = GameObject.Find("MainCanvas").GetComponent<Image>();
    }

    void WinScene()
    {
        // Save Data
        if (levelWonBool == false)
        {
            int curLevel = GameManager.instance.curLevel;
            int curLevelHighscore = GameManager.instance.levelHighscores[curLevel - 1];
            int levelScore = int.Parse(scoreTextStripped.Remove(0, 9));
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
        CanvasGroup mainCanvasGroup = GameObject.Find("MainCanvas").GetComponent<CanvasGroup>();
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
        ffToggle.isOn = true;
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

    private void EscPressed()
    {
        if (levelWonBool == false && levelLostBool == false)
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
    }

    private void RPressed(string fromMenu)
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

	private void FPressed()
	{
		if (!(Time.timeScale == 0))
		{
			Sprite ffOff = Resources.Load<Sprite>("Sprites/Common/fastforward-white");
			Sprite ffOn = Resources.Load<Sprite>("Sprites/Common/fastforward-gray");
			Toggle ffToggle = GameObject.Find("fastForwardToggle").GetComponent<Toggle>();
			Image ffImg = GameObject.Find("fastForwardImage").GetComponent<Image>();

			if (ffToggle.isOn == true)
			{
				ffImg.sprite = ffOff;
				Time.timeScale = 1;
				ffToggle.isOn = false;
			}
			else
			{
				ffImg.sprite = ffOn;
				Time.timeScale = 2;
				ffToggle.isOn = true;
			}
		}
	}

	private void SetShapeVelocity(GameObject newObj, float startPosX, float startPosY, int shapeHits, int levelSpeed, int shapeDensity)
	{
		// Sets velocity
		float levelMultiplier = 0.4f + 0.2f * levelSpeed;
		float overallScaleFactor = 1 / (100.0f);
		float screenVelScaleFact = 1.0f / screenScaling;
		float shapeSizeSpeedMultiplier = 1 - Mathf.CeilToInt(newObj.transform.localScale.x) / 10;
		float shapeHitsSpeedMultiplier = (float)(1 / (1 + (shapeHits * 0.5)));
		float speedMultiplier = ((Random.Range(1000, 5000) / 20.0f) * levelMultiplier * shapeHitsSpeedMultiplier * shapeSizeSpeedMultiplier);
		float normalizeSpeedX = (Math.Abs((startPosX - Screen.width / 2) / (startPosY - Screen.height / 2)));
		float normalizeSpeedY = 1;
		float distFromCenterX = Random.Range(-Screen.width / 1000, Screen.width / 100); // no scaling
		float distFromCenterY = Random.Range(-Screen.height / 1000, Screen.height / 100); // no scaling
		float normalizeFactorX = -((startPosX - Screen.width / 2 + distFromCenterX) / Math.Abs(startPosX - Screen.width / 2) * (50.0f / shapeDensity));
		float normalizeFactorY = -((startPosY - Screen.height / 2 + distFromCenterY) / Math.Abs(startPosY - Screen.height / 2) * (50.0f / shapeDensity));
		float velocityX = normalizeSpeedX * normalizeFactorX * speedMultiplier * overallScaleFactor * screenScaling;
		float velocityY = normalizeSpeedY * normalizeFactorY * speedMultiplier * overallScaleFactor * screenScaling;

		while ((Math.Abs(velocityX) < 25 * screenScaling) || (Math.Abs(velocityY) < 25 * screenScaling))
		{
			velocityX += 2 * normalizeSpeedX * screenScaling * Math.Sign(velocityX);
			velocityY += 2 * normalizeSpeedY * screenScaling * Math.Sign(velocityY);
		}
		while ((Math.Abs(velocityX) > 300 * screenScaling) || (Math.Abs(velocityY) > 300 * screenScaling))
		{
			velocityX /= 2;
			velocityY /= 2;
		}

		//Do something for hexagons and triangles oscillation

		newObj.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(velocityX, velocityY);
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

				// Corrects for shapes not going through screen
				Rigidbody2D rb = levelShapes[i].GetComponent<Rigidbody2D>();
				Vector2 rbVel = rb.velocity;
				bool tooHigh = (rbVel.y > 0) && (shapePos.y > Screen.height * 1.1);
				bool tooLow = (rbVel.y < 0) && (shapePos.y < -Screen.height * 1.1);
				bool tooRight = (rbVel.x > 0) && (shapePos.x > Screen.width * 1.1);
				bool tooLeft = (rbVel.x < 0) && (shapePos.x < -Screen.width * 1.1);
				if (shapesEntered[i] == false && (tooHigh || tooLow || tooRight || tooLeft))
				{
					print("correcting shape");
					print(levelShapes[i].GetComponent<Shape>().shapeGeometry);
					SetShapeVelocity(levelShapes[i], levelShapes[i].transform.position.x, levelShapes[i].transform.position.y,
						levelShapes[i].GetComponent<Shape>().shapeHits, levelData[2], levelData[4]);
				}

				// Checks to see if life was lost
				string[] shapeGeometryList = new string[] { "lightning", "triangle", "hexagon" };
				bool specialObj = shapeGeometryList.Contains(levelShapes[i].GetComponent<Shape>().shapeGeometry);
				bool lifeLost = false;
				float shapeHeight = levelShapes[i].GetComponent<RectTransform>().rect.height;
				float shapeWidth = levelShapes[i].GetComponent<RectTransform>().rect.width;
				Rect exitRectNew = new Rect(exitRect.x - shapeWidth / 2, exitRect.y - shapeHeight / 2, exitRect.width + shapeWidth, exitRect.height + shapeHeight);
				Rect exitRectLargeNew = new Rect(exitRectLarge.x - shapeWidth / 2, exitRectLarge.y - shapeHeight / 2, exitRectLarge.width + shapeWidth, exitRectLarge.height + shapeHeight);
				if (!(exitRectNew.Contains(shapePos)) && shapesEntered[i] && specialObj == false)
                {
					lifeLost = true;
                }
				else if (!(exitRectLargeNew.Contains(shapePos)) && shapesEntered[i] && specialObj)
				{
					print(shapesEntered[i]);

					lifeLost = true;
				}

				if (lifeLost)
				{
					lives -= 1;
					int tempVal = int.Parse(shapesLeftText.text.Remove(0, 15));
					tempVal -= 1;
					shapesLeftText.text = "Shapes Left:   " + tempVal.ToString();
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

				// If last few shapes are far away, move closer
				if (int.Parse(shapesLeftText.text.Remove(0, 15)) < 13 && int.Parse(shapesLeftText.text.Remove(0, 15)) > 0)
				{
					Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
					float dist = Vector3.Distance(shapePos, screenCenter);
					if (dist > 2 * screenCenter.magnitude)
					{
						print("moving last object closer");
						levelShapes[i].transform.position = Vector3.MoveTowards(levelShapes[i].transform.position, screenCenter,
																			  screenCenter.magnitude);
					}
				}
			}
        }

		// Deals with points
		if (Time.timeScale == 2)
		{
			int scoreVal = int.Parse(scoreText.text.Remove(0, 9));
			scoreVal += 1;
			scoreText.text = "Score:   " + scoreVal.ToString();
		}

        // Wins the level.
        shapesLeftVal = int.Parse(shapesLeftText.text.Remove(0, 15));
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

		if ((Event.current.Equals(Event.KeyboardEvent("f"))) && (levelWonBool == false) && pauseMenuActive == false)
		{
			FPressed();
		}
    }
}
