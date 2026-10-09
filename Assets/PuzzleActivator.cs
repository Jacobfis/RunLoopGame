using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public enum PuzzleNumber
    {
        Puzzle1,
        Puzzle2,
        Puzzle3
    }

    [Header("Puzzle")]
    public PuzzleNumber puzzle;

    [Header("Reference")]
    public PuzzleManager puzzleManager;

    [Header("Settings")]
    public bool ghostCanActivate = true;

    void OnTriggerEnter(Collider other)
    {
        bool isPlayer =
            other.CompareTag("Player");

        GhostReplay ghost =
            other.GetComponentInParent<GhostReplay>();

        bool isGhost =
            ghost != null;

        if (!isPlayer &&
            !(ghostCanActivate && isGhost))
        {
            return;
        }

        SolvePuzzle();
    }

    void SolvePuzzle()
    {
        if (puzzleManager == null)
            return;

        switch (puzzle)
        {
            case PuzzleNumber.Puzzle1:
                puzzleManager.SetPuzzle1(true);
                break;

            case PuzzleNumber.Puzzle2:
                puzzleManager.SetPuzzle2(true);
                break;

            case PuzzleNumber.Puzzle3:
                puzzleManager.SetPuzzle3(true);
                break;
        }

        Debug.Log(puzzle + " KLARET!");
    }
}