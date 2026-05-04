using UnityEngine;
using Yarn.Unity;

public class InteractableObject : MonoBehaviour, Iinteractable
{
    public bool objectActive=false;
    public float distance;
    public bool standardNotifications;

    private GameObject player;
    public DialogueRunner dialogueRunner;

    public void Awake()
    {
        player = GameObject.Find("PlayerObj");
        
    }

    public void OnEnable()
    {
        Player.OnInteractionEnabled += InteractionActivated;
    }
    public void OnDisable()
    {
        Player.OnInteractionEnabled -= InteractionActivated;
    }
    public void InteractionActivated() 
    {
        player = GameObject.Find("PlayerObj");
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        distance = Vector2.Distance(player.transform.position, this.transform.position);
        if (distance<3f)
        {
            //Debug.Log("InteractionActivated_Object: " + this.name);
            objectActive= true;
            Interaction();
        }
        else
        {
            
            objectActive = false; 
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) ///deactivation button
        {
            if (objectActive)
            {
                EndInteraction();
            }
        }

        UpdateExtra();
    }

    public virtual void UpdateExtra() { }
    public virtual void Interaction() { }

    public void EndInteraction()
    {
        objectActive= false;
        Debug.Log("Interaction ENDED. Object: " + this.name);
        EndSpecifics();
    }

    public virtual void EndSpecifics() { }

    
}
