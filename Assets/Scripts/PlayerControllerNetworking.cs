using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public int peaAmmoCnt = 0;
    public int cornAmmoCnt = 0;
    public int wheatAmmoCnt = 0;
    public int sunAmmoCnt = 0;


    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;

    [SyncVar]
    public bool hasFlag = false;
    // pause menu
    public GameObject pauseMenu;
    private Button resumeBtn;
    private Button quitBtn;

    private GameObject outerCamera;

    private CTFManager ctfMan;

    TMPro.TMP_Text redScore;
    TMPro.TMP_Text blueScore;
    void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        ctfMan = GameObject.Find("/NetworkManager").transform.Find("CTFManager").GetComponent<CTFManager>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        
        characterController = GetComponent<CharacterController>();

        LockPlayer(false);

        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }

        // disable outer camera

        // why doesn't unity let me find inactive game objects???? 
        SetPauseMenu();

        if (isLocalPlayer) {
            outerCamera = GameObject.Find("/OuterCamera");
            outerCamera.SetActive(false);
        }

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

       // Debug.Log(transform.position);
    }
    
    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "grenade")
        {

            Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }
        if (Col.gameObject.tag == "ammoPea")
        {
            peaAmmoCnt+=15;
            Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }
        if (Col.gameObject.tag == "ammoWheat")
        {
            wheatAmmoCnt +=15;
            Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }
        if (Col.gameObject.tag == "ammoSun")
        {
            sunAmmoCnt+=15;
            Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }

        if (Col.gameObject.tag == "health")
        {
            IncHealth(20);
            Col.gameObject.SetActive(false);
        }
        else if (Col.gameObject.name == "StartFlag") 
        {
            ctfMan.StartCTF();
        }
        else if (Col.gameObject.tag == "RedFlag")
        {
            if (GetComponent<CTFPlayerManager>().playerTeam == Team.Blue) {
                Destroy(Col.gameObject);
                hasFlag = true;
            }
        }
        else if (Col.gameObject.tag == "BlueFlag")
        {
            if (GetComponent<CTFPlayerManager>().playerTeam == Team.Red) {
                Destroy(Col.gameObject);
                hasFlag = true;
            }
        }
        else if (Col.gameObject.tag == "RedArea")
        {
            if (GetComponent<CTFPlayerManager>().playerTeam == Team.Red && hasFlag) {
                ctfMan.redWins++;
                ctfMan.redCountGUI.text = $"Red Score: {ctfMan.redWins}";
                ctfMan.blueCountGUI.text = $"Blue Score: {ctfMan.blueWins}";

                ResetAllPositions();
            }
        }

        else if (Col.gameObject.tag == "BlueArea")
        {
            if (GetComponent<CTFPlayerManager>().playerTeam == Team.Blue && hasFlag) {
                ctfMan.blueWins++;
                ctfMan.redCountGUI.text = $"Red Score: {ctfMan.redWins}";
                ctfMan.blueCountGUI.text = $"Blue Score: {ctfMan.blueWins}";
                ResetAllPositions();
            }            
        }
    }

    // called upon win. If limit is reached, change scene. 
    public void ResetAllPositions() {
        if (ctfMan.redWins >= ctfMan.winLimit || ctfMan.blueWins >= ctfMan.winLimit) {
            ctfMan.inGame = false;
            ctfMan.StartCTF();
        }
        ctfMan.chosenSpawnPoints = new HashSet<int>();
        foreach(var g in GameObject.FindGameObjectsWithTag("Player")) {
            var pcn = g.GetComponent<PlayerControllerNetworking>();
            pcn.hasFlag = false;
            pcn.SetPosition();
            pcn.currentHealth = maxHealth;
            pcn.healthBar.SetMaxHealth(maxHealth);
        }
    }
    public void KillPlayer() {
        ctfMan.chosenSpawnPoints = new HashSet<int>();
        hasFlag = false;
        SetPosition();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDmg(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) {
            currentHealth = 0;
            KillPlayer();
        }
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
        var pos = ctfMan.GetRandomSpawnLocation2(GetComponent<CTFPlayerManager>().playerTeam).position;

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