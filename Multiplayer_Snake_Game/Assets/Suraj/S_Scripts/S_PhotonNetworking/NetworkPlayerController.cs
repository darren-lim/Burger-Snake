using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkPlayerController : MonoBehaviour
{
    private PhotonView PV;
    int playerID;
    public GameObject myAvatar;

    // Initializes the network avatar with the main snake controller
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            if(GameObject.FindGameObjectWithTag("Handler") != null)
            {
                playerID = GameObject.FindGameObjectWithTag("Room").GetComponent<PhotonRoom>().myNumberInRoom;
                string playerModel = "player" + playerID.ToString() + "Controller";
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",playerModel),
                                                    GameAssets.instance.SpawnPoints[playerID-1].position,
                                                    GameAssets.instance.SpawnPoints[playerID-1].rotation,
                                                    0);
                myAvatar.GetComponent<S_Snake_Player>().tag = "Player_"+playerID.ToString();
                Text myScoreBoard = GameObject.Find("Points_P"+playerID.ToString()).GetComponent<Text>();
                if(myScoreBoard != null)
                {
                    myScoreBoard.text = "Player "+playerID.ToString()+": 0";
                }
                myAvatar.GetComponent<S_Snake_Player>().myScoreboard = myScoreBoard;
                myAvatar.GetComponent<S_Snake_Player>().setSelfIntersect(GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().selfCollision);
                myAvatar.GetComponent<S_Snake_Player>().SetSizeWrap(
                    GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().GetWindowSize(),
                    GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().GetWarp());
            }
        }
    }
}
