using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour {

    public int shapeHits;
    public string shapeGeometry;

    private float shapeFrequency;
    private float teleportDist;
    private float blinkTime;
    private float hexFreq;
    private float hexAmp;
    private float hexMoveSpeed;
    private float triFreq;
    private float triAmp;
    private float triMoveSpeed;

    private int timeOffset;

    private bool dontMoveRight;
    private bool dontMoveUp;
    private bool dontMoveLeft;
    private bool dontMoveDown;

    public void SetupShape(int shapeHitsNew)
    {
        shapeHits = shapeHitsNew;
    }

    public void SetShapeFreq(float setFreq)
    {
        shapeFrequency = setFreq;
    }

    public void SetTriangleProperties(float freq, float amp, float moveSpeed)
    {
        triFreq = freq;
        triAmp = amp;
        triMoveSpeed = moveSpeed;
    }

    public void SetHexProperties(float freq, float amp, float moveSpeed)
    {
        hexFreq = freq;
        hexAmp = amp;
        hexMoveSpeed = moveSpeed;
    }

    public void SetShapeTeleport(float distance, int time)
    {
        teleportDist = distance;
        timeOffset = time;
    }

    public void SetShapeBlink(float objBlinkTime)
    {
        blinkTime = objBlinkTime;
    }

    void Update()
    {
        // If clicked.
        if (Input.GetMouseButtonDown(0))
        {
            
            if (Physics2D.OverlapCircle(Input.mousePosition, 0))
            {
                Collider2D myCollider = Physics2D.OverlapCircle(Input.mousePosition, 0);
                GameObject myObj = myCollider.transform.gameObject;
                Shape myShape = myCollider.GetComponentInParent<Shape>();

                if (GetInstanceID() == myShape.GetInstanceID())
                {
                    myShape.shapeHits -= 1;
                    if (myShape.shapeHits == 0)
                    {
                        Text shapesLeftText = GameObject.Find("shapesLeftVal").GetComponent<Text>();
                        int shapesLeftVal = int.Parse(shapesLeftText.text);
                        shapesLeftVal -= 1;
                        shapesLeftText.text = shapesLeftVal.ToString();

                        Text scoreText = GameObject.Find("scoreVal").GetComponent<Text>();
                        int scoreVal = int.Parse(scoreText.text);
                        scoreVal += 100;
                        scoreText.text = scoreVal.ToString();

                        Destroy(myObj);
                    }
                    else 
                    {
                        NewShapeColors(myObj, myShape.shapeHits);
                    }

                    RectTransform rt = myObj.GetComponent<RectTransform>();
                }
            }
        }

        // Update shape per frame.
        if ((int)Time.timeScale == 1)
        {
            int currTime;

            switch (this.shapeGeometry)
            {
                case "hexagon":
                    Vector3 pos = transform.position;
                    Vector3 axis = transform.right;
                    pos += transform.up * Time.deltaTime * hexMoveSpeed;
                    transform.position = pos + axis * Mathf.Sin(Time.time * hexFreq) * hexAmp;
                    break;
                case "triangle":
                    Vector3 posTri = transform.position;
                    Vector3 axisTri = transform.up;
                    posTri += transform.up * Time.deltaTime * triMoveSpeed;
                    transform.position = posTri + axisTri * Mathf.Sin(Time.time * triFreq) * triAmp;
                    break;
                case "star":
                    currTime = (int)(Time.time);
                    Image objImage = this.GetComponentInParent<Image>();

                    if ((currTime % 4) == 0)
                    {
                        Color objColor = objImage.color;
                        objImage.color = new Color(objColor.r, objColor.g, objColor.b, 0);
                    }
                    else if ((currTime % 4) == 2)
                    {
                        Color objColor = objImage.color;
                        objImage.color = new Color(objColor.r, objColor.g, objColor.b, 255);
                    }
                    break;
                case "star10":
                    RectTransform objRect = this.GetComponentInParent<RectTransform>();
                    float currScale = Mathf.Sin(Time.time * this.shapeFrequency);
                    objRect.localScale = new Vector3(currScale, currScale, 1);
                    break;
                case "arrow":
                    Image arrowImage = this.GetComponentInParent<Image>();
                    Color arrowColor = arrowImage.color;
                    float arrowOpacity = ((1 - Mathf.Sin(Time.time / blinkTime)) / 2);
                    Color newColor = new Color(arrowColor.r, arrowColor.g, arrowColor.b, arrowOpacity);
                    arrowImage.color = newColor;
                    break;
                case "lightning":
                    currTime = ((int)(Time.time));
                    Vector3 currPos = transform.position;
                    Vector3 translation;
                    int transformDir = (Time.frameCount + timeOffset) % 160;

                    if (transformDir == 30 && dontMoveRight == false)
                    {
                        translation = new Vector3(teleportDist, 0, 0);
                        transform.Translate(translation);
                        dontMoveRight = true;
                        dontMoveDown = false;
                    }
                    else if (transformDir == 70 && dontMoveUp == false)
                    {
                        translation = new Vector3(0, teleportDist, 0);
                        transform.Translate(translation);
                        dontMoveUp = true;
                    }
                    else if (transformDir == 110 && dontMoveLeft == false)
                    {
                        translation = new Vector3(-teleportDist, 0, 0);
                        transform.Translate(translation);
                        dontMoveLeft = true;
                    }
                    else if (transformDir == 150 && dontMoveDown == false)
                    {
                        translation = new Vector3(0, -teleportDist, 0);
                        transform.Translate(translation);
                        dontMoveDown = true;
                        dontMoveUp = false;
                        dontMoveLeft = false;
                        dontMoveRight = false;
                    }
                    break;
            }
        }
    }

    public void NewShapeColors(GameObject newObj, int shapeHits)
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
                shapeColor = new Color(225.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 180.0f / 255.0f);
                break;
        }

        newObj.transform.GetComponent<Image>().color = shapeColor;

    }

}
