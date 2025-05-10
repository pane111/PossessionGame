using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    public Rigidbody rb;
    public float initialMoveSpeed;
    public float moveSpeed;
    float inputF;
    public bool canMove = true;
    Vector3 lastDir;
    public Animator anim;
    public Image hpBar;
    public float maxHp;
    private float curHp=100;
    bool hasTakenDamage=false;
    public Animator uiNotif;
    public TextMeshProUGUI uiNotifText;
    public Animator dmgAnim;
    bool isDashing = false;
    public float CurHp
    {
        get { return curHp; }
        set { curHp = value; hpBar.fillAmount = value / maxHp; 
           
        
            
        }
    }
    void Start()
    {
        anim = Camera.main.GetComponent<Animator>();
        moveSpeed = initialMoveSpeed;
        cam = Camera.main;
        lastDir = transform.forward;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 inputV = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputF = inputV.magnitude;

        float moveVertical = inputV.y;
        float moveHorizontal = inputV.x;


        Vector3 fw = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
        Vector3 rt = new Vector3(cam.transform.right.x, 0, cam.transform.right.z);

        transform.rotation = Quaternion.LookRotation(fw, Vector3.up);
        if (CurHp < maxHp)
        {
            CurHp+=0.25f;
        }


        if (canMove)
        {
            if (inputF > 0.4f)
            {
                lastDir = ((fw * moveVertical) + (rt * moveHorizontal)).normalized;

                //rb.AddForce(lastDir.normalized * moveSpeed, ForceMode.Acceleration);
                rb.velocity = new Vector3(lastDir.x * moveSpeed * inputF, rb.velocity.y, lastDir.z * moveSpeed * inputF);


            }
            else
            {

                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            if (Input.GetButtonDown("Jump"))
            {
                isDashing = true;
                canMove = false;
                rb.AddForce(lastDir * 35, ForceMode.Impulse);
                anim.SetTrigger("Dash");
                Invoke("RestoreMovement", 0.3f);
            }

        }
    }
    void RestoreMovement()
    {
        isDashing = false;
        canMove = true;
    }
    void OnDmg()
    {
        if (!hasTakenDamage)
        {
            hasTakenDamage = true;
            uiNotifText.text = "Your undying resolve shields you from harm!";
            uiNotif.SetTrigger("Message");
        }
        StartCoroutine(hitAnim());
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if ( !isDashing)
        {
            if (other.CompareTag("DZone"))
            {
                CurHp -= 35;
                OnDmg();

            }
            if (other.CompareTag("Projectile"))
            {
                CurHp -= 15;
                Destroy(other.gameObject);
                OnDmg();
            }
        }
        
    }
    IEnumerator hitAnim()
    {
        hpBar.color = Color.red;
        dmgAnim.SetTrigger("Hit");
        yield return new WaitForSeconds(0.15f);
        hpBar.color = Color.white;
        yield return null;
    }
}
