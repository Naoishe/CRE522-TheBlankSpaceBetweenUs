using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    public Player player;
    public int currentCollectables
    {
        get { return player.playerCollectableCounter; }
        set {  player.playerCollectableCounter = value; }
    }
    public Text scoreText;

    private void Start()
    {
        player=player.GetComponent<Player>();
    }

    private void Update()
    {
        scoreText.text = currentCollectables.ToString();
    }
  
}
