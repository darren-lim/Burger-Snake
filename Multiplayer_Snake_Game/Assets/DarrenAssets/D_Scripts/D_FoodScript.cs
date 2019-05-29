using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_FoodScript : MonoBehaviour
{
    private Vector2Int gridPos;
    //get size of playing area
    public int windowXSize;
    public int windowYSize;


    // Start is called before the first frame update
    void Start()
    {
        //spawn food in random position

    }

    private void OnEnable()
    {
        newPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void newPosition()
    {
        int randPosX = Random.Range(-windowXSize, windowXSize);
        int randPosY = Random.Range(-windowYSize, windowYSize);
        Vector2Int newPos = new Vector2Int(randPosX, randPosY);
        this.transform.position = new Vector3(newPos.x, newPos.y);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            //add tail to player and score
            newPosition();
        }
    }
}
