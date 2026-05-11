using UnityEngine;

public class SalemInteract : InteractableObject
{
    public GameObject salemDiaImage;
    public bool watchBool;
    public void Start()
    {
        // Cache the Salem dialogue image and hide it on start
        salemDiaImage = GameObject.Find("SalemDiaImage");
        salemDiaImage.SetActive(false);
    }
    public override void Interaction()
    {
        // Trigger Salem dialogue if it's the correct day
        watchBool = true;
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            dialogueRunner.StartDialogue("MeetingSalem");
        }
        else
        {
            // Not day 0
        }
    }

    public override void UpdateExtra()
    {
        if (watchBool)
        {
            WatchImageBool();
        }
    }

    public override void EndSpecifics()
    {
        // End Salem dialogue and clear watch state
        dialogueRunner.Stop();
        watchBool = false;
    }

    public void WatchImageBool()
    {
        salemDiaImage.SetActive(ContinuousData.instance.SalemDiaImageState);
    }
}
