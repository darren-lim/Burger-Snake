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

    // Sprites Assets - Will soon be removed (use prefabs instead)
    public Sprite snakeHeadSprite;
    public Sprite snakeBodySprite;
    public Sprite foodSprite;

    // Prefab Assets - Snake
    public GameObject[] snakeHeadPrefab;
    public GameObject snakeBodyPrefab;
    public GameObject snakeTailPrefab;
    // Prefab Assets - Food
    public GameObject foodPattyPrefab;
    public GameObject foodLettucePrefab;
    public GameObject foodTomatoPrefab;
    public GameObject foodOnionsPrefab;
    public GameObject foodPicklesPrefab;
    // Prefab Assets - Power Up
    public GameObject powerupKetchupPrefab;
    public GameObject powerupMusturdPrefab;
    public GameObject powerupHotSaucePrefab;
}
