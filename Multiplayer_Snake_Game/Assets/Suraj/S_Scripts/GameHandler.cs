using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Level-focused Details
    private GameObject levelBack;
    private S_LevelManager levelGrid;
    [SerializeField]
    private Vector2Int levelSize;
    private float refreshRate = 10.0f;
    private float refreshTimer;
    private float simulationRate = 0.03f;
    private float simulationTimer;

    // Player-focused Details
    private int playerCount;
    private GameObject[] snakeControllers;


    // Options for gameplay - CAN BE EXPANDED LATER
    private int numberOfFood = 10;
    private bool wrapAround = true;
    private bool selfCollision = false;
    
    void Start()
    {
        levelBack = GameObject.FindGameObjectWithTag("Background");
        levelSize = new Vector2Int(2*(int)levelBack.transform.localScale.x, 2*(int)levelBack.transform.localScale.y);
        levelGrid = new S_LevelManager(levelSize.x,levelSize.y, numberOfFood);
        refreshTimer = 0.0f;
        simulationTimer = 0.0f;
        if (playerCount <= 0)
        {
            playerCount = 1;
        }
        snakeControllers = new GameObject[playerCount];
        AddPlayers();
    }
    
    void FixedUpdate()
    {
        simulationTimer += Time.deltaTime;
        if (simulationTimer >= simulationRate)
        {
            // Move all players simultaneously from command of the server
            foreach(GameObject controller in snakeControllers)
            {
                controller.GetComponent<S_Snake_Player>().HandleMovement(levelSize,wrapAround);
            }
            simulationTimer -= simulationRate;
        }
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshRate)
        {
            // Refresh the level
            levelGrid.RefreshLevel();
            refreshTimer -= refreshRate;
        }
    }

    private void AddPlayers()
    {
        for(int i = 0; i < playerCount; ++i)
        {
            // Create a new player control
            snakeControllers[i] = new GameObject("Snake_Controller_"+(i+1).ToString(), typeof(S_Snake_Player),typeof(SpriteRenderer),typeof(PolygonCollider2D),typeof(Rigidbody2D));
            snakeControllers[i].transform.parent = this.transform;
            snakeControllers[i].GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeHeadSprite;
            snakeControllers[i].GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }
}
