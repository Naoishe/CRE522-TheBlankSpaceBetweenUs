using UnityEngine;
using System;

public class Player : MonoBehaviour 
{

    public static Action OnMinigameInput;
    public static Action OnInteractionEnabled;

    delegate void PlayerDelegate();
    PlayerDelegate playerDelegate;

    private float privStamina=0;
    private string privName;
    private bool staminaDepleted;
    private Vector2 playerLocation;
    private ContactFilter2D contactFilter;
    public float searchRadius=10;

    [SerializeField] public GameObject player;
    [SerializeField] public Animator animator;
    public Collider2D[] collidingObjects;
    public GameObject currentlyInteractingObject;
    private Vector2 currentObjectVector;
    private float shortestDistance;

    [Header("Movement")]
    public float walkSpeed=5f;
    public float sprintSpeed;

    public bool isWalking = false;

    public int playerCollectableCounter;
    

    //public Animator anim;
    private Vector2 movementInput;
    Rigidbody2D rb;
    public float stamina
    {
        get { return privStamina; }
        set { 
            privStamina = value;
            if (privStamina < 0)
            {
                staminaDepleted= true;
            }
            else
            {
                staminaDepleted= false;
            }
            privStamina = Mathf.Clamp(privStamina, 0, 100);
        }
    }

    public string name
    {
        get { return privName; }
        set { privName = value; }
    }

    public Player()
    {
        //constructor
    }

    private void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
        playerDelegate += MyInput;
        playerDelegate += SpeedControl;
        playerDelegate += PlayerButtons;
        OnInteractionEnabled += InteractionCheck;
        playerCollectableCounter = 0;


        
    }
    public void Update()
    {
        playerDelegate();
        playerLocation=this.transform.position;

    }

    public void PlayerButtons()
    {
        /*if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractionEnabled?.Invoke();
        }*/
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnMinigameInput?.Invoke();
        }
        
        
        
    }

    public void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput.Normalize();

        if (movementInput.magnitude > 0)
        {
            animator.SetBool("isWalking", true);

        }
        else
        {
            animator.SetBool("isWalking",false);
        }
    }

    public void MovePlayer()
    {
        if(rb!= null)
        {
            rb.velocity = movementInput * walkSpeed;
            
        }
        else
        {
            rb = player.GetComponent<Rigidbody2D>();
            rb.velocity = movementInput * walkSpeed;
        }

        /*if (rb.velocity.magnitude > 0.01f)
        {
            anim.SetBool("isWalking", true); //Controls animation bopping if walking.

        }
        else
        {
            
                anim.SetBool("isWalking", false);
            
        }
        */

    }

    public void SpeedControl()
    {
        Vector2 flatVel = new Vector2(rb.velocity.x, rb.velocity.y);

        //limit velocity if needed
        if (flatVel.magnitude > walkSpeed)
        {
            Vector2 limitedVel = flatVel.normalized * walkSpeed;
            rb.velocity = new Vector2(limitedVel.x, limitedVel.y);
        }
    }

    public void InteractionCheck()
    {
        IORadiusCheck();
        if(currentlyInteractingObject != null)
        {
            Debug.Log("Interaction Enabled On: " + gameObject.name);
            currentlyInteractingObject.GetComponent<InteractableObject>().InteractionActivated(currentlyInteractingObject);
        }
        else
        {
            Debug.Log("No Object Located");
        }

    }

    private void IORadiusCheck()
    {
        Physics2D.OverlapCircle(playerLocation, searchRadius, contactFilter, collidingObjects);
        if (collidingObjects != null)
        {
            for (int i=0; i > collidingObjects.Length; i++)
            {
                if (i == 0)
                {
                    currentlyInteractingObject = collidingObjects[0].GetComponent<GameObject>();
                    shortestDistance = Vector2.Distance(player.transform.position, currentlyInteractingObject.transform.position);
                }
                else
                {
                    currentObjectVector = collidingObjects[i].transform.position;
                    float comparingDistance = Vector2.Distance(player.transform.position, currentObjectVector);
                    if (comparingDistance < shortestDistance)
                    {
                        shortestDistance = comparingDistance;
                        currentlyInteractingObject = collidingObjects[i].GetComponent<GameObject>();
                    }
                }
                
                
            }
        }
        else
        {
            //currentlyInteractingObject = null;
            if (ContinuousData.instance.currentSceneName == "CampusGrounds")
            {
                currentlyInteractingObject = DetectionFix.Instance.closestChar;
            }
            else
            {
                currentlyInteractingObject = null;
            }
        }

        if(currentlyInteractingObject==null && ContinuousData.instance.currentSceneName == "CampusGrounds")
        {
            currentlyInteractingObject=DetectionFix.Instance.closestChar;
        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            if (playerCollectableCounter != null)
            {
                playerCollectableCounter++;
            }
       

        }
    }

    
  

}
