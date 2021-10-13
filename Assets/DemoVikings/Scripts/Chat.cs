using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : Photon.MonoBehaviour
{
    public GameObject ChatList;
    public List<string> messages = new List<string>();

    public InputField chatInput;

    public GameObject FoldButton;
    public GameObject SpreadButton;

    private void Start()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            ChatList.transform.GetChild(i).GetComponent<Text>().text = "";
        }

        this.gameObject.SetActive(false);
    }

    public void ChatInput() //edit call
    {
        if(chatInput.text.Length > 0)
        {
            SendChat();
        }
    }

    void SendChat()
    {
        //photonView.RPC("SendChatMessage", PhotonTargets.All, chatInput.text);   // error when one player in game.
        SendChatMessage(chatInput.text);
        chatInput.text = "";
        
    }

    //[PunRPC]
    //void SendChatMessage(string text, PhotonMessageInfo info)
    void SendChatMessage(string text)
    {
        //AddMessage("[" + info.sender + "] " + text);
        AddMessage(text);
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

    public void Fold()
    {
        ChatList.SetActive(false);
        chatInput.gameObject.SetActive(false);
        FoldButton.SetActive(false);
        SpreadButton.SetActive(true);
    }

    public void Spread()
    {
        ChatList.SetActive(true);
        chatInput.gameObject.SetActive(true);
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
