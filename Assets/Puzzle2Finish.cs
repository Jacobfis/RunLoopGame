
using UnityEngine;

public class Puzzle2Finish : MonoBehaviour
{
    [Header("References")]
    public PuzzleManager puzzleManager;

    [Header("Visual")]
    public Renderer buttonRenderer;
    public Color inactiveColor = Color.red;
    public Color completedColor = Color.green;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip completeSound;

    private bool completed = false;

    void Start()
    {
        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<PuzzleManager>();

        if (buttonRenderer == null)
            buttonRenderer = GetComponent<Renderer>();

        UpdateVisual();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return;

        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            CompletePuzzle();
        }
    }

    public void GhostEnter(GameObject ghost)
    {
        if (ghost != null)
            CompletePuzzle();
    }

    void CompletePuzzle()
    {
        if (completed)
            return;

        completed = true;

        if (puzzleManager != null)
            puzzleManager.SetPuzzle2(true);

        if (audioSource != null && completeSound != null)
            audioSource.PlayOneShot(completeSound);

        UpdateVisual();

        Debug.Log("PUZZLE 2 KLARET!");
    }

    void UpdateVisual()
    {
        if (buttonRenderer != null)
            buttonRenderer.material.color =
                completed ? completedColor : inactiveColor;
    }

    public void ResetPuzzle()
    {
        completed = false;
        UpdateVisual();

        if (puzzleManager != null)
            puzzleManager.SetPuzzle2(false);
    }
}
