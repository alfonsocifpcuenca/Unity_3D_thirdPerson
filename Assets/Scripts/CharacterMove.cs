using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMove : MonoBehaviour
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

    void Start()
    {
        this.characterController = GetComponent<CharacterController>();
        this.floorDistanceFromFoot = this.characterController.stepOffset + this.characterController.skinWidth + this.characterController.height / 2;
    }
    
    void Update()
    {
        this.AddGravity();
        this.AddJump();
        this.AddRotate();
        this.AddMove();
        this.AddRun();

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

    private void AddRotate()
    {
        var moveX = Input.GetAxisRaw("Horizontal");
        this.transform.Rotate(Vector3.up, moveX * this.rotationSensivity);
    }

    private void AddMove()
    {
        var moveZ = Input.GetAxisRaw("Vertical");

        Vector3 newDirection = this.transform.forward * -1;
        this.moveDirection.x = newDirection.x * moveZ;
        this.moveDirection.z = newDirection.z * moveZ;
    }

    private void AddRun()
    {
        if (this.energySystem && this.energySliderUI != null)
        {
            if (Input.GetButton("Fire1") && (this.characterEnergy > 0.2f || this.isRunning))
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
}
