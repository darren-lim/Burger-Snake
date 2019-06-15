using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;

public class S_Snake_Player : MonoBehaviourPun
{

    // Grid postions on game area
    private Vector2Int gridPos;
    private Vector2Int levelSize;
    private bool wrapAround;

    // Movement of diection on game area
    private Vector2Int moveDir;

    private int snakeSpeed;
    private GameObject partsHolder;
    public List<GameObject> bodyParts;
    private bool selfIntersect;
    public uint points;

    //Networking
    private PhotonView PV;

    private float simulationRate = 0.2f;
    private float simulationTimer;

    float timer = 3f;
    public Text myScoreboard;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        gridPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        //Start moving up
        if (snakeSpeed <= 0)
        {
            snakeSpeed = 1;
        }
        partsHolder = new GameObject(this.name + "\'s Holder");
        partsHolder.transform.position = new Vector3(0,0);//Vector3(gridPos.x, gridPos.y);
        // partsHolder.transform.parent = this.transform.parent.transform;
        moveDir = new Vector2Int(0, snakeSpeed);
        bodyParts = new List<GameObject>();
        points = 0;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer--;
        }
    }

    void FixedUpdate()
    {
        simulationTimer += Time.deltaTime;
        if (simulationTimer >= simulationRate)
        {
            HandleMovement();
            simulationTimer -= simulationRate;
        }
        HandleInput();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            PV.RPC("SetFoodInactive", RpcTarget.All, other.gameObject.GetComponent<PhotonView>().ViewID);
            //SetFoodInactive(other.gameObject);
            //other.gameObject.SetActive(false); // NEED TO MAKE THIS RPC FUNCTION
            AddBodyPart();
            PV.RPC("RPC_addPoints", RpcTarget.All,1.0f);
        }
        // //Check other snake collison
        else if (!other.gameObject.CompareTag(tag) && timer <= 0)
        {
            // Destroy all parts and start over
            // Add point to other, subtract point from self
            if (bodyParts.Count > 0)
            {
                timer = 3f;
                RemoveBodyPart();
            }
            // // head to body Collided
            // if (other.transform.parent != null)
            // {
            //     other.transform.parent.GetComponent<S_Snake_Player>().addPoints();
            // }
            // // head to head Collided
            // else
            // {
            //     other.GetComponent<S_Snake_Player>().addPoints();
            // }
            if (points > 0)
            {
                PV.RPC("RPC_subPoints", RpcTarget.All,1.0f);
            }
        }
    }

    [PunRPC]
    public void SetFoodInactive(int food)
    {
        PhotonView Disable = PhotonView.Find(food);
        Disable.transform.gameObject.SetActive(false);
    }

    private void HandleInput()
    {
        // Handles input of the user
        // MUST take only local inputs
        if (PV.IsMine)
        {
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
    }

    public void HandleMovement()
    {
        // Debug.Log("Grid Pos: " + gridPos.ToString());
        // Debug.Log("Move Dir: " + moveDir.ToString());
        gridPos += moveDir;
        HandleOutOfBounds(levelSize.x, levelSize.y, wrapAround);
        for (int i = bodyParts.Count - 1; i > 0; --i)
        {
            bodyParts[i].transform.position = new Vector3(bodyParts[i - 1].transform.position.x, bodyParts[i - 1].transform.position.y);
        }
        // The "first" body part takes its next value from current head postion
        if (bodyParts.Count >= 1)
        {
            bodyParts[0].transform.position = new Vector3(this.transform.position.x, this.transform.position.y);
        }
        transform.position = new Vector3(gridPos.x, gridPos.y);
        if (moveDir.x != 0)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Sign(moveDir.x) * -90);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Sign(moveDir.y) < 0 ? 180 : 0);
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

    void AddBodyPart()
    {
        GameObject body;
        // Adds body parts when needed
        if (bodyParts.Count == 0)
        {
            // Take the postion of the head
            body = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Snake_Body"),
                                            new Vector3(this.transform.position.x, this.transform.position.y),
                                            Quaternion.identity, 0);
        }
        else
        {
            // Take the postion of the current last body part
            body = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Snake_Body"),
                                            new Vector3(bodyParts[bodyParts.Count - 1].transform.position.x, bodyParts[bodyParts.Count - 1].transform.position.y),
                                            Quaternion.identity, 0);
        }
        body.transform.parent = partsHolder.transform;
        body.AddComponent<PhotonView>();
        body.tag = this.tag;
        bodyParts.Add(body);
    }

    void RemoveBodyPart()
    {

        if (bodyParts.Count > 0)
        {
            //Destroy Last body
            //PhotonNetwork.RemoveRPCs(bodyParts[bodyParts.Count - 1].GetComponent<PhotonView>().ViewID);
            PhotonNetwork.Destroy(bodyParts[bodyParts.Count - 1]);
            //Destroy(bodyParts[bodyParts.Count-1]);
            //Remove null pointer
            bodyParts.Remove(bodyParts[bodyParts.Count - 1]);
        }
    }

    public bool getSelfIntersect()
    {
        return selfIntersect;
    }

    public void setSelfIntersect(bool canSelfInter)
    {
        selfIntersect = canSelfInter;
    }

    // Add a point with multiplier
    [PunRPC]
    void RPC_addPoints(float mult = 1.0f)
    {
        points += (uint)(1 * mult);
        myScoreboard.text = myScoreboard.text.Substring(0,9) + points.ToString();
    }

    // Subtract a point with multiplier
    [PunRPC]
    void RPC_subPoints(float mult = 1.0f)
    {
        points -= (uint)(1 * mult);
        myScoreboard.text = myScoreboard.text.Substring(0,9) + points.ToString();
    }

    [PunRPC]
    void RPC_textInitialize()
    {
        myScoreboard.text = myScoreboard.text.Substring(0,9) + points.ToString();
    }

    public void SetSizeWrap(Vector2Int size, bool wrap)
    {
        levelSize = size;
        wrapAround = wrap;
    }
}
