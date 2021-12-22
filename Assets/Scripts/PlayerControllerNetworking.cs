using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerNetworking: NetworkBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;


    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    public HealthBar healthBar;
    public int maxHealth = 100;
    [SyncVar] public int currentHealth;
    // pause menu
    public GameObject pauseMenu;
    private Button resumeBtn;
    private Button quitBtn;

    private GameObject outerCamera;
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        
        characterController = GetComponent<CharacterController>();

        LockPlayer(false);

        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }

        // disable outer camera

        outerCamera = transform.Find("/OuterCamera").gameObject;
        outerCamera.SetActive(false);

        // why doesn't unity let me find inactive game objects???? 
        SetPauseMenu();

    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!NetworkClient.isConnected && canMove) {
            LockPlayer(true);

            outerCamera.SetActive(true);
        } 

        
        //For test purposes, can be changed afterwards to work with gun damage 
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDmg(20);
        }
        

        if (Input.GetButton("Pause")) {
            // bad code but it works
            SetPauseMenu();
            pauseMenu.SetActive(true);
            LockPlayer(true);
        }

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

    }
    
    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "health")
        {
            IncHealth(20);
            
            Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }
        else if (Col.gameObject.name == "StartFlag") 
        {
            GameObject.Find("/NetworkManager").GetComponent<CTFManager>().StartCTF();
        }
        
    }

    public void TakeDmg(int dmg)
    {
        currentHealth -= dmg;
        healthBar.SetHealth(currentHealth);
    }

    public void IncHealth(int inc)
    {
        currentHealth += inc;
        healthBar.SetHealth(currentHealth);
    }
    
    public void SetPauseMenu() {
        pauseMenu = GameObject.Find("Canvas").transform.Find("PauseMenu").gameObject;
        resumeBtn = pauseMenu.transform.Find("ResumeButton").GetComponent<Button>();

        resumeBtn.onClick.AddListener(() => {
            pauseMenu.SetActive(false);
            LockPlayer(false);
        });
    }

    public void SetPosition() {
        var pos = GameObject.Find("/NetworkManager").GetComponent<CTFManager>().GetRandomSpawnLocation(true, GetComponent<CTFPlayerManager>().playerTeam).position;

        // character controller messes up teleporting, disable then move then re-enable.
        //https://forum.unity.com/threads/unity-multiplayer-through-mirror-teleporting-player-inconsistent.867079/
        GetComponent<CharacterController>().enabled = false;
        transform.position = pos;
        GetComponent<CharacterController>().enabled = true;
    }

    public void LockPlayer(bool isLock) {
            // lock mouse again
            Cursor.lockState = isLock ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isLock;
            // can move again
            canMove = !isLock;
    }
}