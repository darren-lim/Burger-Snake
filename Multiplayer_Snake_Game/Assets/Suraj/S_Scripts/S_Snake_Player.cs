using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Snake_Player : MonoBehaviour
{

    // Grid postions on game area
    private Vector2Int gridPos;

    // Movement of diection on game area
    private Vector2Int moveDir;

    private int snakeSpeed;
    private GameObject partsHolder;
    private List<GameObject> bodyParts;
    private bool selfIntersect;

    private uint points; 

    void Start()
    {
        gridPos = new Vector2Int(0,0);
        //Start moving up
        if (snakeSpeed <= 0)
        {
            snakeSpeed = 1;
        }
        partsHolder = new GameObject(this.name + "\'s Holder");
        partsHolder.transform.position = new Vector3(0,0);
        moveDir = new Vector2Int(0,snakeSpeed);
        bodyParts = new List<GameObject>();
        points = 0;
    }

    void Update()
    {
        HandleInput();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log(other.gameObject.transform.parent);
        // Debug.Log(partsHolder.transform);
        // Debug.Log(partsHolder);
        if(other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            AddBodyPart();
        }
        // Currently does not work correctly
        else if (!selfIntersect && other.gameObject.transform.parent == partsHolder.transform)
        {
            // Destroy all parts and start over
            bodyParts.Clear();
            transform.position = new Vector3(0,0);
        }
        else if (other.gameObject.CompareTag("Body"))
        {
            // Check self intersection
            if (!selfIntersect && other.gameObject.transform.parent == partsHolder.transform)
            {
                // Destroy all parts and start over
                // Subtract point from self
                bodyParts.Clear();
                transform.position = new Vector3(0,0);
                if (points > 0)
                {
                    subPoints();
                }
            }
        }
        //Check other snake collison
            else
            {
                // Destroy all parts and start over
                // Add point to other, subtract point from self
                bodyParts.Clear();
                transform.position = new Vector3(0,0);
                other.transform.parent.GetComponent<S_Snake_Player>().addPoints();
                if (points > 0)
                {
                    subPoints();
                }
            }
        
    }

    private void HandleInput ()
    {
        // Handles input of the user
        // MUST take only local inputs 
        if (Input.GetKeyDown(KeyCode.W) && moveDir.y == 0)
        {
            moveDir.x = 0;
            moveDir.y = +snakeSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.S) && moveDir.y == 0)
        {
            moveDir.x = 0;
            moveDir.y = -snakeSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.A) && moveDir.x == 0)
        {
            moveDir.x = -snakeSpeed;
            moveDir.y = 0;
        }
        else if (Input.GetKeyDown(KeyCode.D) && moveDir.x == 0)
        {
            moveDir.x = +snakeSpeed;
            moveDir.y = 0;
        }
    }

    public void HandleMovement(Vector2Int windowSize, bool wrapAround)
    {
        gridPos += moveDir;
        HandleOutOfBounds(windowSize.x, windowSize.y, wrapAround);
        for(int i = bodyParts.Count-1; i>0;--i)
        {
            bodyParts[i].transform.position = new Vector3(bodyParts[i-1].transform.position.x,bodyParts[i-1].transform.position.y);
        }
        // The "first" body part takes its next value from current head postion
        if (bodyParts.Count >= 1)
        {
            bodyParts[0].transform.position = new Vector3(this.transform.position.x,this.transform.position.y);
        }
        transform.position = new Vector3(gridPos.x,gridPos.y);
        if (moveDir.x != 0)
        {
            transform.eulerAngles = new Vector3(0,0,Mathf.Sign(moveDir.x)*-90);
        } else
        {
            transform.eulerAngles = new Vector3(0,0, Mathf.Sign(moveDir.y) < 0? 180: 0);
        }
    }

    private void HandleOutOfBounds(int windowXSize, int windowYSize, bool canWrap)
    {
        if (canWrap)
        {
            // Wrap around i.e. Go to the other side
           if (gridPos.x >= windowXSize || gridPos.x <= -windowXSize)
            {
                gridPos.x = -gridPos.x;
            }
            if (gridPos.y >= windowYSize || gridPos.y <= -windowYSize)
            {
                gridPos.y = -gridPos.y;
            }
        }
        else
        {
            // No wrap around level
            if (gridPos.x >= windowXSize || gridPos.x <= -windowXSize)
            {
                // Currently Reset to 0 point on x position
                gridPos.x = 0;
            }
            if (gridPos.y >= windowYSize || gridPos.y <= -windowYSize)
            {
                // Currently Reset to 0 point on y position
                gridPos.y = 0;
            }
        }
    }

    private void AddBodyPart()
    {
        // Adds body parts when needed
        GameObject body = Instantiate(GameAssets.instance.snakeBodyPrefab);
        body.transform.parent = partsHolder.transform;
        if (bodyParts.Count == 0)
        {
            // Take the postion of the head
            body.transform.position = new Vector3(this.transform.position.x,this.transform.position.y);
        }
        else
        {
            // Take the postion of the current last body part
            body.transform.position = new Vector3(bodyParts[bodyParts.Count-1].transform.position.x,bodyParts[bodyParts.Count-1].transform.position.y);
        }
        body.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
        body.GetComponent<PolygonCollider2D>().isTrigger = true;
        body.tag = "Body";
        bodyParts.Add(body);
    }

    public bool getSelfIntersect()
    {
        return selfIntersect;
    }

    public void setSelfIntersect(bool canSelfInter)
    {
        selfIntersect = canSelfInter;
    }

    public uint getPoints()
    {
        return points;
    }

    // Add a point with multiplier
    public void addPoints(float mult = 1.0f)
    {
        points += (uint)(1*mult);
    }

    // Subtract a point with multiplier
    public void subPoints(float mult = 1.0f)
    {
        points -= (uint)(1*mult);
    }
}
