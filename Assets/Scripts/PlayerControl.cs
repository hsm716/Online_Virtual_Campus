using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

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

    public int grade;
    GameObject Location_text;

    public Image grade_img;
    public Sprite[] grade_sp;

    public GameObject interaction_btn;

    private void Awake()
    {


        NickName.text = PV.owner.NickName;
        rgbd2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
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


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Trigger_Check"))
        {
            Location_text.transform.GetChild(0).GetComponent<Text>().text = col.name;
        }
        if (col.CompareTag("상호작용_구역"))
        {
            interaction_btn.SetActive(true);

            if (col.name =="제2과기대")
            {

            }
            if ((col.name == "자유관"))
            {
                Application.OpenURL("https://dormitel.korea.ac.kr/ipsaUser/facilitiesImageList.do?site_id=hoyeon&facilitiesId=1&id=hoyeon_010402000000");
            }
            if ((col.name == "IPARK"))
            {

            }
            if ((col.name == "종합운동장"))
            {

            }
            if ((col.name == "극장"))
            {

            }
            if ((col.name == "ICT가속"))
            {

            }
            if ((col.name == "엘레베이터"))
            {

            }
            if ((col.name == "산학협력"))
            {

            }
            if ((col.name == "농심국제관"))
            {

            }
            if ((col.name == "호상"))
            {

            }
            if ((col.name == "주차장"))
            {

            }
            if ((col.name == "정문"))
            {

            }
            if ((col.name == "학생회관"))
            {

            }
            if ((col.name == "공공정책관"))
            {

            }
            if ((col.name == "행정관"))
            {

            }
            if ((col.name == "석원경상"))
            {

            }
            if ((col.name == "학군단"))
            {

            }
            if ((col.name == "문화스포츠"))
            {

            }
            if ((col.name == "체육과학관"))
            {

            }
            if ((col.name == "동문운동장"))
            {

            }
            if ((col.name == "호익플라자"))
            {

            }
            if ((col.name == "약학대"))
            {

            }
            if ((col.name == "중앙광장"))
            {

            }
            if ((col.name == "학술정보원"))
            {

            }
            if ((col.name == "고미사"))
            {

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
            //stream.SendNext(transform.GetComponent<Rigidbody2D>().velocity);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            grade = (int)stream.ReceiveNext();
            //transform.GetComponent<Rigidbody2D>().velocity = (Vector2)stream.ReceiveNext();
        }
    }
}
