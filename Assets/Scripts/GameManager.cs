using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Photon.MonoBehaviour,IPunObservable
{

    static public string[] GamePlayer_list = new string[100];



    public int time = 3;




    public void JoinGame()
    {
        for (int i = 0; i < GamePlayer_list.Length; i++) {
            if (GamePlayer_list[i] == "빔") {
                GamePlayer_list[i] = PhotonNetwork.player.NickName;
                break;
            }
        }
    }




    void Start()
    {
        for(int i = 0; i < 100; i++)
        {
            GamePlayer_list[i] = "빔";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(GamePlayer_list);
            stream.SendNext(time);
        }
        else
        {
            GamePlayer_list = (string[])stream.ReceiveNext();
            time = (int)stream.ReceiveNext();
        }
    }
}
