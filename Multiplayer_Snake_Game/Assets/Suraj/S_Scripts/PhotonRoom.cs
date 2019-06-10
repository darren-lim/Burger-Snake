using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // Room Info: singleton room + shared UI
    public static PhotonRoom room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;
    public Text currentPlayerCountText;

    // Player Info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playerInGame;


    // Delayed Start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMax; // Time need to start at less than 4 players
    private float atMax; // Time need to start at 4 players
    private float timeToStart; // Time at the start


    void Awake()
    {
         if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMax = startingTime;
        atMax = 6;
        timeToStart = startingTime;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are in a room!");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();
        
        currentPlayerCountText.text = playersInRoom.ToString() + " in Room";
        if(playersInRoom > 1)
        {
            readyToCount = true;
        }
        if(playersInRoom == MultiplayerSettings.multiplayerSetting.maxPlayers)
        {
            readyToStart = true;
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player joined the room.");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if(playersInRoom > 1)
        {
            readyToCount = true;
        }
        if(playersInRoom == MultiplayerSettings.multiplayerSetting.maxPlayers)
        {
            readyToStart = true;
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    void Update()
    {
        if(playersInRoom == 1)
        {
            ResetTimer();
        }
        if(!isGameLoaded)
        {
            if(readyToStart)
            {
                atMax -= Time.deltaTime;
                lessThanMax = atMax;
                timeToStart = atMax;
            }
            else if(readyToCount)
            {
                lessThanMax -= Time.deltaTime;
                timeToStart = lessThanMax;
            }
            Debug.Log("Current time to start " + timeToStart.ToString());
            if(timeToStart<=0)
            {
                StartGame();
            }
        }
    }

    void StartGame()
    {
        isGameLoaded = true;
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSetting.multiplayerScene);
    }

    void ResetTimer()
    {
        lessThanMax = startingTime;
        timeToStart = startingTime;
        atMax = 6;
        readyToCount = false;
        readyToStart = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSettings.multiplayerSetting.multiplayerScene)
        {
            isGameLoaded = true;
            PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playerInGame++;
        if(playerInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"),transform.position,Quaternion.identity, 0);   
    }
}
