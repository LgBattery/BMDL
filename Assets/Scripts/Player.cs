using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour //This is the control script go make another for abilities
{
    public float moveSpeed = 1;
    public float lookSens = 1;
    public float gCheck = 1;
    public float jumpForce = 200;
    public float camMaxY = 80;
    public float camMinY = -80;

    public int Health = 1700;

    [SerializeField] private GameObject Pcamera; 
    [SerializeField] private Animator anim; 

    #region inputs
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode crouch;
    public KeyCode sprint;
    #endregion

    private Rigidbody rb;

    public bool grounded;

    private float yRot;
    private float xRot;
    private Vector3 StartRot;
    private float IPT;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        yRot = transform.localEulerAngles.y;
        xRot = Pcamera.transform.localEulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        StartRot = transform.eulerAngles;
        IPT = Time.time;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if(Physics.Raycast(transform.position, -transform.up, gCheck))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        anim.SetBool("Grounded", grounded);
    }
    private void FixedUpdate()
    {
        #region baseMovement
        if (Input.GetKey(forward))
        {
            anim.SetFloat("WalkSpeed", 1f);
            anim.SetBool("Walking", true);
            rb.MovePosition(rb.position + transform.forward * moveSpeed);
        }
        if(Input.GetKeyUp(forward))
        {
            anim.SetBool("Walking", false);
        }
        if(Input.GetKey(left))
        {
            anim.SetBool("Walking", true);
            rb.MovePosition(rb.position - transform.right * moveSpeed);
        }
        if (Input.GetKeyUp(left))
        {
            anim.SetBool("Walking", false);
        }
        if(Input.GetKey(right))
        {
            anim.SetBool("Walking", true);
            rb.MovePosition(rb.position + transform.right * moveSpeed);
        }
        if (Input.GetKeyUp(right))
        {
            anim.SetBool("Walking", false);
        }
        if(Input.GetKey(backward))
        {
            anim.SetFloat("WalkSpeed", -1f);
            anim.SetBool("Walking", true);
            rb.MovePosition(rb.position - transform.forward * moveSpeed);
        }
        if (Input.GetKeyUp(backward))
        {
            anim.SetBool("Walking", false);
        }
        if(Input.GetKeyDown(jump) && grounded && Time.time > IPT)
        {
            rb.AddForce(transform.up * jumpForce);
            IPT = Time.time + 1f;
            anim.Play("Jump/Land");
        }
        if(Input.GetKey(sprint))
        {
            anim.SetBool("Running", true);
            moveSpeed = 0.12f;
        }
        else
        {
            anim.SetBool("Running", false);
            moveSpeed = 0.07f;
        }
        if(!Input.GetKey(forward) && !Input.GetKey(backward) && !Input.GetKey(left) && !Input.GetKey(right))
        {
            anim.SetBool("Walking", false);
        }
            #endregion
        if (!grounded)
        {
            var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            rb.AddTorque(new Vector3(rot.x, 0, rot.z) * 200);
            rb.constraints = RigidbodyConstraints.None;
        }
        else if (grounded)
        {
            rb.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(StartRot.x, transform.eulerAngles.y, StartRot.z), Time.deltaTime * 500);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        yRot = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * lookSens;
        xRot = xRot - Input.GetAxis("Mouse Y") * lookSens;
        xRot = Mathf.Clamp(xRot, camMinY, camMaxY);
        transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, yRot, transform.localEulerAngles.z); //rotate body
        Pcamera.transform.localRotation = Quaternion.Euler(xRot, 0, 0); //rotate camera
    }
}
