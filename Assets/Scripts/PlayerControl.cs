using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    

    private void Awake()
    {
        
        transform.GetComponent<Rigidbody2D>();
        transform.GetComponent<Animator>();
        NickName.text = PV.owner.NickName;

    }
    private void Start()
    {
        if (!PV.isMine)
        {
            myUI.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.isMine)
        {

            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

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
            transform.position = Vector3.Slerp(transform.position, curPos, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime * 10f);
            
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveVec = isHorizontalMove ? new Vector2(h, 0) : new Vector2(0, v);
        transform.GetComponent<Rigidbody2D>().velocity = moveVec * Speed;
    }

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
