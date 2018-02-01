using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;


public class ButtonMethods : MonoBehaviour
{

    public void DeactivateBtn()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowOptions()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }

    public void ShowOptionsInGame()
    {
        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();

        pauseCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }

    public void ShowAchievements()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas achievsCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = false;
        achievsCanvas.enabled = true;
    }

    public void ShowAchievsInGame()
    {
        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        Canvas achievsCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();

        pauseCanvas.enabled = false;
        achievsCanvas.enabled = true;
    }

    public void ShowHighscores()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas hsCanvas = GameObject.Find("HighscoresCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = false;
        hsCanvas.enabled = true;
    }

    public void ShowLevelSelect()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas levelselectCanvas = GameObject.Find("LevelSelectCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = false;
        levelselectCanvas.enabled = true;
    }

    public void ShowMainMenu()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();
        Canvas achievsCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();
        Canvas hsCanvas = GameObject.Find("HighscoresCanvas").GetComponent<Canvas>();
        Canvas levelselectCanvas = GameObject.Find("LevelSelectCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = true;
        optionsCanvas.enabled = false;
        achievsCanvas.enabled = false;
        hsCanvas.enabled = false;
        levelselectCanvas.enabled = false;
    }

    public void ShowPauseMenu()
    {
        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();
        Canvas achievsCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();

        pauseCanvas.enabled = true;
        optionsCanvas.enabled = false;
        achievsCanvas.enabled = false;
    }

    public void ResumeGame()
    {
        Canvas pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();
        Canvas achievsCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();

        pauseCanvas.enabled = false;
        optionsCanvas.enabled = false;
        achievsCanvas.enabled = false;

        Time.timeScale = 1;
        EventSystem.current.BroadcastMessage("UnPauseScene");
    }

    public void LoadMainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void LoadLevelSelectScene()
    {
        GameManager.instance.loadMenuWithLevelSelectActive = true;
        GameManager.instance.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ToggleAudio()
    {
        Sprite audioOffSpr = Resources.Load<Sprite>("Sprites/Common/audio-off-gray");
        Sprite audioOnSpr = Resources.Load<Sprite>("Sprites/Common/audio-on-white");
        Slider audioSlider = GameObject.Find("sefxSlider").GetComponent<Slider>();
        Toggle audioToggle = GameObject.Find("audioToggle").GetComponent<Toggle>();
        Image audioImg = GameObject.Find("audioImg").GetComponent<Image>();

        if (audioToggle.isOn == true)
        {
            audioImg.sprite = audioOffSpr;
            audioSlider.value = 0;
        }
        else
        {
            audioImg.sprite = audioOnSpr;
            audioSlider.value = 1;
        }

    }

    public void ToggleMusic()
    {
        Sprite musicOffSpr = Resources.Load<Sprite>("Sprites/Common/music-off-gray");
        Sprite musicOnSpr = Resources.Load<Sprite>("Sprites/Common/music-on-white");
        Slider musicSlider = GameObject.Find("musicSlider").GetComponent<Slider>();
        Toggle musicToggle = GameObject.Find("musicToggle").GetComponent<Toggle>();
        Image musicImg = GameObject.Find("musicImg").GetComponent<Image>();

        if (musicToggle.isOn == true)
        {
            musicImg.sprite = musicOffSpr;
            musicSlider.value = 0;
        }
        else
        {
            musicImg.sprite = musicOnSpr;
            musicSlider.value = 1;
        }
    }

    public void Retrylevel()
    {
        int levelNum = GameManager.instance.curLevel;

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

    public void NextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void LevelSelect(int level_num)
    {
        GameManager.instance.curLevel = level_num;
        GameManager.instance.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(2, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void AchievementsHoverIn(int achiev_num)
    {
        Text achiev_text = GameObject.Find("achievDisplayText").GetComponent<Text>();
        achiev_text.text = GameManager.instance.achiev_text[achiev_num];
    }

    public void AchievementsHoverOut()
    {
        Text achiev_text = GameObject.Find("achievDisplayText").GetComponent<Text>();
        int num_achievs = GameManager.instance.num_achievs_completed;
        achiev_text.text = "Completed Achievements:  " + num_achievs + "/12";
    }

    #region Debug Methods

    public void DebugMenu()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas optionsCanvas = GameObject.Find("OptionsCanvas").GetComponent<Canvas>();
        Canvas achievsCanvas = GameObject.Find("AchievementsCanvas").GetComponent<Canvas>();
        Canvas hsCanvas = GameObject.Find("HighscoresCanvas").GetComponent<Canvas>();
        Canvas levelselectCanvas = GameObject.Find("LevelSelectCanvas").GetComponent<Canvas>();
        Canvas debugCanvas = GameObject.Find("DebugCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = false;
        optionsCanvas.enabled = false;
        achievsCanvas.enabled = false;
        hsCanvas.enabled = false;
        levelselectCanvas.enabled = false;
        debugCanvas.enabled = true;
    }

    public void HideDebugMenu()
    {
        Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        Canvas debugCanvas = GameObject.Find("DebugCanvas").GetComponent<Canvas>();

        mainMenuCanvas.enabled = true;
        debugCanvas.enabled = false;
    }

    public void ResetAllData()
    {
        GameManager.instance.ResetData();

        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ResetLevels()
    {
        GameManager.instance.curLevel = 1;
        Array.Clear(GameManager.instance.levelsCompleted, 0, GameManager.instance.levelsCompleted.Length);
        Array.Clear(GameManager.instance.levelHighscores, 0, GameManager.instance.levelHighscores.Length);
        GameManager.instance.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
        
    public void OpenAllLevels()
    {
        GameManager.instance.levelsCompleted = Enumerable.Repeat(true, GameManager.instance.levelsCompleted.Length).ToArray();
        GameManager.instance.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    #endregion


}
