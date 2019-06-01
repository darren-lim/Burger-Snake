using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Snake_Player : MonoBehaviour
{

    // Grid postions on game area
    private Vector2Int gridPos;

    // Movement of diection on game area
    private Vector2Int moveDir;

    // How the accumulated time till movement
    private float gridMoveTimer;

    // How many second to wait before next move
    private float gridMoveTimerMax;

    public int snakeSpeed;


    // Start is called before the first frame update
    void Start()
    {
        gridPos = new Vector2Int(0,0);
        //Start moving up
        if (snakeSpeed <= 0)
        {
            snakeSpeed = 1;
        }
        moveDir = new Vector2Int(0,snakeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
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
        transform.position = new Vector3(gridPos.x,gridPos.y);
        if (moveDir.x != 0)
        {
            transform.eulerAngles = new Vector3(0,0,Mathf.Sign(moveDir.x)*-90);
        } else
        {
            transform.eulerAngles = new Vector3(0,0, Mathf.Sign(moveDir.y) < 0? 180: 0);
        }
    }

    private void HandleOutOfBounds(int windowXSize, int windowYSize, bool wrap)
    {
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

    private void AddBodyPart()
    {
        // Adds body parts when needed
    }
}
