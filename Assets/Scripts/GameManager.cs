using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Photon.MonoBehaviour,IPunObservable
{

    //static public string[] GamePlayer_list = new string[100];

    static public GameManager instance;

    public GameObject StartButton;

    public int time = 3;

    public int GameReader_ID;

    GameObject[] players;

    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    public void JoinGame()
    {
       
        
        foreach(var p in players)
        {
            if(p.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.player.NickName)
            {
                p.GetComponent<PlayerControl>().isGaming = true;
                break;
            }
        }
        
    }
    public void StartGame()
    {

    }




    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        FindPlayers();
        foreach (var p in players)
        {
            if (p.GetComponent<PlayerControl>().isGameReader==true) 
            {
                StartButton.SetActive(true);
                break;
            }
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
            //stream.SendNext(GamePlayer_list);
            stream.SendNext(time);
            stream.SendNext(GameReader_ID);
        }
        else
        {
            //GamePlayer_list = (string[])stream.ReceiveNext();
            time = (int)stream.ReceiveNext();
            GameReader_ID = (int)stream.ReceiveNext();
        }
    }
}
