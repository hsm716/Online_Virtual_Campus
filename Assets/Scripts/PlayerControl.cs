using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using agora_gaming_rtc;

public class PlayerControl : Photon.MonoBehaviour, IPunObservable
{

    [Header("이동 속도")]
    public float Speed;

    [Header("포톤 뷰")]
    public PhotonView PV;

    [Header("닉네임")]
    public Text NickName;


    private float h;
    private float v;
    bool isHorizontalMove;

    Vector3 curPos;
    Quaternion curRot;


    public GameObject myUI;

    public GameObject Bubble;
    public Text bubbleText;

    private Rigidbody2D rgbd2d;
    private Animator anim;

    public SpriteRenderer SR;
    
    Vector2 moveVec;

    //Mobile Key Var
    int up_Value;
    int down_Value;
    int left_Value;
    int right_Value;
    bool up_Down;
    bool down_Down;
    bool left_Down;
    bool right_Down;
    bool up_Up;
    bool down_Up;
    bool left_Up;
    bool right_Up;

    public Joystick joystick;
    public Transform Buttons;

    public int grade;
    GameObject Location_text;

    public Image grade_img;
    public Sprite[] grade_sp;

    public GameObject interaction_btn;
    string cur_inter_data = "";
    string cur_inter_type = "";

    public GameObject info_btn;
    string cur_url = "";

    public Transform VideoSpawnPoint;
    public long myUID;
    bool uiActive;
    public AgoraVideoChat AVC;
    public GameObject GameBoard;
    
    private void Awake()
    {

        
        NickName.text = PV.owner.NickName;
        rgbd2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        GameBoard = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
        if (!PV.isMine)
        {
            myUI.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
           
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
            Location_text = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
            Location_text.SetActive(true);
            uiActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (grade)
        {
            case 0:
                break;

            case 1:
                grade_img.sprite = grade_sp[0];
                break;
            case 2:
                grade_img.sprite = grade_sp[1];
                break;
            case 3:
                grade_img.sprite = grade_sp[2];
                break;

            case 4:
                grade_img.sprite = grade_sp[3];
                break;
        }
        if (PV.isMine)
        {
            //h = Input.GetAxisRaw("Horizontal");
            //v = Input.GetAxisRaw("Vertical");
            
            h = Input.GetAxisRaw("Horizontal") + right_Value + left_Value;
            v = Input.GetAxisRaw("Vertical") + up_Value + down_Value;

            bgm.volume = bgm_slider.value;

            ButtonUp();

            if(Mathf.Abs(joystick.Horizontal) > 0.2f || Mathf.Abs(joystick.Vertical) > 0.2f)
            {
                if(Mathf.Abs(joystick.Horizontal) > Mathf.Abs(joystick.Vertical))
                {
                    v = 0;
                    if(joystick.Horizontal > 0.2f)
                    {
                        h = 1;
                        ButtonPress(3); //right : 3
                        right_Down = true;
                    }
                    else if(joystick.Horizontal < -0.2f)
                    {
                        h = -1;
                        ButtonPress(2); //left : 2
                        left_Down = true;
                    }
                    else
                    {
                        h = 0;
                        left_Up = true;
                        right_Up = true;
                    }
                }
                else
                {
                    h = 0;
                    if (joystick.Vertical > 0.2f)
                    {
                        v = 1;
                        ButtonPress(0); //up : 0
                        up_Down = true;
                    }
                    else if(joystick.Vertical < -0.2f)
                    {
                        v = -1;
                        ButtonPress(1); //down : 1
                        down_Down = true;
                    }
                    else
                    {
                        v = 0;
                        down_Up = true;
                        up_Up = true;
                    }
                }
            }


            bool hDown = Input.GetButtonDown("Horizontal") || left_Down || right_Down;
            bool vDown = Input.GetButtonDown("Vertical") || up_Down || down_Down;
            bool hUp = Input.GetButtonUp("Horizontal") || left_Up || right_Up;
            bool vUp = Input.GetButtonUp("Vertical") || up_Up || down_Up;


            if (h != 0)
            {
                anim.SetBool("isRun", true);
                PV.RPC("FlipX", PhotonTargets.All,h);
            }
            else
            {
                anim.SetBool("isRun", false);
            }

            if (v > 0)
            {
                anim.SetBool("isRun_back", true);
                anim.SetBool("isRun_mid", true);
                
            }
            else if (v == 0)
            {
                anim.SetBool("isRun_mid", false);
            }
            else
            {
                anim.SetBool("isRun_back", false);
                anim.SetBool("isRun_mid", true);
                anim.SetBool("isRun", true);
            }

            //Mobile Var Init
            up_Down = false;
            down_Down = false;
            left_Down = false;
            right_Down = false;
            up_Up = false;
            down_Up = false;
            left_Up = false;
            right_Up = false;

            if (hDown)
                isHorizontalMove = true;
            else if (vDown)
                isHorizontalMove = false;
            else if (hUp || vUp)
                isHorizontalMove = h != 0;

            if (isHorizontalMove == true)
            {
                anim.SetBool("isRun_mid", false);
            }


            GameBoard.SetActive(uiActive);


            /* if (anim.GetInteger("hAxisRaw") != h)
             {
                 anim.SetBool("isChange", true);
                 anim.SetInteger("hAxisRaw", (int)h);
             }
             else if (anim.GetInteger("vAxisRaw") != v)
             {
                 anim.SetBool("isChange", true);
                 anim.SetInteger("vAxisRaw", (int)v);
             }
             else
             {
                 anim.SetBool("isChange", false);
             }*/


        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, curPos, Time.deltaTime*10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime*10f);

        }
    }
    [PunRPC]
    void FlipX(float axis)
    {
        SR.flipX = axis == -1;
    }
    private void FixedUpdate()
    {
        rgbd2d.velocity = Vector2.zero;
        rgbd2d.angularVelocity =0f;
       
        moveVec = isHorizontalMove ? new Vector2(h, 0) : new Vector2(0, v);
       
        //isHorizontalMove ? new Vector2(h, 0) : new Vector2(0, v);
        rgbd2d.MovePosition(rgbd2d.position + moveVec.normalized * Speed * Time.fixedDeltaTime);
        //transform.GetComponent<Rigidbody2D>().velocity = moveVec * Speed;
    }

    void ButtonPress(int index)
    {
        for (int i = 0; i < Buttons.childCount; i++)
        {
            //Button targetButton = Buttons.GetChild(i).GetComponent<Button>();
            if(i == index)
            {
                //Buttons.GetChild(i).GetComponent<Button>().onClick.Invoke();
                //targetButton.image.sprite = targetButton.spriteState.pressedSprite;
                Transform targetButton = Buttons.GetChild(i);
                targetButton.GetComponent<Image>().sprite = targetButton.GetComponent<ButtonSprite>().pressedSprite;
            }
        }
    }
    void ButtonUp()
    {
        for (int i = 0; i < 4; i++)
        {
            //Button targetButton = Buttons.GetChild(i).GetComponent<Button>();
            //ButtonSprite bs = Buttons.GetChild(i).GetComponent<ButtonSprite>();
            //targetButton.image.sprite = bs.NormalSprite;
            Transform targetButton = Buttons.GetChild(i);
            targetButton.GetComponent<Image>().sprite = targetButton.GetComponent<ButtonSprite>().NormalSprite;
            //targetButton.image.sprite = targetButton.spriteState.disabledSprite;
        }
    }

    public void Request_Game()
    {
        PV.RPC("Receive_GameRequest", PhotonTargets.All, myUID);
    }

    [PunRPC]
    public void Receive_GameRequest(long remoteUID)
    {
        Debug.Log(remoteUID);
        Open_Close_GameBoard(remoteUID);
    }

    void Open_Close_GameBoard(long remoteUID)
    {
        foreach (Transform item in VideoSpawnPoint)
        {
            Debug.Log("test");
            if (remoteUID.ToString() == item.name)
            {
                Debug.Log(myUID.ToString() + " " + item.name);

                if (uiActive == true)
                {
                    uiActive = false;
                }
                else
                {
                    uiActive = true;
                }

                break;
            }
        }
        
    }

    public void Back_Game()
    {
        GameBoard.SetActive(false);
    }
    public void test()
    {
        Debug.Log("@@@@@@");
    }

    /*
    //버튼 클릭
    public void ButtonDown(string type)
    {
        switch (type)
        {
            case "U":
                up_Value = 1;
                up_Down = true;
                break;
            case "D":
                down_Value = -1;
                down_Down = true;
                break;
            case "L":
                left_Value = -1;
                left_Down = true;
                break;
            case "R":
                right_Value = 1;
                right_Down = true;
                break;

        }
    }
    public void ButtonUp(string type)
    {
        switch (type)
        {
            case "U":
                up_Value = 0;
                up_Up = true;
                break;
            case "D":
                down_Value = 0;
                down_Up = true;
                break;
            case "L":
                left_Value = 0;
                left_Up = true;
                break;
            case "R":
                right_Value = 0;
                right_Up = true;
                break;

        }
    }
    */
    /////
    public void BubbleBubble(string chatText)
    {
        StopAllCoroutines();
        Bubble.SetActive(true);
        bubbleText.text = chatText;
        StartCoroutine(BubbleOff());
    }

    IEnumerator BubbleOff()
    {
        yield return new WaitForSeconds(3f);
        Bubble.SetActive(false);
    }
    /////
    ///
    public void Interaction_OpenURL()
    {
        
        if (cur_inter_data != "")
        {
            if (cur_inter_type=="url")
            {
                Application.OpenURL(cur_inter_data);
            }
            else if (cur_inter_type == "act")
            {
                if (cur_inter_data == "엘레베이터_1층_act")
                {
                    transform.position = GameObject.Find("엘레베이터_2층_act").transform.position;
                }
                if (cur_inter_data == "엘레베이터_2층_act")
                {
                    transform.position = GameObject.Find("엘레베이터_1층_act").transform.position;
                }
            }
        }
    }
    public void Info_OpenURL()
    {
        if (cur_url != "")
        {
            Application.OpenURL(cur_url);
        }
    }
    public AudioSource bgm;
    public Slider bgm_slider;
    public void Music_Onoff()
    {
        if (bgm.isPlaying)
        {
            bgm.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            bgm.Stop();
        }
        else
        {
            
            bgm.GetComponent<Image>().color = new Color(0.48f, 0, 0.1f);
            bgm.Play();

        }
    }

    public GameObject setting_pannel;
    public void Setting_btn()
    {
        setting_pannel.SetActive(true);
    }
    public void resume_btn()
    {
        setting_pannel.SetActive(false);
    }
    public void home_btn()
    {
        AgoraVideoChat.instance.TerminateAgoraEngine();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("StartScene");
    }
    public void quit_btn()
    {
        Application.Quit();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (PV.isMine)
        {
            if (col.CompareTag("Trigger_Check"))
            {
                Location_text.transform.GetChild(0).GetComponent<Text>().text = col.name;
                info_btn.SetActive(true);

                if (col.name == "제2과기대")
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.17";

                }
                else if (col.name == "제1과기대")
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.16";
                }
                else if ((col.name == "호연사관"))
                {
                    cur_url = "https://dormitel.korea.ac.kr/mbshome/mbs/hoyeon/subview.do?id=hoyeon_010401000000";
                }
                else if ((col.name == "IPARK"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.18";
                }
                else if ((col.name.Contains("종합운동장")))
                {

                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.22";
                }
                else if ((col.name == "가속기ICT융합관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.21";
                }
                else if ((col.name == "산학협력관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.20";

                }
                else if ((col.name == "농심국제관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.14";
                }
                else if ((col.name == "호상정류장"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.13";
                }
                else if ((col.name == "정문"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.1";
                }
                else if ((col.name == "학생회관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.11";
                }
                else if ((col.name == "공공정책관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.8";
                }
                else if ((col.name == "행정관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.7";
                }
                else if ((col.name == "석원경상관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.6";
                }
                else if ((col.name == "학군단"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.10";
                }
                else if ((col.name == "문화스포츠관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.4";
                }
                else if ((col.name == "체육과학관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.3";
                }
                else if ((col.name == "동문운동장"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.2";
                }
                else if ((col.name == "호익플라자"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.9";
                }
                else if ((col.name == "약학대"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.19";
                }
                else if ((col.name == "중앙광장"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.15";
                }
                else if ((col.name == "학술정보원"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.12";
                }
                else if ((col.name == "문화융합관"))
                {
                    cur_url = "https://namu.wiki/w/%EA%B3%A0%EB%A0%A4%EB%8C%80%ED%95%99%EA%B5%90/%EC%84%B8%EC%A2%85%EC%BA%A0%ED%8D%BC%EC%8A%A4/%EC%8B%9C%EC%84%A4#s-1.5";
                }
                else
                {
                    cur_url = "";
                }

            }
            if (col.CompareTag("상호작용_구역"))
            {
                interaction_btn.SetActive(true);

                if ((col.name.Contains("url")))
                {
                    cur_inter_type = "url";
                }
                if ((col.name.Contains("act")))
                {
                    cur_inter_type = "act";
                }


                if ((col.name == "미래관_식당_url"))
                {
                    cur_inter_data = "https://sejong.korea.ac.kr/campuslife/facilities/dining/weeklymenu";
                }
                else if ((col.name == "IPARK_url"))
                {
                    cur_inter_data = "http://sejong.korea.ac.kr/mbshome/mbs/kr/subview.do?id=kr_050705060000";
                }
                else if ((col.name.Contains("종합운동장")))
                {
                    cur_inter_data = "http://sejong.korea.ac.kr/campuslife/facilities/field/enrollment";
                }
                else if ((col.name == "산학협력_url"))
                {
                    cur_inter_data = "http://innovation.korea.ac.kr/";
                }
                else if ((col.name == "학군단_url"))
                {
                    cur_inter_data = "https://102rotc.korea.ac.kr/rotc102/SelectionSchedule.do";
                }
                else if ((col.name == "학술정보원_url"))
                {
                    cur_inter_data = "https://libs.korea.ac.kr/";
                }
                else if ((col.name == "엘레베이터_1층_act"))
                {
                    cur_inter_data = col.name;
                }
                else if ((col.name == "엘레베이터_2층_act"))
                {
                    cur_inter_data = col.name;
                }
                else
                {
                    cur_inter_data = "";
                }

            }
        }
       
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (PV.isMine)
        {
            if (col.CompareTag("상호작용_구역"))
            {
                interaction_btn.SetActive(false);
                cur_inter_data = "";
            }
            if (col.CompareTag("Trigger_Check"))
            {
                Location_text.transform.GetChild(0).GetComponent<Text>().text = "";
                info_btn.SetActive(false);
                cur_url = "";
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(grade);
            stream.SendNext(myUID);
            stream.SendNext(uiActive);
            //stream.SendNext(transform.GetComponent<Rigidbody2D>().velocity);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            grade = (int)stream.ReceiveNext();
            myUID = (long)stream.ReceiveNext();
            uiActive = (bool)stream.ReceiveNext();
            //transform.GetComponent<Rigidbody2D>().velocity = (Vector2)stream.ReceiveNext();
        }
    }
}
