using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : Photon.MonoBehaviour
{
    public GameObject ChatList;
    public GameObject ChatListBackGround;
    public List<string> messages = new List<string>();

    public InputField chatInput;

    public GameObject FoldButton;
    public GameObject SpreadButton;

    public PhotonView PV;
    public int viewID = 0;

    public Text Location_txt;

    private void Start()
    {
        chatInput.characterLimit = 27;
        for (int i = 0; i < messages.Count; i++)
        {
            ChatList.transform.GetChild(i).GetComponent<Text>().text = "";
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<PhotonView>().isMine)   //자신만
            {
                viewID = p.GetComponent<PhotonView>().viewID;
            }
        }
    }

    public void ChatInput() //edit call
    {
        if (chatInput.text.Length > 0)
        {
            SendChat();
        }
    }

    void SendChat()
    {
        PV.RPC("SendChatMessage", PhotonTargets.All, chatInput.text, viewID);   // error when one player in game.
        chatInput.text = "";
    }

    [PunRPC]
    void SendChatMessage(string text, int viewID, PhotonMessageInfo info)
    {
        AddMessage("[" + info.sender.NickName + "]{" + Location_txt.text + "} " + text);
        BubbleOn(viewID, text);
        //AddMessage(text);
    }

    public void AddMessage(string text)
    {
        messages.Add(text);
        if (messages.Count > 10)
            messages.RemoveAt(0);

        ChatPrint();
    }

    void ChatPrint()
    {
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            ChatList.transform.GetChild(i).GetComponent<Text>().text = messages[i];
        }
    }

    void BubbleOn(int viewID, string text)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("테스트");
        Debug.Log("" + viewID);
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerControl>().PV.viewID == viewID)
            {
                
                p.GetComponent<PlayerControl>().BubbleBubble(text);
            }
        }
    }

    public void Fold()
    {
        //ChatList.SetActive(false);
        //chatInput.gameObject.SetActive(false);
        ChatListBackGround.transform.localScale = new Vector3(1, 0, 1);
        chatInput.transform.localScale = new Vector3(1, 0, 1);
        //ChatListBackGround.SetActive(false);
        FoldButton.SetActive(false);
        SpreadButton.SetActive(true);
    }

    public void Spread()
    {
        //ChatList.SetActive(true);
        //chatInput.gameObject.SetActive(true);
        ChatListBackGround.transform.localScale = new Vector3(1, 1, 1);
        chatInput.transform.localScale = new Vector3(1, 1, 1);
        //ChatListBackGround.SetActive(true);
        FoldButton.SetActive(true);
        SpreadButton.SetActive(false);
    }
    /*
    void OnLeftRoom()
    {
        this.enabled = false;
    }

    void OnJoinedRoom()
    {
        this.enabled = true;
    }

    void OnCreatedRoom()
    {
        this.enabled = true;
    }
    */
}
