using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class NetworkPlayerController : MonoBehaviour
{
    private PhotonView PV;
    int playerID;
    public GameObject myAvatar;


    // Start is called before the first frame update
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
                myAvatar.GetComponent<S_Snake_Player>().setSelfIntersect(GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().selfCollision);
                myAvatar.GetComponent<S_Snake_Player>().SetSizeWrap(
                    GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().GetWindowSize(),
                    GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().GetWarp());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (myAvatar == null && GameObject.FindGameObjectWithTag("Handler") != null)
        // {
        //     playerID = GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().AddPlayers();
        //     string playerModel = "player" + playerID.ToString() + "Controller";
        //     myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",playerModel),
        //                                         GameAssets.instance.SpawnPoints[playerID-1].position,
        //                                         GameAssets.instance.SpawnPoints[playerID-1].rotation,
        //                                         0);
        // }

        // if (myAvatar.transform.parent == null)
        // {
        //     myAvatar.transform.parent = GameObject.FindGameObjectWithTag("Handler").transform;
        // }
        
    }
}
