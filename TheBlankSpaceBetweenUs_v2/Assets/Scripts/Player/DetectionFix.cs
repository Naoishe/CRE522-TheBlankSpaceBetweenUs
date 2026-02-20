using UnityEngine;

public class DetectionFix : MonoBehaviour
{
    public static DetectionFix Instance;
    [SerializeField] GameObject[] characters;
    [SerializeField] GameObject player;
    public GameObject closestChar;
    private Vector2 currentObjectVector;
    private float shortestDistance;

    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {
        Check();
    }

    public void Check()
    {
        for (int i = 0; i > characters.Length; i++)
        {
            if (i == 0)
            {
                closestChar = characters[0].GetComponent<GameObject>();
                shortestDistance = Vector2.Distance(player.transform.position, closestChar.transform.position);
            }
            else
            {
                currentObjectVector = characters[i].transform.position;
                float comparingDistance = Vector2.Distance(player.transform.position, currentObjectVector);
                if (comparingDistance < shortestDistance)
                {
                    shortestDistance = comparingDistance;
                    closestChar = characters[i].GetComponent<GameObject>();
                }
            }


        }
    }
}
