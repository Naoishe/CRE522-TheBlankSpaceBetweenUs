using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

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
