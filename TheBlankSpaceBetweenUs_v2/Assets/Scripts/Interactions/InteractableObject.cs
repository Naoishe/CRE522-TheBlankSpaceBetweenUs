using UnityEngine;

public class InteractableObject : MonoBehaviour, Iinteractable
{
    

    public GameObject thisObject;
    public bool objectActive=false;
    public float distance;
    public bool standardNotifications;

    private GameObject player;

    public void Awake()
    {
        player = GameObject.Find("PlayerObj");
        distance= Vector2.Distance(player.transform.position, thisObject.transform.position);
    }
    public void InteractionActivated(GameObject gameObject) 
    {
        Debug.Log("Interaction Activated. Object: "+thisObject.name);
        objectActive= true;
        if(ContinuousData.instance.currentlyInteracting)
        {
            Debug.Log("ERROR: Interaction attempted while another interaction is active. Object: " + thisObject.name);
        }
        else
        {
            Interaction();
        }
            
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) ///temp deactivation button until dialogue is added
        {
            if (objectActive)
            {
                EndInteraction();
            }
        }
    }
    public virtual void Interaction() { }

    public void EndInteraction()
    {
        objectActive= false;
        Debug.Log("Interaction ENDED. Object: " + thisObject.name);
    }

    public virtual void EndSpecifics() { }

    
    


}
