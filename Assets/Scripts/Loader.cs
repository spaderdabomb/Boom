using UnityEngine;
using System.Collections;


public class Loader : MonoBehaviour 
{
	public GameObject game_manager;
	public LevelManager level_manager;

	void Awake ()
	{
		if (GameManager.instance == null)
		{
			Instantiate (game_manager);
		}

		if (LevelManager.level_manager == null)
		{
			Instantiate (level_manager);
		}

	}
}