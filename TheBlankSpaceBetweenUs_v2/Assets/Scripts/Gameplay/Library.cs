using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class Library : MonoBehaviour
{
    public Collider2D toCampus;
    public Collider2D playerCollider;
    public GameObject playerObj;
    public GameObject nikoImage;
    //public GameObject DialogueRef;
    //public YarnTask DiaRef;
    private bool toggleTempFix;

    public static Action ReturnToCampus;
    void Start()
    {
        playerObj.transform.position = new Vector3(-3.83f,-2.57f,0f);
        toggleTempFix = false;
    }


    void Update()
    {
        if (Physics2D.IsTouching(toCampus, playerCollider))
        {
            
            ReturnToCampus?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.T)) 
        {
            toggleTempFix = !toggleTempFix;
        }

        if (toggleTempFix)
        {
            nikoImage.SetActive(true);
        }
        else
        {
            nikoImage.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ReturnToCampus?.Invoke();
        }

        /*if (ContinuousData.instance.nikoImagebool)
        {
            nikoImage.SetActive(true);
        }
        else
        {
            nikoImage.SetActive(false);
        }*/

        //DiaRef.DialogueRunner.StartDialogue("MeetingNiko");
    }
}
