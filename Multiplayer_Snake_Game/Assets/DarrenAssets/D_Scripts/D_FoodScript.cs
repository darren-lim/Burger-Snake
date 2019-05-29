using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_FoodScript : MonoBehaviour
{
    Vector2 pos;
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
        this.transform.position = newPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2 newPosition()
    {
        int randPosX = Random.Range(-windowXSize, windowXSize);
        int randPosY = Random.Range(-windowYSize, windowYSize);
        Vector2 newPos = new Vector2(randPosX, randPosY);
        return newPos;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            //add tail to player and score
            Debug.Log("SADFGS");
            this.transform.position = newPosition();
        }
    }
}
