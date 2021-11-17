using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_369 : Photon.MonoBehaviour
{
    public PhotonView PV;
    public Text nick_name;
    public Image myTurn_img;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.curPlayer_str == nick_name.text)
        {
            myTurn_img.gameObject.SetActive(true);
        }
        else
        {
            myTurn_img.gameObject.SetActive(false);
        }
    }
}
