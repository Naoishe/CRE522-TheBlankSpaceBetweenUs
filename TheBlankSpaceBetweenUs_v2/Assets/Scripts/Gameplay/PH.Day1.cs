using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using System;

public class PHDay1 : MonoBehaviour
{
    public Image blackLerp;
    static float blackT = 0.0f;
    public Collider2D playerCollider;
    public Collider2D interactingCollider;
    public Collider2D toDownStairs;
    public Collider2D toUpStairs;
    public GameObject player;

    private float minOpacity = -1.0f;
    private float maxOpacity = 1.0f;

    public static Action LeavingHouse;
    void Start()
    {
        player.transform.position = new Vector3(0.7f, -0.9f, 0f);
    }
    void Update()
    {
        if (Physics2D.IsTouching(interactingCollider, playerCollider))
        {
            LeavingHouse?.Invoke();
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

    public void BlackLerpScreen()
    {
        blackLerp.color = new Color(blackLerp.color.r, blackLerp.color.g, blackLerp.color.b, Mathf.Lerp(minOpacity, maxOpacity, blackT));
        blackT += 0.5f * Time.deltaTime;

        if (blackT >= 1.0f)
        {
            SceneManager.LoadScene("Midday");
        }
    }
}
