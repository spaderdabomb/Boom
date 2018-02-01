using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuEventSys : MonoBehaviour
{

    public EventSystem eventSystem;
    public GameObject selectedObject;

    private bool buttonSelected;

    void Start()
    {
        SetCanvas();
        SetButtons();
        SetAchievs();
    }

    void SetCanvas()
    {
        if (GameManager.instance.loadMenuWithLevelSelectActive)
        {
            Canvas mainMenuCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
            Canvas levelSelectCanvas = GameObject.Find("LevelSelectCanvas").GetComponent<Canvas>();

            mainMenuCanvas.enabled = false;
            levelSelectCanvas.enabled = true;

            GameManager.instance.loadMenuWithLevelSelectActive = false;
            GameManager.instance.Save();
        }
    }

    void SetButtons()
    {
        Button[] level_btns = GameObject.Find("LevelSelectPanel").GetComponentsInChildren<Button>(true);
        Text[] btn_text = GameObject.Find("LevelSelectPanel").GetComponentsInChildren<Text>(true);
        for (int i = 0; i < (GameManager.instance.levelsCompleted.Length); i++)
        {
            if (i == 0)
            {
                level_btns[0].interactable = true;
            }
            else
            {
                bool button_on = GameManager.instance.levelsCompleted[i - 1];
                level_btns[i].interactable = GameManager.instance.levelsCompleted[i];
                if (button_on)
                    btn_text[i].color = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 139.0f / 255.0f);
                else
                    btn_text[i].color = new Color(135.0f / 255.0f, 135.0f / 255.0f, 135.0f / 255.0f, 139.0f / 255.0f);
            }
        }
    }

    void SetAchievs()
    {
        Button[] achievs_boxes = GameObject.Find("AchievementsPanel").GetComponentsInChildren<Button>(true);
        Sprite achievs_gold = Resources.Load<Sprite>("Sprites/Boom/achievements-outline");
        Sprite achievs_gray = Resources.Load<Sprite>("Sprites/Boom/achievements-outline-gray");
        Text achiev_text = GameObject.Find("achievDisplayText").GetComponent<Text>();
        Color gold = new Color(255.0f / 255.0f, 255.0f / 255.0f, 165.0f / 255.0f, 255.0f / 255.0f);
        Color gray = new Color(165.0f / 255.0f, 165.0f / 255.0f, 165.0f / 255.0f, 255.0f / 255.0f);

        for (int i = 0; i < GameManager.instance.achievs_completed.Length; i++)
        {
            Image achiev_image = achievs_boxes[i].GetComponent<Image>();
            bool achiev_completed = GameManager.instance.achievs_completed[i];
            if (achiev_completed)
            {
                achiev_image.sprite = achievs_gold;
                achiev_image.color = gold;
            }
            else
            {
                achiev_image.sprite = achievs_gray;
                achiev_image.color = gray;
            }
        }

        int num_achievs = GameManager.instance.num_achievs_completed;
        achiev_text.text = "Completed Achievements:  " + num_achievs + "/12";
    }

    void Update()
    {
        if ((Input.GetAxisRaw("Vertical") != 0) && (buttonSelected == false))
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
    }

    private void OnDisable()
    {
        buttonSelected = false;
    }
}
