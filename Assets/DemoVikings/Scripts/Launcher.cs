using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    public GameObject LoadingMenu;
    public GameObject MainMenu;

    public InputField CreateRoomInput;
    public InputField JoinRoomInput;

    private void Awake()
    {
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;
    }

    private string roomName = "myRoom";
    private Vector2 scrollPos = Vector2.zero;

    public void Start()
    {
        LoadingMenu.SetActive(true);
    }

    public void CreateRoom()
    {
        roomName = CreateRoomInput.text;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void JoinRoom()
    {
        roomName = JoinRoomInput.text;
        PhotonNetwork.JoinRoom(roomName);
    }


    void ShowConnectingGUI()
    {
        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
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
