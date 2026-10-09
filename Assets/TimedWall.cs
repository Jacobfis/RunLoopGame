using UnityEngine;

public class TimedWall : MonoBehaviour
{
    [Header("Settings")]
    public float appearAfterSeconds = 15f;

    [Header("References")]
    public GameObject wall;

    private TimeLoopManager loopManager;

    void Start()
    {
        loopManager =
            FindFirstObjectByType<TimeLoopManager>();

        if (wall == null)
            wall = gameObject;
    }

    void Update()
    {
        if (loopManager == null)
            return;

        float timePassed =
            loopManager.loopDuration -
            loopManager.GetTimeRemaining();

        bool shouldBeActive =
            timePassed >= appearAfterSeconds;

        // Vi kan ikke deaktivere GameObjectet
        // som dette script selv sidder på,
        // så slå render/collider til og fra.
        SetWallActive(shouldBeActive);
    }

    void SetWallActive(bool active)
    {
        Renderer[] renderers =
            wall.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
            rend.enabled = active;

        Collider[] colliders =
            wall.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
            col.enabled = active;
    }
}