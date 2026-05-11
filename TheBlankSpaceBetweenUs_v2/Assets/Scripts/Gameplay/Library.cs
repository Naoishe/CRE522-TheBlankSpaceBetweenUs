using System;
using UnityEngine;

public class Library : MonoBehaviour
{
    public Collider2D toCampus;
    public Collider2D playerCollider;
    public GameObject playerObj;


    public static Action ReturnToCampus;
    void Update()
    {
        if (Physics2D.IsTouching(toCampus, playerCollider))
        {

            ContinuousData.instance.SceneChangeDetected("CampusGrounds", ContinuousData.instance.campusGrounds_LibrarySpawn);
        }

    }



}
