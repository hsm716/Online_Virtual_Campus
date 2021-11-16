using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Photon.MonoBehaviour,IPunObservable
{

    //static public string[] GamePlayer_list = new string[100];



    public int time = 3;




    public void JoinGame()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(var p in players)
        {
            if(p.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.player.NickName)
            {
                p.GetComponent<PlayerControl>().isGaming = true;
                break;
            }
        }
        
    }




    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(GamePlayer_list);
            stream.SendNext(time);
        }
        else
        {
            //GamePlayer_list = (string[])stream.ReceiveNext();
            time = (int)stream.ReceiveNext();
        }
    }
}
