using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHouse : MonoBehaviour
{
    public Collider2D playerCollider;
    public Collider2D leavingCollider;
    public Collider2D toDownStairs;
    public Collider2D toUpStairs;
    public GameObject player;

    public static Action LeavingHouse;
    void Start()
    {
        player.transform.position = new Vector3(0.7f, -0.9f, 0f);
    }
    void Update()
    {
        if (Physics2D.IsTouching(leavingCollider, playerCollider))
        {
            ContinuousData.instance.SceneChangeDetected("Midday",ContinuousData.instance.campusGrounds_BridgeSpawn);
        }
        if (Physics2D.IsTouching(toDownStairs, playerCollider))
        {
            player.transform.position = new Vector3(8.1f, -18.9f, 0f);
        }
        if (Physics2D.IsTouching(toUpStairs, playerCollider))
        {
            player.transform.position = new Vector3(-12.3f, -1.9f, 0f);
        }

    }
}
