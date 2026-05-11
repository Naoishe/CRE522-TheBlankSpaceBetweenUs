public class ObjectCounters : InteractableObject
{
    PlayerHouse playerHouse;


    public override void Interaction()
    {
        playerHouse = FindObjectOfType<PlayerHouse>();
        if (playerHouse != null)
        {
            if (playerHouse.breakfastDone == false)
            {
                dialogueRunner.StartDialogue("CookingBreakfast");
                playerHouse.UpdateBreakfastStatus(true);
            }
        }

    }

    public override void EndSpecifics()
    {

    }
}
