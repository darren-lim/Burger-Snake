using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LevelManager
{
    // Handles all things not done by the players

    private Vector2Int currentRandomPostion;
    private Vector2Int levelSize;
    private GameObject levelHolder;
    private List<GameObject> levelObjects;
    private int initialFood;
    // IMPLEMENT WALLS LATER
    // private int initialWalls;

    public S_LevelManager(int xSize, int ySize, int foodNum)
    {
        this.levelSize = new Vector2Int(xSize,ySize);
        this.initialFood = foodNum;
        this.currentRandomPostion = new Vector2Int(0,0);
        levelHolder = GameObject.FindGameObjectWithTag("Holder");
        levelObjects = new List<GameObject>();
        for(int i = 0; i < initialFood; ++i)
        {
            SpawnObject("Food");
        }
    }   

    private void RandomGridPos(bool randomizeX = true, bool randomizeY = true)
    {
        if (randomizeX)
        {
            currentRandomPostion.x = Random.Range(-levelSize.x,levelSize.x);
        }
        if (randomizeY)
        {
            currentRandomPostion.y = Random.Range(-levelSize.y,levelSize.y);
        }
    }

    private void SpawnObject(string objTag)
    {
        // This is where an object is initally created
        if(objTag == "Food")
        {
            // Create a food object
            RandomGridPos();
            GameObject foodObject = new GameObject(objTag+levelObjects.Count.ToString(), typeof(SpriteRenderer), typeof(CircleCollider2D));
            foodObject.transform.parent = levelHolder.transform;
            foodObject.transform.position = new Vector3(currentRandomPostion.x, currentRandomPostion.y);
            foodObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
            foodObject.GetComponent<CircleCollider2D>().isTrigger = true;
            foodObject.tag = objTag;
            levelObjects.Add(foodObject);
        }
    }

    private void ReactivateObject(GameObject levelObject)
    {
        // This is where a level object can change
        // Most often, the level object will just be reactiviated in a random position
        RandomGridPos();
        levelObject.transform.position = new Vector3(currentRandomPostion.x, currentRandomPostion.y);
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
