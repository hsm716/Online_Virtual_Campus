using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    public GameObject LoadingMenu;
    public GameObject MainMenu;

    public InputField PlayerNameInput;

    private void Awake()
    {
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        PlayerNameInput.text = "Player " + Random.Range(0, 10000).ToString("0000");

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;
    }

    private string roomName = "Main Campus";
    private Vector2 scrollPos = Vector2.zero;

    public void Start()
    {
        LoadingMenu.SetActive(true);

        if (PlayerPrefs.HasKey("username"))
        {
            PlayerNameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.playerName = PlayerPrefs.GetString("username");
        }
        else
        {
            PlayerNameInput.text = "Player " + Random.Range(0, 10000).ToString("0000");
        }
    }


    public void JoinRoom()
    {
        if(PhotonNetwork.GetRoomList().Length == 0) //if there's no room, create room self
        {
            PlayerPrefs.SetString("username", PlayerNameInput.text);
            PhotonNetwork.playerName = PlayerNameInput.text;
            MainMenu.SetActive(false);
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 20 }, TypedLobby.Default);
        }
        else //if already room exists, join that room
        {
            PlayerPrefs.SetString("username", PlayerNameInput.text);
            PhotonNetwork.playerName = PlayerNameInput.text;
            MainMenu.SetActive(false);
            PhotonNetwork.JoinRoom(roomName);
        }
    }


    public void OnConnectedToMaster()
    {
        // this method gets called by PUN, if "Auto Join Lobby" is off.
        // this demo needs to join the lobby, to show available rooms!

        PhotonNetwork.JoinLobby();  // this joins the "default" lobby
        LoadingMenu.SetActive(false);
        MainMenu.SetActive(true);

    }
}
