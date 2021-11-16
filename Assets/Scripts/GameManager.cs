using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour,IPunObservable
{

    //static public string[] GamePlayer_list = new string[100];

    static public GameManager instance;

    public GameObject StartButton;
    public GameObject JoinButton;

    public Text curPlayerCount_txt;

    public int time = 3;

    public int GameReader_ID;

    public GameObject[] players;

    [SerializeField]
    int curPlayer_Count = 1;
    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    public void JoinGame()
    {

        FindPlayers();
        foreach (var p in players)
        {
            if(p.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.player.NickName)
            {
                p.GetComponent<PlayerControl>().isGaming = true;
                JoinButton.SetActive(false);
                curPlayer_Count += 1;
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

    // Update is called once per frame

    private void Update()
    {
        curPlayerCount_txt.text = curPlayer_Count + " / " + PhotonNetwork.playerList.Count();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(GamePlayer_list);
            stream.SendNext(time);
            stream.SendNext(curPlayer_Count);
        }
        else
        {
            //GamePlayer_list = (string[])stream.ReceiveNext();
            time = (int)stream.ReceiveNext();
            curPlayer_Count = (int)stream.ReceiveNext();
        }
    }
}
