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

    private bool nikoImageBoolRead;

    public static Action ReturnToCampus;
    void Start()
    {
        playerObj.transform.position = new Vector3(-3.83f,-2.57f,0f);
        
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

      

        NikoImageStatus();
    }

    public void NikoImageStatus()
    {
        ContinuousData.instance.FetchYarnBoolVariable("$nikoImageActive", nikoImageBoolRead);

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
