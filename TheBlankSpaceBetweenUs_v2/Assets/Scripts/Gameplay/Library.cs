using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.UI;

public class Library : MonoBehaviour
{
    public Collider2D toCampus;
    public Collider2D playerCollider;
    public GameObject playerObj;
    public Image nikoImage;
    public YarnCommsLibrary yarnComms;

    public bool nikoImageBoolRead;
    public bool holderBool;

    public static Action ReturnToCampus;
    void Start()
    {
        playerObj.transform.position = new Vector3(-3.83f,-2.57f,0f);
        
    }

    private void OnEnable()
    {
        ContinuousData.ReturnYarnAsTrue += AssignNikoTrue;
        ContinuousData.ReturnYarnAsFalse += AssignNikoFalse;
    }

    private void OnDisable()
    {
        ContinuousData.ReturnYarnAsTrue -= AssignNikoTrue;
        ContinuousData.ReturnYarnAsFalse -= AssignNikoFalse;
    }


    void Update()
    {
        if (Physics2D.IsTouching(toCampus, playerCollider))
        {
            
            ReturnToCampus?.Invoke();
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            ReturnToCampus?.Invoke();
        }

        ContinuousData.instance.FetchYarnBoolVariable("$nikoImageActive", holderBool);
    }

    public void AssignNikoFalse()
    {
        nikoImageBoolRead = false;
        NikoImageStatus();
    }
    public void AssignNikoTrue()
    {
        nikoImageBoolRead = true;
        NikoImageStatus();
    }

    public void NikoImageStatus()
    {
        //ContinuousData.instance.FetchYarnBoolVariable("$nikoImageActive", nikoImageBoolRead);

        Debug.Log("Processed Value: " + nikoImageBoolRead);
        if (nikoImageBoolRead)
        {

            nikoImage.color=new Color(nikoImage.color.r,nikoImage.color.g,nikoImage.color.b,255);
        }
        else
        {
            nikoImage.color = new Color(nikoImage.color.r, nikoImage.color.g, nikoImage.color.b, 0);
        }
    }
}
