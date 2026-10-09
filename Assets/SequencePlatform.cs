
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencePlatform : MonoBehaviour
{
    [Header("Puzzle")]
    public int platformNumber = 1;
    public SequencePuzzle puzzle;

    [Header("Visual")]
    public Renderer platformRenderer;
    public Color normalColor = Color.white;
    public Color pressedColor = Color.green;
    public float flashDuration = 0.25f;

    private readonly HashSet<GameObject> objectsOnPlatform =
        new HashSet<GameObject>();

    private Coroutine flashRoutine;

    void Start()
    {
        if (puzzle == null)
            puzzle = FindFirstObjectByType<SequencePuzzle>();

        if (platformRenderer == null)
            platformRenderer = GetComponent<Renderer>();

        if (platformRenderer != null)
            platformRenderer.material.color = normalColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return;

        GameObject player = GetPlayer(other);

        if (player != null)
            EnterPlatform(player);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return;

        GameObject player = GetPlayer(other);

        if (player != null)
            EnterPlatform(player);
    }

    void OnTriggerExit(Collider other)
    {
        GameObject player = GetPlayer(other);

        if (player != null)
            ExitPlatform(player);
    }

    GameObject GetPlayer(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return null;

        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            return other.transform.root.gameObject;
        }

        return null;
    }

    void EnterPlatform(GameObject activator)
    {
        if (activator == null)
            return;

        if (!objectsOnPlatform.Add(activator))
            return;

        if (puzzle != null)
            puzzle.PressPlatform(platformNumber);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(Flash());
    }

    void ExitPlatform(GameObject activator)
    {
        objectsOnPlatform.Remove(activator);
    }

    public void GhostEnter(GameObject ghost)
    {
        EnterPlatform(ghost);
    }

    public void GhostExit(GameObject ghost)
    {
        ExitPlatform(ghost);
    }

    public void ForceReset()
    {
        objectsOnPlatform.Clear();

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (platformRenderer != null)
            platformRenderer.material.color = normalColor;
    }

    IEnumerator Flash()
    {
        if (platformRenderer != null)
            platformRenderer.material.color = pressedColor;

        yield return new WaitForSeconds(flashDuration);

        if (platformRenderer != null)
            platformRenderer.material.color = normalColor;

        flashRoutine = null;
    }
}
