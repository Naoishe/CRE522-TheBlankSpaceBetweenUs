using UnityEngine;

public class FaustInteract : InteractableObject
{
    public GameObject faustDiaImage;
    public bool watchBool;
    public void Start()
    {
        faustDiaImage = GameObject.Find("FaustDiaImage");
        faustDiaImage.SetActive(false);
    }
    public override void Interaction()
    {
        watchBool = true;
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            dialogueRunner.StartDialogue("MeetingFaust");
        }
        else
        {

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
        dialogueRunner.Stop();
        watchBool = false;
    }

    public void WatchImageBool()
    {
        faustDiaImage.SetActive(ContinuousData.instance.FaustDiaImageState);
    }
}
