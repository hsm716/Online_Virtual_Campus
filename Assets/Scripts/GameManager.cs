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
    public Text curPlayer_Txt;

    public Text curPlayerTop_Txt;

    public Slider time_slider;
    public string curPlayer_str;

    public int curIdx;
    

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
        PV.RPC("SetGaming", PhotonTargets.All);
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


        curPlayer_str = players[0].GetComponent<PlayerControl>().NickName.text;
    }
    [PunRPC]
    public void SetGaming()
    {
        isGaming = true;
    }

    [PunRPC]
    public void panel_On()
    {
        foreach (var p in players)
        {
            if (p.GetComponent<PlayerControl>().isReady == true)
            {
                InGamePanel.SetActive(true);
                time = 5f;
                p.GetComponent<PlayerControl>().isGaming = true;
                GameObject po = PhotonNetwork.Instantiate("InGame_369_player", Vector3.zero, Quaternion.identity, 0);
                po.transform.SetParent(players_loc);
                po.transform.localScale = new Vector3(1f, 1f, 1f);
                po.GetComponent<Player_369>().nick_name.text = p.GetComponent<PlayerControl>().NickName.text;
            }
            else
            {
                GameUI.SetActive(false);
            }
            /*if (p.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.player.NickName)
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
            }*/
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
        if (curPlayer_str == PhotonNetwork.player.NickName)
        {
            if (curNumber.ToString().Contains('3') || curNumber.ToString().Contains('6') || curNumber.ToString().Contains('9'))
            {
                curPlayer_str = PhotonNetwork.player.NickName;
                PV.RPC("Quit_Game", PhotonTargets.All, curPlayer_str);

            }
            else
            {
                PV.RPC("InCrease_CurNumber", PhotonTargets.All);
                
                PV.RPC("Next_Data", PhotonTargets.All);
            }
        }
    }
    public void Select_Clap()
    {
        if (curPlayer_str == PhotonNetwork.player.NickName)
        {
            if (!(curNumber.ToString().Contains('3') || curNumber.ToString().Contains('6') || curNumber.ToString().Contains('9')))
            {
                curPlayer_str = PhotonNetwork.player.NickName;

                PV.RPC("Quit_Game", PhotonTargets.All, curPlayer_str);

            }
            else
            {
                PV.RPC("InCrease_CurNumber", PhotonTargets.All);
                
                PV.RPC("Next_Data", PhotonTargets.All);
            }
        }
    }
    [PunRPC]
    public void Next_Data()
    {
        time = 5f;
        curIdx++;
        if (curIdx >= players.Count())
        {
            curIdx = 0;
        }
        curPlayer_str = players[curIdx].GetComponent<PlayerControl>().NickName.text;
        curPlayerTop_Txt.text = curPlayer_str;
    }


    [PunRPC]
    public void Quit_Game(string name)
    {
        isGaming = false;
        isFinish = true;
        
        var gp = GameObject.FindGameObjectsWithTag("game_player");
        players = new GameObject[0];
        foreach (var g in gp)
        {
            Destroy(g);
        }
        InGamePanel.SetActive(false);
        curPlayer_Count = 1;
        curNumber = 1;
        if (name != "")
        {
            TotalPanel.SetActive(true);
            curPlayer_Txt.text = name + " 패배";
        }
        JoinButton.GetComponent<Button>().interactable = true;
        StartButton.GetComponent<Button>().interactable = false;
        
        foreach (var p in players)
        {
            p.GetComponent<PlayerControl>().isReady = false;
            p.GetComponent<PlayerControl>().isGameReader = false;
            p.GetComponent<PlayerControl>().isGaming = false;
        }
        GameUI.SetActive(false);
        isFinish = false;
    }
    public void TimeOver()
    {
        PV.RPC("Quit_Game", PhotonTargets.All, curPlayer_str);
        
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
        time_slider.value = time / 5f;
        if (players.Count() != 0)
        {
            if (players[0].GetComponent<PlayerControl>().NickName.text == PhotonNetwork.player.NickName)
            {
                time -= Time.deltaTime;
            }
        }
        if (time < 0f && isGaming&& isFinish==false)
        {
            
            TimeOver();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(GamePlayer_list);
            stream.SendNext(time);
            stream.SendNext(curPlayer_Count);
            stream.SendNext(isGaming);
            stream.SendNext(curNumber);
            stream.SendNext(curPlayer_str);
            stream.SendNext(curPlayer_Txt.text);
            stream.SendNext(curIdx);
            stream.SendNext(isFinish);
            stream.SendNext(curPlayerTop_Txt.text);
        }
        else
        {
            //GamePlayer_list = (string[])stream.ReceiveNext();
            time = (float)stream.ReceiveNext();
            curPlayer_Count = (int)stream.ReceiveNext();
            isGaming = (bool)stream.ReceiveNext();
            curNumber = (int)stream.ReceiveNext();
            curPlayer_str = (string)stream.ReceiveNext();
            curPlayer_Txt.text = (string)stream.ReceiveNext();
            curIdx = (int)stream.ReceiveNext();
            isFinish = (bool)stream.ReceiveNext();
            curPlayerTop_Txt.text = (string)stream.ReceiveNext();

        }
    }
}
