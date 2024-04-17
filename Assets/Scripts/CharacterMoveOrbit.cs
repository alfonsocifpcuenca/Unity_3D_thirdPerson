using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMoveOrbit : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Movement")]
    [SerializeField]
    private float walkMultiplier;
    [SerializeField]
    private float runMultiplier;
    [SerializeField]
    private float rotationSensivity;
    [SerializeField]
    private float jumpForce;
    
    [Header("Gravity")]
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float gravityMultiplier;

    [Header("Energy")]
    [SerializeField]
    private bool energySystem;
    [SerializeField]
    private Slider energySliderUI;
    private bool isRunning = false;
    private float characterEnergy = 0.5f;
    private float fatigueRate = 0.5f;
    private float recoveryRate = 0.1f;


    private Vector3 moveDirection;
    private float floorDistanceFromFoot = 0f;
    private float mouseX;
    private float orbitSpeed;

    void Start()
    {
        this.characterController = GetComponent<CharacterController>();
        this.floorDistanceFromFoot = this.characterController.stepOffset + this.characterController.skinWidth + this.characterController.height / 2;
    }
    
    void Update()
    {
        this.AddGravity();
        this.AddJump();
        this.AddMove();
        this.AddLateralMove();
        this.AddRotation();
        this.AddRun();

        // Rotate player (Obsolete)
        //if (this.moveDirection.x != 0f && this.moveDirection.z != 0f)
        //{
        //    Vector3 rotateDirection = new Vector3(this.moveDirection.x, 0f, this.moveDirection.z);
        //    Quaternion newRotation = Quaternion.LookRotation(rotateDirection);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 1000f * Time.deltaTime);
        //}


        this.transform.Rotate(Vector3.up, this.mouseX);


        // Move player
        this.characterController.Move(this.moveDirection * Time.deltaTime);

        // Update UI
        this.UpdateUI();
    }

    private void AddJump()
    {
        if (Input.GetButtonDown("Jump") && this.IsGrounded())
        {
            this.moveDirection.y = this.jumpForce;
        }
    }

    private void AddGravity()
    {
        if (this.characterController.isGrounded && this.moveDirection.y < 0)
        {
            this.moveDirection.y = -1f;
        } 
        else 
        { 
            this.moveDirection.y += this.gravity * this.gravityMultiplier * Time.deltaTime;
        }
    }

    private void AddMove()
    {
        var moveZ = Input.GetAxisRaw("Vertical");

        Vector3 newDirection = Camera.main.transform.forward;
        Vector3 newDirectionXZ = new Vector3(newDirection.x, 0f, newDirection.z).normalized;
        this.moveDirection.x = newDirectionXZ.x * moveZ;
        this.moveDirection.z = newDirectionXZ.z * moveZ;
    }

    private void AddRotation()
    {
        this.mouseX = Input.GetAxisRaw("Mouse X");
    }

    private void AddLateralMove()
    {
        var moveX = Input.GetAxisRaw("Horizontal");

        if (moveX != 0)
        {
            Vector3 newDirectionPerpendicular = Vector3.Cross(this.transform.forward, Vector3.up);
            Vector3 newDirectionXZ = new Vector3(newDirectionPerpendicular.x, 0f, newDirectionPerpendicular.z).normalized;
            var moveDirectionX = newDirectionXZ.x * -moveX;
            var moveDirectionZ = newDirectionXZ.z * -moveX;

            this.moveDirection.x += moveDirectionX;
            this.moveDirection.z += moveDirectionZ;
        }
    }

    private void AddRun()
    {
        if (this.energySystem && this.energySliderUI != null)
        {
            if (Input.GetButton("Fire1") && (this.characterEnergy > 0.2f || this.isRunning) && this.moveDirection.x != 0 && this.moveDirection.z != 0)
            {
                this.isRunning = true;
                isRunning = true;
                this.characterEnergy -= fatigueRate * Time.deltaTime;
                this.characterEnergy = Mathf.Clamp(this.characterEnergy, 0f, 1f);
                if (this.characterEnergy < 0.01f)
                    this.isRunning = false;
            }
            else
            {
                this.isRunning = false;
                this.characterEnergy += recoveryRate * Time.deltaTime;
                this.characterEnergy = Mathf.Clamp(this.characterEnergy, 0f, 1f);
            }
        }

        var moveMultiplier = this.walkMultiplier;
        if (this.isRunning)
        {
            moveMultiplier = this.runMultiplier;
        }

        this.moveDirection = new Vector3(this.moveDirection.x * moveMultiplier, this.moveDirection.y, this.moveDirection.z * moveMultiplier);
    }

    private void UpdateUI()
    {
        this.energySliderUI.value = this.characterEnergy;
    }

    private bool IsGrounded()
    {
        if (characterController.isGrounded)
            return true;
             
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, this.floorDistanceFromFoot))
            return true;

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawRay(this.transform.position, this.transform.forward * 2f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, Vector3.Cross(this.transform.forward, Vector3.up) * 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position, Camera.main.transform.forward * 2f);
    }
}
