using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{

    public static LevelManager level_manager = null;

    private int[] levelSettings;

    private string[] shapeArr;

    private float screenScaling;
    private float totalSpeedX;

    public GameObject circleWhite;

    void Awake()
    {
        if (level_manager == null)
            level_manager = this;
        else if (level_manager != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        screenScaling = Screen.width / 808.0f;
    }
 

    public void InitLevel(int levelNum, out GameObject[] shapeHolder, out int[] levelData, out int[] allShapeHits)
    {
        GameManager.instance.curLevel = levelNum;
        GameManager.instance.Save();
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        LevelManager.level_manager.LevelSettings(levelNum, out levelData, out shapeArr);
        shapeHolder = new GameObject[levelData[0]];
        allShapeHits = new int[levelData[0]];
        int totalShapeHits = levelData[1];
        float avgShapeHits = totalShapeHits / levelData[0];

        // Initializes each shape with various properties.
        totalSpeedX = 0;
        for (int i = 0; i < levelData[0]; i++)
        {
            GameObject newObj = MakeShapeObj(mainCanvas, i, levelData[3]);

            // Randomly assigns position to objects.
            float waveMultiplier = (float)Math.Round(Random.Range(1.0f, (float)levelData[6]));
            float startPosX = screenScaling * (2 * waveMultiplier * (Screen.width / 2 + Random.Range(500.0f, 1200.0f + (100.0f * levelData[4])) * (Random.Range(0, 2) * 2 - 1)));
            float startPosY = screenScaling * (2 * waveMultiplier * (Screen.height / 2 + Random.Range(400.0f, 700.0f + (100.0f * levelData[4])) * (Random.Range(0, 2) * 2 - 1)));
            Vector3 pos = new Vector3(startPosX, startPosY, 0);
            newObj.transform.position = pos;
			print(newObj.GetComponent<RectTransform>().position.z);

            allShapeHits[i] = SetShapeHits(newObj, totalShapeHits, levelData, allShapeHits[i], avgShapeHits, i);
            SetShapeScale(newObj, levelData);
            //SetShapeCollider(newObj);
            SetShapeColors(newObj, allShapeHits[i]);
            SetShapeVelocity(newObj, startPosX, startPosY, allShapeHits[i], levelData[2], levelData[4]);
			FinishingShapeTouches(newObj, levelData[3], i);

            shapeHolder[i] = newObj;
        }
    }

    private GameObject MakeShapeObj(GameObject mainCanvas, int index, int levelDifficulty)
    {
        // Instantiates object in scene with class component "Shape".
        GameObject newObj = new GameObject();
        switch (shapeArr[index])
        {
            case "circle":
                newObj = Instantiate(Resources.Load("Prefabs/circleWhite")) as GameObject;
                break;
            case "square":
                newObj = Instantiate(Resources.Load("Prefabs/squareWhite")) as GameObject;
                break;
            case "rect":
                newObj = Instantiate(Resources.Load("Prefabs/rectWhite")) as GameObject;
                break;
            case "diamond":
                newObj = Instantiate(Resources.Load("Prefabs/diamondWhite")) as GameObject;
                break;
            case "triangle":
                newObj = Instantiate(Resources.Load("Prefabs/triangleWhite")) as GameObject;
                break;
            case "hexagon":
                newObj = Instantiate(Resources.Load("Prefabs/hexagonWhite")) as GameObject;
                break;
            case "star":
                newObj = Instantiate(Resources.Load("Prefabs/starWhite")) as GameObject;
                break;
            case "ring":
                newObj = Instantiate(Resources.Load("Prefabs/ringWhite")) as GameObject;
                break;
            case "star10":
                newObj = Instantiate(Resources.Load("Prefabs/star10White")) as GameObject;
                break;
            case "arrow":
                newObj = Instantiate(Resources.Load("Prefabs/arrowWhite")) as GameObject;
                break;
            case "lightning":
                newObj = Instantiate(Resources.Load("Prefabs/lightningWhite")) as GameObject;
                break;
        }



        newObj.transform.SetParent(mainCanvas.transform);

        return newObj;
    }

    private int SetShapeHits(GameObject newObj, int totalShapeHits, int[] levelData, int tempShapeHits, float avgShapeHits, int index)
    {
        // Determines how many hits shape has.
        int shapeHits = 1;
        if (totalShapeHits > (levelData[0] - index))
            shapeHits = (int)Math.Round(Random.Range(1000, avgShapeHits * 1000 + 499) / 1000);
        else
            shapeHits = 1;

        tempShapeHits = shapeHits;
        totalShapeHits -= shapeHits;
        Shape shapeObj = newObj.GetComponent<Shape>();
        shapeObj.SetupShape(shapeHits);

        return tempShapeHits;
    }

    private void SetShapeScale(GameObject newObj, int[] levelData)
    {
        // Adjusts Scale (levelData[5] can range from 0-4)
        float randomScale = 1 + levelData[5] * ((Random.Range(0, 50)) / 40);
		if (newObj.GetComponent<Shape>().shapeGeometry == "lightning" && randomScale < 1.2)
		{
			randomScale += 0.2f;
		}
        newObj.transform.localScale = new Vector3(randomScale, randomScale, 1);
    }

    private void SetShapeCollider(GameObject newObj)
    {
        // Adjusts Object Collider 
        BoxCollider2D objCollider = newObj.GetComponent<BoxCollider2D>();
        PolygonCollider2D polyCollider = newObj.GetComponent<PolygonCollider2D>();
        Shape shapeObj = newObj.GetComponent<Shape>();
        RectTransform objRect = newObj.GetComponent<RectTransform>();
        float objWidth = objRect.rect.width;
        float objHeight = objRect.rect.height;

        switch (shapeObj.shapeGeometry)
        {
            case "circle":
                objCollider.size = new Vector2(1, 1);
                objCollider.edgeRadius = 1 + (objWidth * newObj.transform.localScale.x) / 4;
                break;
            case "square":
                objCollider.size = new Vector2(objWidth * newObj.transform.localScale.x + 5,
                                    objHeight * newObj.transform.localScale.y + 5);
                objCollider.edgeRadius = 0;
                break;
            case "rect":
                objCollider.size = new Vector2(objWidth * newObj.transform.localScale.x + 5,
                                    objHeight * newObj.transform.localScale.y + 5);
                objCollider.edgeRadius = 0;
                break;
            case "diamond":
                polyCollider = newObj.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
            case "triangle":
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
            case "hexagon":
                polyCollider = newObj.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
            case "star":
                polyCollider = newObj.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
            case "ring":
                polyCollider = newObj.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
            case "star10":
                objCollider.size = new Vector2(1, 1);
                objCollider.edgeRadius = 5 + (objWidth * newObj.transform.localScale.x) / 4;
                break;
            case "arrow":
                polyCollider = newObj.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
            case "lightning":
                polyCollider = newObj.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < polyCollider.GetTotalPointCount(); i++)
                {
                    polyCollider.points[i].x *= newObj.transform.localScale.x;
                    polyCollider.points[i].y *= newObj.transform.localScale.y;
                }
                break;
        }
    }

    private void SetShapeVelocity(GameObject newObj, float startPosX, float startPosY, int shapeHits, int levelSpeed, int shapeDensity)
    {
        // Sets velocity.
        float levelMultiplier = 0.4f + 0.2f * levelSpeed;
        float overallScaleFactor = 1 / (100.0f);
        float screenVelScaleFact = 1.0f / screenScaling;
        float shapeSizeSpeedMultiplier = 1 - Mathf.CeilToInt(newObj.transform.localScale.x) / 10;
        float shapeHitsSpeedMultiplier = (float)(1 / (1 + (shapeHits * 0.5)));
        float speedMultiplier = ((Random.Range(1000, 5000) / 20.0f) * levelMultiplier * shapeHitsSpeedMultiplier * shapeSizeSpeedMultiplier);
        float normalizeSpeedX = (Math.Abs((startPosX - Screen.width / 2) / (startPosY - Screen.height / 2)));
        float normalizeSpeedY = 1;
		float distFromCenterX = 0;//Random.Range(-Screen.width / 1000, Screen.width / 100); // no scaling
		float distFromCenterY = 0;//Random.Range(-Screen.height / 1000, Screen.height / 100); // no scaling
        float normalizeFactorX = -((startPosX - Screen.width / 2 + distFromCenterX) / Math.Abs(startPosX - Screen.width / 2) * (50.0f / shapeDensity));
        float normalizeFactorY = -((startPosY - Screen.height / 2 + distFromCenterY) / Math.Abs(startPosY - Screen.height / 2) * (50.0f / shapeDensity));
        float velocityX = normalizeSpeedX * normalizeFactorX * speedMultiplier * overallScaleFactor * screenScaling;
        float velocityY = normalizeSpeedY * normalizeFactorY * speedMultiplier * overallScaleFactor * screenScaling;

        while ((Math.Abs(velocityX) < 25 * screenScaling) || (Math.Abs(velocityY) < 25 * screenScaling))
        {
			velocityX *= 2; // this didn't work: 2 * normalizeSpeedX * screenScaling * Math.Sign(velocityX);
			velocityY *= 2;
        }
        while ((Math.Abs(velocityX) > 300 * screenScaling) || (Math.Abs(velocityY) > 300 * screenScaling))
        {
            velocityX /= 2;
            velocityY /= 2;
        }

        newObj.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(velocityX, velocityY);
    }



    private void SetShapeColors(GameObject newObj, int shapeHits)
    {
        Color shapeColor = new Color(0, 0, 0, 0);

        switch (shapeHits)
        {
            case 1:
                shapeColor = new Color(255.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 2:
                shapeColor = new Color(0.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 3:
                shapeColor = new Color(0.0f / 255.0f, 77.0f / 255.0f, 255.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 4:
                shapeColor = new Color(0.0f / 255.0f, 255.0f / 255.0f, 81.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 5:
                shapeColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 33.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 6:
                shapeColor = new Color(255.0f / 255.0f, 139.0f / 255.0f, 33.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 7:
                shapeColor = new Color(225.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 180.0f / 255.0f);
                break;
            case 8:
                shapeColor = new Color(30.0f / 255.0f, 30.0f / 255.0f, 30.0f / 255.0f, 180.0f / 255.0f);
                break;
        }

        newObj.transform.GetComponent<Image>().color = shapeColor;
    }

    private void FinishingShapeTouches(GameObject newObj, float levelDifficulty, int index)
    {
		// Redistributes shapes to come in more evenly.
		Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
		float velMag = (float)Math.Sqrt(Math.Pow(rb.velocity.x, 2) + Math.Pow(rb.velocity.y, 2));
		Vector3 currPos = newObj.GetComponent<RectTransform>().position;
		Vector3 screen = new Vector3(Screen.width / 2, Screen.height / 2, 0);

		// Redistributes slow shapes.
		for (int i = 50; i > 2; i--)
		{
			if (velMag < 95 && ((Math.Abs(currPos.x - Screen.width / 2) > Screen.width * i + 100) || (Math.Abs(currPos.y - Screen.height / 2) > Screen.height * i + 100)))
			{
				newObj.transform.position = Vector3.MoveTowards(newObj.transform.position, screen, screen.magnitude * ((float)(i - 1.0f)));
				break;
			}
		}
		

		//Redistributes fast shapes.
		Vector3 reflectVect;
		if (velMag > 210 && ((Math.Abs(currPos.x - Screen.width / 2) < Screen.width * 2) || (Math.Abs(currPos.y - Screen.height / 2) < Screen.height * 2)))
		{
			reflectVect = Vector3.MoveTowards(newObj.transform.position, screen, screen.magnitude * 9) * -1;
			newObj.transform.position = reflectVect;
		}
		else if (velMag > 225 && ((Math.Abs(currPos.x - Screen.width / 2) < Screen.width * 2) || (Math.Abs(currPos.y - Screen.height / 2) < Screen.height * 2)))
		{
			reflectVect = Vector3.MoveTowards(newObj.transform.position, screen, screen.magnitude * 12) * -1;
			newObj.transform.position = reflectVect;
		}
		else if (velMag > 240 && ((Math.Abs(currPos.x - Screen.width / 2) < Screen.width * 2) || (Math.Abs(currPos.y - Screen.height / 2) < Screen.height * 2)))
		{
			reflectVect = Vector3.MoveTowards(newObj.transform.position, screen, screen.magnitude * 15) * -1;
			newObj.transform.position = reflectVect;
		}

		switch (shapeArr[index])
		{
			case "rect":
				newObj.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(50, 500);
				newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 100);
				break;
			case "triangle":
				newObj.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(50, 500);
				newObj.GetComponent<Shape>().SetTriangleProperties(Random.Range(8.0f, 14.0f), Random.Range(4.0f, 8.0f), Random.Range(10.0f, 16.0f), (float)levelDifficulty);
				break;
			case "hexagon":
				newObj.GetComponent<Shape>().SetHexProperties(Random.Range(6.0f, 12.0f), Random.Range(6.0f, 10.0f), Random.Range(4.0f, 8.0f), (float)levelDifficulty);
				break;
			case "star":
				newObj.GetComponent<Rigidbody2D>().angularVelocity = 50;
				break;
			case "star10":
				newObj.GetComponent<Rigidbody2D>().angularVelocity = 500.0f;
				newObj.GetComponent<Shape>().SetShapeFreq(Random.Range(5, 40) / 10.0f);
				break;
			case "arrow":
				newObj.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(400, 500);
				newObj.GetComponent<Shape>().SetShapeBlink(Random.Range(10, 30) / 40.0f);
				break;
			case "lightning":
				newObj.GetComponent<Shape>().SetShapeTeleport(Random.Range(100, 160), (int)(Random.Range(0, 240)));
				break;
		}
	}

	void Pause()
    {
        Time.timeScale = 0;
    }

    void Resume()
    {
        Time.timeScale = 1;
    }

    void Update()
    {

    }

    public void LevelSettings(int levelNum, out int[] levelSettings, out string[] shapeArr)
    {
        // Creates array of the form
        //		levelSettings(numShapes, totalClicks, levelDifficulty, speedDifficulty,
        //					  shapeDensity, shapeScaling, numWaves)

        levelSettings = null;
        shapeArr = null;
        switch (levelNum)
        {
            case 1:
                levelSettings = new int[] { 24, 24, 1, 1, 3, 0, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle"};
                break;
            case 2:
                levelSettings = new int[] { 30, 60, 1, 1, 4, 0, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle"};
                break;
            case 3:
                levelSettings = new int[] { 25, 75, 1, 1, 2, 1, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle"};
                break;
            case 4:
                levelSettings = new int[] { 50, 100, 5, 1, 16, 1, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle"};
                break;
            case 5:
                levelSettings = new int[] { 40, 120, 3, 1, 13, 1, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "square", "square",
                                      "square", "square", "square", "square", "square", "square",
                                      "square", "square", "square", "square", "square", "square",
                                      "square", "square", "square", "square" };
                break;
            case 6:
                levelSettings = new int[] { 60, 180, 2, 1, 5, 1, 3 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "square", "square", "square", "square", "square", "square",
                                      "square", "square", "square", "square", "square", "square",
                                      "square", "square", "square", "square", "square", "square",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "rect", "rect", "rect", "rect", "rect", "rect"};
                break;
            case 7:
                levelSettings = new int[] { 40, 120, 5, 2, 7, 1, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "square", "square", "square", "square", "square", "square",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square" };
                break;
            case 8:
                levelSettings = new int[] { 54, 240, 4, 2, 28, 2, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "square", "square", "square", "square", "square", "square",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon"};

                break;
            case 9:
                levelSettings = new int[] { 72, 72, 4, 3, 30, 3, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "square", "square", "square", "square", "square", "square",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle"};
                break;
            case 10:
                levelSettings = new int[] { 30, 180, 1, 1, 10, 2, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "ring", "ring", "ring", "ring", "ring", "ring",
                                      "ring", "ring", "ring", "ring", "ring", "ring"};
                break;
            case 11:
                levelSettings = new int[] { 72, 144, 6, 1, 30, 2, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "square", "square", "square", "square", "square", "square",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "ring", "ring", "ring", "ring", "ring", "ring",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "star", "star", "star", "star", "star", "star",
                                      "star", "star", "star", "star", "star", "star",
                                      "star", "star", "star", "star", "star", "star"};
                break;
            case 12:
                levelSettings = new int[] { 84, 504, 1, 1, 60, 2, 1 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle"};
                break;
            case 13:
                levelSettings = new int[] { 102, 306, 2, 1, 60, 3, 2 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "square", "square", "square", "square", "square", "square",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle"};
                break;
            case 14:
                levelSettings = new int[] { 90, 360, 8, 1, 20, 3, 4 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "square", "square", "square", "square", "square", "square",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow"};
                break;

            case 15:
                levelSettings = new int[] { 126, 126, 4, 1, 10, 1, 3 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
									  "circle", "circle", "circle", "circle", "circle", "circle",
									  "circle", "circle", "circle", "circle", "circle", "circle",
									  "circle", "circle", "circle", "circle", "circle", "circle",
									  "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "square", "square", "square", "square", "square", "square",
									  "square", "square", "square", "square", "square", "square",
									  "square", "square", "square", "square", "square", "square",
									  "rect", "rect", "rect", "rect", "rect", "rect",
									  "rect", "rect", "rect", "rect", "rect", "rect",
									  "rect", "rect", "rect", "rect", "rect", "rect",
									  "rect", "rect", "rect", "rect", "rect", "rect",
									  "square", "square", "square", "square", "square", "square",
									  "square", "square", "square", "square", "square", "square",
									  "square", "square", "square", "square", "square", "square",
									  "square", "square", "square", "square", "square", "square",
									  "square", "square", "square", "square", "square", "square"};
                break;
            case 16:
                levelSettings = new int[] { 66, 462, 1, 1, 1, 1, 4 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle"};
                break;
            case 17:
                levelSettings = new int[] { 90, 180, 4, 5, 30, 4, 3 };
                shapeArr = new string[] {"triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon" };
                break;
            case 18:
                levelSettings = new int[] { 96, 72, 30, 1, 60, 4, 1 };
                shapeArr = new string[] {"star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "star10", "star10", "star10", "star10", "star10", "star10"};
                break;
            case 19:
                levelSettings = new int[] { 114, 228, 2, 1, 30, 4, 4 };
                shapeArr = new string[] {"lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "star", "star", "star", "star", "star", "star",
									  "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning"};
                break;
            case 20:
                levelSettings = new int[] { 144, 1152, 1, 4, 60, 3, 3 };
                shapeArr = new string[] {"circle", "circle", "circle", "circle", "circle", "circle",
                                      "circle", "circle", "circle", "circle", "circle", "circle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "triangle", "triangle", "triangle", "triangle", "triangle", "triangle",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "rect", "rect", "rect", "rect", "rect", "rect",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "diamond", "diamond", "diamond", "diamond", "diamond", "diamond",
                                      "square", "square", "square", "square", "square", "square",
                                      "square", "square", "square", "square", "square", "square",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "hexagon", "hexagon", "hexagon", "hexagon", "hexagon", "hexagon",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "lightning", "lightning", "lightning", "lightning", "lightning", "lightning",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star10", "star10", "star10", "star10", "star10", "star10",
                                      "star", "star", "star", "star", "star", "star",
                                      "star", "star", "star", "star", "star", "star",
                                      "ring", "ring", "ring", "ring", "ring", "ring",
                                      "ring", "ring", "ring", "ring", "ring", "ring",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow",
                                      "arrow", "arrow", "arrow", "arrow", "arrow", "arrow"};
                break;
        }
    }
}
