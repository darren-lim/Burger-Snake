using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    // Holds all assest from which other scripts reference from

    public static GameAssets instance;

    private void Awake()
    {
        instance = this;
    }

    public Sprite snakeHeadSprite;
    public Sprite snakeBodySprite;
    public Sprite foodSprite;
}
