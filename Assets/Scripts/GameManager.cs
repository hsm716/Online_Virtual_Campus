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

    public float time = 5f;

    public int GameReader_ID;

    public GameObject[] players;
    public GameObject GameUI;
    public GameObject InGamePanel;
    [SerializeField]
    private int curPlayer_Count;

    public PhotonView PV;

    public bool isGaming;
    public bool isFinish;

    public int curNumber;
    public Text curNumber_txt;
    public string curTurnplayer;
    

    public Transform players_loc;
    public GameObject TotalPanel;
    public Text Loser_Txt;
    public Slider time_slider;
    public string loser_str;
    

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
                p.GetComponent<PlayerControl>().isReady = true;
                JoinButton.GetComponent<Button>().interactable = false;
                PV.RPC("Increase_Count",PhotonTargets.All);
                break;
            }
        }
        
    }
    [PunRPC]
    public void Increase_Count()
    {
        curPlayer_Count += 1;
    }
    public void StartGame()
    {
        time = 5f;
        curNumber = 1;
        FindPlayers();
        isGaming = true;
        InGamePanel.SetActive(true);
        PV.RPC("panel_On", PhotonTargets.All);
        /*        foreach (var p in players)
                {
                    if (p.GetComponent<PlayerControl>().isGameReader== true)
                    {
                        GameObject po = PhotonNetwork.Instantiate("InGame_369_player", Vector3.zero, Quaternion.identity, 0);
                        po.transform.SetParent(players_loc);
                        po.transform.localScale = new Vector3(1f, 1f, 1f);
                        po.GetComponent<Player_369>().nick_name.text = PhotonNetwork.player.NickName;
                    }
                }*/



    }
    [PunRPC]
    public void panel_On()
    {
        foreach (var p in players)
        {
            if (p.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.player.NickName)
            {
                if(p.GetComponent<PlayerControl>().isReady == true)
                {
                    InGamePanel.SetActive(true);
                    p.GetComponent<PlayerControl>().isGaming = true;
                    GameObject po = PhotonNetwork.Instantiate("InGame_369_player", Vector3.zero, Quaternion.identity, 0);
                    po.transform.SetParent(players_loc);
                    po.transform.localScale = new Vector3(1f, 1f, 1f);
                    po.GetComponent<Player_369>().nick_name.text = PhotonNetwork.player.NickName;
                }
                else
                {
                    GameUI.SetActive(false);
                }

                
                
            }
        }
        
    }
    [PunRPC]
    public void InCrease_CurNumber()
    {
        curNumber += 1;
    }
    public void SetTurnPlayer()
    {
        curTurnplayer = players_loc.GetChild(0).GetComponent<Player_369>().nick_name.text;
    }
    public void Select_Number()
    {
        if (curNumber.ToString().Contains('3')|| curNumber.ToString().Contains('6')|| curNumber.ToString().Contains('9'))
        {
            loser_str = PhotonNetwork.player.NickName;
            PV.RPC("Quit_Game", PhotonTargets.All, loser_str);

        }
        else
        {
            PV.RPC("InCrease_CurNumber", PhotonTargets.All);
            time = 5f;
        }
    }
    public void Select_Clap()
    {
        if (!(curNumber.ToString().Contains('3') || curNumber.ToString().Contains('6') || curNumber.ToString().Contains('9')))
        {
            loser_str = PhotonNetwork.player.NickName;

            PV.RPC("Quit_Game", PhotonTargets.All,loser_str);
            
        }
        else
        {
            PV.RPC("InCrease_CurNumber", PhotonTargets.All);
            time = 5f;
        }
    }
    [PunRPC]
    public void Quit_Game(string name)
    {
        Loser_Txt.text = name+ " 패배";
    }

    [PunRPC]
    public void Quit_Game()
    {
        isGaming = false;
        InGamePanel.SetActive(false);
        curPlayer_Count = 1;
        curNumber = 1;
        JoinButton.GetComponent<Button>().interactable = true;
        StartButton.GetComponent<Button>().interactable = false;
        var gp = GameObject.FindGameObjectsWithTag("game_player");
        foreach(var g in gp)
        {
            Destroy(g);
        }
        foreach (var p in players)
        {
            p.GetComponent<PlayerControl>().isReady = false;
            p.GetComponent<PlayerControl>().isGameReader = false;
            p.GetComponent<PlayerControl>().isGaming = false;
        }
        GameUI.SetActive(false);


    }
    public void TimeOver()
    {
        loser_str = PhotonNetwork.player.NickName;
        Quit_Game();
        PV.RPC("Quit_Game", PhotonTargets.All);
    }

    public void Check_result()
    {
        TotalPanel.SetActive(false);
    }

   
    void Awake()
    {
        instance = this;
        curPlayer_Count = 1;
    }

    // Update is called once per frame

    private void Update()
    {
        curPlayerCount_txt.text = curPlayer_Count + " / " + PhotonNetwork.playerList.Count();
        curNumber_txt.text = curNumber + "";
        //time_slider.value = time / 5f;
        //time -= Time.deltaTime;
        /*if (time < 0f&&isGaming)
        {
            isGaming = false;
            TimeOver();
        }*/
        if (isGaming&&isFinish==true)
        {
            isFinish = false;
            InGamePanel.SetActive(false);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(GamePlayer_list);
           // stream.SendNext(time);
            stream.SendNext(curPlayer_Count);
            stream.SendNext(isGaming);
            stream.SendNext(curNumber);
            stream.SendNext(loser_str);
            stream.SendNext(Loser_Txt.text);
        }
        else
        {
            //GamePlayer_list = (string[])stream.ReceiveNext();
            //time = (float)stream.ReceiveNext();
            curPlayer_Count = (int)stream.ReceiveNext();
            isGaming = (bool)stream.ReceiveNext();
            curNumber = (int)stream.ReceiveNext();
            loser_str = (string)stream.ReceiveNext();
            Loser_Txt.text = (string)stream.ReceiveNext();

        }
    }
}
