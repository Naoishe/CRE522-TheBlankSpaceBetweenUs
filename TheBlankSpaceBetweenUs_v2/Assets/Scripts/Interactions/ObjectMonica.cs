public class ObjectMonica : InteractableObject
{

    public override void Interaction()
    {
        dialogueRunner.StartDialogue("Monica");
    }

    public override void EndSpecifics()
    {
        dialogueRunner.Stop();
    }


}
