using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Level-focused Details
    private GameObject levelBack;
    private S_LevelManager levelGrid;
    private Vector2Int levelSize;
    private int marginPercent = 10;
    private float refreshRate = 10.0f;
    private float refreshTimer;
    private float simulationRate = 0.1f;
    private float simulationTimer;

    // Player-focused Details
    [SerializeField]
    private int playerCount;
    // private GameObject[] snakeControllers;
    [SerializeField]
    private int playersAdded = 0;


    // Options for gameplay - CAN BE EXPANDED LATER
    private int numberOfFood = 20;
    private bool wrapAround = true;
    private bool selfCollision = true;

    // Network Stuff
    private GameObject roomCache;
    
    void Start()
    {
        levelBack = GameObject.FindGameObjectWithTag("Background");
        levelSize = new Vector2Int(2*(int)levelBack.transform.localScale.x, 2*(int)levelBack.transform.localScale.y);
        levelGrid = new S_LevelManager(levelSize.x,levelSize.y, levelSize.x/marginPercent, levelSize.x/marginPercent, numberOfFood);
        refreshTimer = 0.0f;
        simulationTimer = 0.0f;
        roomCache = GameObject.FindGameObjectWithTag("Room");
        playerCount = roomCache.GetComponent<PhotonRoom>().playersInRoom;
        if (playerCount <= 0)
        {
            playerCount = 1;
        }
        // snakeControllers = new GameObject[playerCount];
        // AddPlayers();
    }

    void FixedUpdate()
    {
        // simulationTimer += Time.deltaTime;
        // if (simulationTimer >= simulationRate)
        // {
        //     // Move all players simultaneously from command of the server
        //     foreach(GameObject controller in snakeControllers)
        //     {
        //         //controller.GetComponent<S_Snake_Player>().HandleMovement(levelSize,wrapAround);
        //     }
        //     simulationTimer -= simulationRate;
        // }
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshRate)
        {
            // Refresh the level
            levelGrid.RefreshLevel();
            refreshTimer -= refreshRate;
        }
    }

    public int AddPlayers()
    {
        // for(int i = 0; i < playerCount; ++i)
        // {
        //     // Create a new player control - Now with prefab
        //     snakeControllers[i] = Instantiate(GameAssets.instance.snakeHeadPrefab[i],
        //                                         GameAssets.instance.SpawnPoints[i].position,
        //                                         GameAssets.instance.SpawnPoints[i].rotation,
        //                                         this.transform);
        //     snakeControllers[i].name = "Snake_Controller_"+i.ToString();
        //     // Transfer Ownership to other players if not own
        //     snakeControllers[i].GetComponent<S_Snake_Player>().SetOwner(i);
        // }
        // GameObject newPlayer = Instantiate(GameAssets.instance.snakeHeadPrefab[playersAdded],
        //                                    GameAssets.instance.SpawnPoints[playersAdded].position,
        //                                    GameAssets.instance.SpawnPoints[playersAdded].rotation,
        //                                    this.transform);
        // newPlayer.name = "Snake_Controller_"+playersAdded.ToString();
        // newPlayer.GetComponent<S_Snake_Player>().SetSizeWrap(levelSize,wrapAround);
        if(playersAdded <= playerCount)
        {
            playersAdded++;
            return playersAdded;
        }
        else
        {
            return -1;
        }
    }
}
