using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] weapons;
    public bool[] hasWeapon;

    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    Vector3 moveVec;
    Vector3 dodgeVec;



    bool isJump;
    bool isDodge;
    bool isSwap;
    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    GameObject equippedWeapon;
    int equippedWeaponIndex = -1;

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
        Interaction();
        Swap();
    }

    void GetInput() {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move() {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge) {
            moveVec = dodgeVec;
        }

        if (isSwap) {
            moveVec = Vector3.zero;
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

    void Swap() {
        if (sDown1 && (!hasWeapon[0] || equippedWeaponIndex == 0)) {
            return;
        }
        else if (sDown2 && (!hasWeapon[1] || equippedWeaponIndex == 1)) {
            return;
        }
        else if (sDown3 && !hasWeapon[2] || equippedWeaponIndex == 2) {
            return;
        }

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        else if (sDown2) weaponIndex = 1;
        else if (sDown3) weaponIndex = 2;


        if (weaponIndex != -1 && !isJump && !isDodge) {
            if (equippedWeapon != null) {
                equippedWeapon.SetActive(false);
            }
            equippedWeaponIndex = weaponIndex;
            equippedWeapon = weapons[weaponIndex];
            equippedWeapon.SetActive(true);
            
            anim.SetTrigger("doSwap");
            isSwap = true;

            Invoke("SwapOut", 0.5f);
        }
    }

    void SwapOut() {
        isSwap = false;
    }

    void Interaction() {
        if (iDown && nearObject != null && !isJump && !isDodge) {
            if (nearObject.tag == "Weapon") {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }
    
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Weapon") {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Weapon") {
            nearObject = null;
        }
    }
}
