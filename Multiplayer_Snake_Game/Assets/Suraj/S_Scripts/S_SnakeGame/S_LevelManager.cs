using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class S_LevelManager : MonoBehaviourPun
{
    public static S_LevelManager levelManager;
    // Handles all things not done by the players

    private Vector2Int currentRandomPostion;
    private Vector2Int levelSize;
    private Vector2Int levelMargin;
    private GameObject levelHolder;
    private List<GameObject> levelObjects;
    private int initialFood;

    PhotonView pView;
    // IMPLEMENT WALLS LATER
    // private int initialWalls;
    
    private void Awake()
    {
        if (S_LevelManager.levelManager == null)
        {
            S_LevelManager.levelManager = this;
        }
        else
        {
            if (S_LevelManager.levelManager != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    /*
    public S_LevelManager(int xSize, int ySize, int xMargin, int yMargin, int foodNum)
    {
        this.levelSize = new Vector2Int(xSize,ySize);
        this.levelMargin = new Vector2Int(xMargin,yMargin);
        this.initialFood = foodNum;
        this.currentRandomPostion = new Vector2Int(0,0);
        levelHolder = GameObject.FindGameObjectWithTag("Holder");
        levelObjects = new List<GameObject>();
        for(int i = 0; i < initialFood; ++i)
        {
            SpawnObject("Food");
        }
        pView = gameObject.AddComponent(typeof(PhotonView)) as PhotonView;
    }   */

    public void StartManager(int xSize, int ySize, int xMargin, int yMargin, int foodNum)
    {
        this.levelSize = new Vector2Int(xSize, ySize);
        this.levelMargin = new Vector2Int(xMargin, yMargin);
        this.initialFood = foodNum;
        this.currentRandomPostion = new Vector2Int(0, 0);
        levelHolder = GameObject.FindGameObjectWithTag("Holder");
        levelObjects = new List<GameObject>();
        for (int i = 0; i < initialFood; ++i)
        {
            SpawnObject("Food");
        }
        pView = this.GetComponent<PhotonView>();
    }

    private void RandomGridPos(bool randomizeX = true, bool randomizeY = true)
    {
        if (randomizeX)
        {
            currentRandomPostion.x = Random.Range(-levelSize.x+levelMargin.x,levelSize.x-levelMargin.x);
        }
        if (randomizeY)
        {
            currentRandomPostion.y = Random.Range(-levelSize.y+levelMargin.y,levelSize.y-levelMargin.y);
        }
    }
    [PunRPC]
    private void SpawnObject(string objTag)
    {
        // This is where an object is initally created
        if(objTag == "Food" && PhotonNetwork.IsMasterClient)
        {
            // Create a food object
            RandomGridPos();
            GameObject foodObject = PhotonNetwork.Instantiate("Food", new Vector3(currentRandomPostion.x, currentRandomPostion.y, 0), Quaternion.identity, 0);

            foodObject.tag = objTag;
            foodObject.name = objTag + levelObjects.Count.ToString();
            foodObject.transform.parent = levelHolder.transform;
            levelObjects.Add(foodObject);
            /*
            RandomGridPos();
            // GameObject foodObject = new GameObject(objTag+levelObjects.Count.ToString(), typeof(SpriteRenderer), typeof(CircleCollider2D));
            // foodObject.transform.parent = levelHolder.transform;
            // foodObject.transform.position = new Vector3(currentRandomPostion.x, currentRandomPostion.y);
            // foodObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
            // foodObject.GetComponent<CircleCollider2D>().isTrigger = true;
            GameObject foodObject = GameObject.Instantiate(GameAssets.instance.foodGenericPrefab);
            foodObject.tag = objTag;
            foodObject.name = objTag+levelObjects.Count.ToString();
            foodObject.transform.parent = levelHolder.transform;
            foodObject.transform.position = new Vector3(currentRandomPostion.x, currentRandomPostion.y);
            levelObjects.Add(foodObject);
            */
        }
    }
    [PunRPC]
    public void ReactivateObject(int levelObject)
    {
        // This is where a level object can change
        // Most often, the level object will just be reactiviated in a random position
        RandomGridPos();
        PhotonView Enable = PhotonView.Find(levelObject);
        Enable.transform.position = new Vector3(currentRandomPostion.x, currentRandomPostion.y);
        Enable.transform.gameObject.SetActive(true);
    }

    public void RefreshLevel()
    {
        // Instead of deleting and remaking objects, objects are hidden then reactivated
        if (!PhotonNetwork.IsMasterClient)
            return;
        foreach(GameObject lvlObj in levelObjects)
        {
            if (lvlObj.activeSelf == false)
            {
                pView.RPC("ReactivateObject", RpcTarget.All, lvlObj.gameObject.GetComponent<PhotonView>().ViewID);
                //ReactivateObject(lvlObj);
            }
        }
    }


}

            // GameObject foodObject = PhotonNetwork.Instantiate("Food", new Vector3(currentRandomPostion.x, currentRandomPostion.y, 0), Quaternion.identity, 0);
            
            // //GameObject foodObject = new GameObject(objTag+levelObjects.Count.ToString(), typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(PhotonView), typeof(PhotonTransformView));
            // //foodObject.transform.position = new Vector3(currentRandomPostion.x, currentRandomPostion.y);
            // //foodObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
            // foodObject.GetComponent<CircleCollider2D>().isTrigger = true;
            // /*
            // PhotonTransformView view = foodObject.GetComponent<PhotonTransformView>();
            // view.m_SynchronizePosition = true;
            // PhotonView pview = foodObject.GetComponent<PhotonView>();
            // pview.ObservedComponents = new List<Component>();
            // pview.ObservedComponents.Add(view);
            // */
            // foodObject.tag = objTag;