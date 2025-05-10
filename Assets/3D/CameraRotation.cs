using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
public class CameraRotation : MonoBehaviour
{
    public float speedX;
    public float speedY;
    public PlayerMovement pm;
    public Rigidbody rb;
    public Quaternion horizontalRotation;
    float yRot=0;
    float rotateHorizontal;
    float rotateVertical;
    public Animator anim;
    public bool canRepel = true;
    public bool canPull = true;
    public GameObject flashlight;
    public PlayerSwordFinal psf;
    public ParticleSystem repel;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        horizontalRotation = transform.rotation;
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
       
        Quaternion og = transform.rotation;
        transform.position = Vector3.Lerp(transform.position,rb.position + (Vector3.up*0.5f),Time.deltaTime*35);
        
            rotateHorizontal = Input.GetAxis("Mouse X") * speedX;
            rotateVertical = Input.GetAxis("Mouse Y") * speedY;
            
            horizontalRotation.x = 0;
             horizontalRotation.z = 0;
             horizontalRotation *= Quaternion.Euler(0, rotateHorizontal, 0);

              yRot -= rotateVertical;
             yRot = Mathf.Clamp(yRot, -89, 89);

            transform.rotation = Quaternion.Lerp(transform.rotation,horizontalRotation * Quaternion.Euler(yRot, 0, 0),Time.deltaTime*35);
            
        if (rb.velocity.magnitude > 0.3f)
        {
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (canRepel)
            {
                anim.SetTrigger("Repel");
                canRepel = false;
                canPull = false;
                psf.Repel();
            }
        }
        if (Input.GetButtonDown("Fire3"))
        {
            if (canPull)
            {
                anim.SetTrigger("Pull");
                canPull = false;
                canRepel = false;
                psf.Pull();
            }
        }


    }
        
    }




