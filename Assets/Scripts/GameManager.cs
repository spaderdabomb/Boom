using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{

	public static GameManager instance = null;   

	public int num_achievs_completed;
	public int curLevel;
    public int[] levelHighscores;

    public bool loadMenuWithLevelSelectActive;
	public bool[] achievs_completed;
    public bool[] levelsCompleted = new bool[20];

	public string[] achiev_text;


	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    

		DontDestroyOnLoad(gameObject);

		InitGame();
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
		PlayerData data = new PlayerData ();

		// Data
		data.num_achievs_completed = num_achievs_completed;
		data.curLevel = curLevel;
        data.levelHighscores = levelHighscores;

        data.loadMenuWithLevelSelectActive = loadMenuWithLevelSelectActive;
		data.achievs_completed = achievs_completed;
        data.levelsCompleted = levelsCompleted;

		data.achiev_text = achiev_text;


		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			// Data
			num_achievs_completed = data.num_achievs_completed;
			curLevel = data.curLevel;
            levelHighscores = data.levelHighscores;

            loadMenuWithLevelSelectActive = data.loadMenuWithLevelSelectActive;
			achievs_completed = data.achievs_completed;
            levelsCompleted = data.levelsCompleted;

			achiev_text = data.achiev_text;
		}

	}

    public void ResetData()
    {
        num_achievs_completed = 0;
        curLevel = 1;
        Array.Clear(levelHighscores, 0, levelHighscores.Length);

        loadMenuWithLevelSelectActive = false;
        Array.Clear(achievs_completed, 0, achievs_completed.Length);
        Array.Clear(levelsCompleted, 0, levelsCompleted.Length);

        //achiev_text; 

        GameManager.instance.Save();
    }

	void InitGame()
	{
		GameManager.instance.Load ();
		GameManager.instance.Save ();
	}


	void Update()
	{

	}
}

[Serializable]
class PlayerData
{
	public int num_achievs_completed;
	public int curLevel;
    public int[] levelHighscores;

    public bool loadMenuWithLevelSelectActive;
	public bool[] achievs_completed;
    public bool[] levelsCompleted = new bool[20];

	public string[] achiev_text;
}
