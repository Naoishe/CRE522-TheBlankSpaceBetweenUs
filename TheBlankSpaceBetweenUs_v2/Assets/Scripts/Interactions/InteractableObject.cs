using UnityEngine;

public class InteractableObject : EmptyMonoBehaviour, Iinteractable
{
    

    public GameObject thisObject;
    public bool objectActive=false;
    public float distance;
    public bool standardNotifications;

    public void InteractionActivated(GameObject gameObject) 
    {
        Debug.Log("Interaction Activated. Object: "+thisObject.name);
        objectActive= true;
        Interaction();
        
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
