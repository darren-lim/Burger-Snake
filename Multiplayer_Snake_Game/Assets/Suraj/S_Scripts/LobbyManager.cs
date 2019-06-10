using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager lobby;
    public GameObject randomRoomButton;
    public GameObject cancelButton;


    private void Awake()
    {
        // Creates a singleton
        lobby = this;
        // Don't allow join if not connected to server
        randomRoomButton.SetActive(false);
    }
    
    void Start()
    {
        // Connects to Master photon server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server!");
        PhotonNetwork.AutomaticallySyncScene = true;
        randomRoomButton.SetActive(true);
    }

    public void OnRandomRoomClicked ()
    {
        randomRoomButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Room not available or full");
        CreateRoom();

    }

    void CreateRoom()
    {
        int randomRoom = Random.Range(0,100000);
        RoomOptions roomOps = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSetting.maxPlayers};
        PhotonNetwork.CreateRoom("Room " + randomRoom, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a room that already exist. Creating new room.");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        randomRoomButton.SetActive(true);
        cancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}
