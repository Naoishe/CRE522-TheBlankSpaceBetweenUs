using UnityEngine;

/// <summary>
/// Reliable campus door: <see cref="OnTriggerEnter2D"/> + <see cref="Collider2D.CompareTag"/> (or PlayerObj name).
/// Assign to a <see cref="Collider2D"/> with <c>Is Trigger</c> on (e.g. <c>ToLibrary</c>).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CampusSceneDoorTrigger2D : MonoBehaviour
{
    public enum DoorAction
    {
        /// <summary>Calls <see cref="ContinuousData.LoadScene"/> (bypasses Yarn).</summary>
        LoadSceneImmediately,
        /// <summary>Runs the Yarn node via <see cref="CampusGrounds"/> (e.g. EnterLibrary).</summary>
        StartYarnDialogue,
    }

    [Tooltip("Must match a tag on the player (set PlayerObj to tag Player).")]
    public string playerTag = "Player";

    [Tooltip("If false, accepts colliders on a root/root object named PlayerObj.")]
    public bool useTag = true;

    public DoorAction action = DoorAction.LoadSceneImmediately;

    [Tooltip("Build Settings scene name, e.g. Library")]
    public string sceneName = "Library";

    [Tooltip("Yarn node name when action is StartYarnDialogue.")]
    public string yarnNode = "EnterLibrary";

    [Tooltip("Optional; otherwise FindObjectOfType at enter.")]
    public CampusGrounds campusGrounds;

    bool _playerInside;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null)
            c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_playerInside || !IsOurPlayer(other))
            return;
        _playerInside = true;

        var cd = ContinuousData.instance;
        if (cd == null)
        {
            Debug.LogError("CampusSceneDoorTrigger2D: No ContinuousData instance.", this);
            return;
        }

        if (action == DoorAction.LoadSceneImmediately)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("CampusSceneDoorTrigger2D: sceneName is empty.", this);
                return;
            }
            cd.AllowPlayerToMove();
            cd.LoadScene(sceneName);
            return;
        }

        var cg = campusGrounds != null ? campusGrounds : FindObjectOfType<CampusGrounds>();
        if (cg == null)
        {
            Debug.LogError("CampusSceneDoorTrigger2D: No CampusGrounds for Yarn mode.", this);
            return;
        }
        cg.OpenDoorDialogueFromTrigger(yarnNode, playNotification: false);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsOurPlayer(other))
            _playerInside = false;
    }

    bool IsOurPlayer(Collider2D other)
    {
        if (other == null)
            return false;
        if (useTag && !string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag))
            return true;
        var go = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        return go.name == "PlayerObj" || go.transform.root.name == "PlayerObj";
    }
}
