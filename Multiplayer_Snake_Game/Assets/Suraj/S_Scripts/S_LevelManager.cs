using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LevelManager
{
    // Handles all things not done by the players

    private Vector2Int levelSize;
    private List<GameObject> levelObjects;
    private int initialFood;
    // IMPLEMENT WALLS LATER
    // private int initialWalls;

    public S_LevelManager(int xSize, int ySize, int foodNum)
    {
        this.levelSize = new Vector2Int(xSize,ySize);
    }   

    public void SpawnObject()
    {
        // This is where an object is initally created
    }

    public void ReactivateObject(GameObject levelObject)
    {
        // This is where a level object can change
        levelObject.SetActive(true);
    }

    public void RefreshLevel()
    {
        // Instead of deleting and remaking objects, objects are hidden then reactivated
        foreach(GameObject lvlObj in levelObjects)
        {
            if (lvlObj.activeSelf == false)
            {
                ReactivateObject(lvlObj);
            }
        }
    }


}
