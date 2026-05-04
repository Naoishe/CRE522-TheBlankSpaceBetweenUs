using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;
using TMPro;

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

    private void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
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
        switch(ContinuousData.instance.EndingIndex)
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
             default:
                EndingGraduation();
                break;
        }

        dialogueRunner.AddCommandHandler("gameFinished", GameFinished);
        dialogueRunner.AddCommandHandler("moodSwitch", MoodSwitch);
        dialogueRunner.AddCommandHandler("darkScreen", DarkScreen);
        dialogueRunner.AddCommandHandler("whiteScreen", WhiteScreen);



    }

    public void DarkScreen()
    {
        BlackBG.SetActive(true);
    }

    public void WhiteScreen()
    {
        WhiteBG.SetActive(true);
    }

    public void MoodSwitch()
    {
        Graduation.Stop();

    }

    public void EndingGraduation()
    {
        BlackBG.SetActive(true);
        Graduation.Play();
        Graduation.loop = true;
        dialogueRunner.StartDialogue("Graduation");
    }

    public void EndingGraduationWithFaust()
    {
        BlackBG.SetActive(true);
        Graduation.Play();
        Graduation.loop = true;
        dialogueRunner.StartDialogue("GraduationWithFaust");
    }

    public void EndingGoodNiko()
    {
        dialogueRunner.StartDialogue("GoodNiko");
        BadEndings.Play();
        nikoImage.SetActive(true);
    }

    public void GoodFaust()
    {
        dialogueRunner.StartDialogue("GoodFaust");
    }

    public void SalemRift()
    {
        BadEndings.Play();
        dialogueRunner.StartDialogue("SalemRift");
    }

    public void NikoEvent()
    {
        BadEndings.Play();
        dialogueRunner.StartDialogue("NikoEventStart");
    }

    public void DetermineNikoEnd()
    {
        if(ContinuousData.instance.NikoRP>0)
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
