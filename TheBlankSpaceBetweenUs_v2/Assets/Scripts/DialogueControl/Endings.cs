using TMPro;
using UnityEngine;
using Yarn.Unity;

public class Endings : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public GameObject RiftBG;
    public GameObject WhiteBG;
    public GameObject BlackBG;
    public AudioSource Graduation;
    public AudioSource BadEndings;
    public GameObject nikoImage;
    public GameObject faustImage;
    public GameObject salemImage;
    public GameObject SalemBad;
    public GameObject NikoCannibal;

    public TextMeshProUGUI endingText;
    public YarnProject[] yarnProjects;

    private void Awake()
    {
        // Initialize dialogue runner, audio sources and UI elements for endings
        dialogueRunner = GetComponent<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);
        RiftBG = GameObject.Find("TheRift");
        WhiteBG = GameObject.Find("WhiteBG");
        BlackBG = GameObject.Find("BlackBG");
        Graduation = GameObject.Find("Graduation").GetComponent<AudioSource>();
        BadEndings = GameObject.Find("BadEnding").GetComponent<AudioSource>();
        nikoImage = GameObject.Find("NikoImage");
        faustImage = GameObject.Find("FaustImage");
        salemImage = GameObject.Find("SalemImage");
        SalemBad = GameObject.Find("SalemBad");
        NikoCannibal = GameObject.Find("NikoCannibalism");

        RiftBG.SetActive(false);
        WhiteBG.SetActive(false);
        BlackBG.SetActive(false);
        nikoImage.SetActive(false);
        faustImage.SetActive(false);
        salemImage.SetActive(false);
        SalemBad.SetActive(false);
        NikoCannibal.SetActive(false);
        endingText.text = "";

    }

    void Start()
    {
        switch (ContinuousData.instance.EndingIndex)
        {
            case 0:
                EndingGraduation();
                break;
            case 1:
                EndingGoodNiko();
                break;
            case 2:
                GoodFaust();
                break;
            case 3:
                SalemRift();
                break;
            case 4:
                GameFinished();
                break;
            case 5:
                NikoEvent();
                break;
            case 6:
                EndingGraduationWithFaust();
                break;
            case 7:
                GameFinished();
                break;
            default:
                GameFinished();
                break;
        }

        // Register dialogue command handlers used in ending sequences
        dialogueRunner.AddCommandHandler("gameFinished", GameFinished);
        dialogueRunner.AddCommandHandler("moodSwitch", MoodSwitch);
        dialogueRunner.AddCommandHandler("darkScreen", DarkScreen);
        dialogueRunner.AddCommandHandler("whiteScreen", WhiteScreen);



    }

    public void DarkScreen()
    {
        // Show a black full-screen background
        BlackBG.SetActive(true);
    }

    public void WhiteScreen()
    {
        // Show a white full-screen background
        WhiteBG.SetActive(true);
    }

    public void MoodSwitch()
    {
        // Stop the graduation music
        Graduation.Stop();

    }

    public void EndingGraduation()
    {
        // Play graduation ending: background and dialogue
        BlackBG.SetActive(true);
        Graduation.Play();
        Graduation.loop = true;
        dialogueRunner.StartDialogue("Graduation");
    }

    public void EndingGraduationWithFaust()
    {
        // Play graduation ending variant with Faust
        BlackBG.SetActive(true);
        Graduation.Play();
        Graduation.loop = true;
        dialogueRunner.StartDialogue("GraduationWithFaust");
    }

    public void EndingGoodNiko()
    {
        // Trigger the Good Niko ending dialogue and visuals
        dialogueRunner.StartDialogue("GoodNiko");
        BadEndings.Play();
        nikoImage.SetActive(true);
    }

    public void GoodFaust()
    {
        // Trigger the Faust good ending dialogue
        dialogueRunner.StartDialogue("GoodFaust");
    }

    public void SalemRift()
    {
        // Trigger the Salem rift ending sequence
        BadEndings.Play();
        dialogueRunner.StartDialogue("SalemEndStart");
    }

    public void NikoEvent()
    {
        // Trigger the Niko event ending sequence
        BadEndings.Play();
        dialogueRunner.StartDialogue("NikoEventStart");
    }

    public void DetermineNikoEnd()
    {
        // Choose Niko's final scene based on relationship points
        if (ContinuousData.instance.NikoRP > 0)
        {
            dialogueRunner.StartDialogue("StockholmSyndromeEndPart");
            nikoImage.SetActive(true);
        }
        else
        {
            dialogueRunner.StartDialogue("CannibalismEnd");
            NikoCannibal.SetActive(true);
        }
    }

    public void GameFinished()
    {
        // Clear ending visuals and display final message
        RiftBG.SetActive(false);
        WhiteBG.SetActive(false);
        BlackBG.SetActive(false);
        nikoImage.SetActive(false);
        faustImage.SetActive(false);
        salemImage.SetActive(false);
        SalemBad.SetActive(false);
        NikoCannibal.SetActive(false);
        endingText.text = "Congratulations! You've reached the true ending of the game! Thank you for playing! You achieved ending number " + ContinuousData.instance.EndingIndex;

    }
}
