using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    Vector3 moveVec;
    Vector3 dodgeVec;



    bool isJump;
    bool isDodge;
    Rigidbody rigid;
    Animator anim;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput() {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move() {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge) {
            moveVec = dodgeVec;
        }
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn() {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump() {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge) {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    
    void Dodge() {
        if (jDown && moveVec != Vector3.zero && !isJump && !isJump) {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
        }
    }
    
    void DodgeOut() {
        isDodge = false;
        speed *= 0.5f;
    }
    
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
