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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.isMine)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            if(h != 0)
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



            bool hDown = Input.GetButtonDown("Horizontal");
            bool vDown = Input.GetButtonDown("Vertical");
            bool hUp = Input.GetButtonUp("Horizontal");
            bool vUp = Input.GetButtonUp("Vertical");

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(transform.GetComponent<Rigidbody2D>().velocity);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            //transform.GetComponent<Rigidbody2D>().velocity = (Vector2)stream.ReceiveNext();
        }
    }
}
