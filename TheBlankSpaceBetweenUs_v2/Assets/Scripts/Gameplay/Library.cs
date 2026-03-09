using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Library : MonoBehaviour
{
    public Collider2D toCampus;
    public Collider2D playerCollider;

    public static Action ReturnToCampus;
    void Start()
    {
        
    }

    void Update()
    {
        if (Physics2D.IsTouching(toCampus, playerCollider))
        {
            
            ReturnToCampus?.Invoke();
        }
    }
}
