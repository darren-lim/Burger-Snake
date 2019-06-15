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
    public Text currentTimerText;
    public Canvas LobbyCanvas;

    // Player Info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playerInGame;


    // Delayed Start
    public bool readyToCount;
    public bool readyToStart;
    public float startingTime = 10f;
    private float lessThanMax; // Time need to start at less than 4 players
    private float atMax; // Time need to start at 4 players
    private float timeToStart; // Time at the start

    public int ReadyPlayers = 0;
    public bool readytoplay = false;
    bool isReady = false;
    public GameObject ready;

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
        currentTimerText.text = "";
        ready.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are in a room!");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();
        PhotonNetwork.AutomaticallySyncScene = true;
        currentPlayerCountText.text = playersInRoom.ToString() + " in Room";
        if (playersInRoom > 1)
        {
            readytoplay = true;
        }

        if (playersInRoom == MultiplayerSettings.multiplayerSetting.maxPlayers)
        {
            readyToStart = true;
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        currentPlayerCountText.text = playersInRoom.ToString() + " in Room";
        if (playersInRoom > 1)
        {
            readytoplay = true;
        }
        if (playersInRoom == MultiplayerSettings.multiplayerSetting.maxPlayers)
        {
            readyToStart = true;
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        playersInRoom--;
        ResetTimer();
        isReady = false;
        readytoplay = false;
        if (currentPlayerCountText == null) return;
        currentPlayerCountText.text = playersInRoom.ToString() + " in Room";
        currentTimerText.text = "Game countdown stopped";
        PV.RPC("ResetReadyPlayers", RpcTarget.All);
        ReadyPlayers = 0;
        ready.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Exited Room");
        ResetTimer();
        isReady = false;
        readytoplay = false;
        if (currentPlayerCountText == null) return;
        currentPlayerCountText.text = "Not in any room";
        currentTimerText.text = "";
        PV.RPC("ResetReadyPlayers", RpcTarget.All);
        ReadyPlayers = 0;
        ready.SetActive(false);
    }

    void Update()
    {
        if (playersInRoom == 1 || playersInRoom != ReadyPlayers)
        {
            ResetTimer();
        }
        else if (!isGameLoaded && readytoplay)
        {
            if (playersInRoom == ReadyPlayers)
                readyToCount = true;
            if (readyToStart)
            {
                atMax -= Time.deltaTime;
                lessThanMax = atMax;
                timeToStart = atMax;
                //PV.RPC("setCountdown", RpcTarget.All, "Time till start " + timeToStart.ToString("0.00"));
                //setCountdown("Time till start " + timeToStart.ToString("0.00"));
                currentTimerText.text = "Time till start " + timeToStart.ToString("0.00");
            }
            else if (readyToCount)
            {
                lessThanMax -= Time.deltaTime;
                timeToStart = lessThanMax;
                //PV.RPC("setCountdown", RpcTarget.All, "Time till start " + timeToStart.ToString("0.00"));
                //setCountdown("Time till start " + timeToStart.ToString("0.00"));
                currentTimerText.text = "Time till start " + timeToStart.ToString("0.00");
            }
            else
            {
                currentTimerText.text = "Game countdown";
            }
            if (timeToStart <= 0)
            {
                StartGame();
            }
        }
    }

    // public Player getSpecifcPlayers(int playerIndex)
    // {
    //     return photonPlayers[playerIndex];
    // }

    public void StartGame()
    {
        isGameLoaded = true;
        if (LobbyCanvas == null) return;
        LobbyCanvas.gameObject.SetActive(false);
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        PV.RPC("LoadMulti", RpcTarget.All);
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
        if (currentScene == MultiplayerSettings.multiplayerSetting.multiplayerScene)
        {
            isGameLoaded = true;
            PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        }
    }
    public void ReadyGameButtonClicked()
    {
        if (!isReady)
        {
            PV.RPC("AddReadyPlayers", RpcTarget.All);
            //ReadyPlayers += 1;
        }
        isReady = true;
        ready.SetActive(true);
    }
    [PunRPC]
    void AddReadyPlayers()
    {
        ReadyPlayers += 1;
    }
    [PunRPC]
    void setCountdown(string cnt)
    {
        currentTimerText.text = cnt;
    }
    [PunRPC]
    void ResetReadyPlayers()
    {
        ReadyPlayers = 0;
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

    [PunRPC]
    void LoadMulti()
    {
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSetting.multiplayerScene);
    }
}
