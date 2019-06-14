using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerController : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;


    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            if(GameObject.FindGameObjectWithTag("Handler") != null)
            {
                myAvatar = GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().AddPlayers();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myAvatar == null && GameObject.FindGameObjectWithTag("Handler") != null)
        {
            myAvatar = GameObject.FindGameObjectWithTag("Handler").GetComponent<GameHandler>().AddPlayers();
        }
        
    }
}
