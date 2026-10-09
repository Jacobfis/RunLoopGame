
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Status")]
    public bool puzzle1Solved = false;
    public bool puzzle2Solved = false;
    public bool puzzle3Solved = false;

    [Header("Win")]
    public GameObject winWall;

    private bool levelUnlocked = false;

    void Start()
    {
        UpdateWinWall();
    }

    void Update()
    {
        UpdateWinWall();
    }

    public void SetPuzzle1(bool solved)
    {
        puzzle1Solved = solved;
        UpdateWinWall();
    }

    public void SetPuzzle2(bool solved)
    {
        puzzle2Solved = solved;
        UpdateWinWall();
    }

    public void SetPuzzle3(bool solved)
    {
        puzzle3Solved = solved;
        UpdateWinWall();
    }

    void UpdateWinWall()
    {
        bool allSolved =
            puzzle1Solved &&
            puzzle2Solved &&
            puzzle3Solved;

        if (allSolved && !levelUnlocked)
        {
            levelUnlocked = true;

            if (winWall != null)
                winWall.SetActive(false);

            Debug.Log("ALLE PUZZLES KLARET! WINWALL FJERNET!");
        }
    }

    public void ResetPuzzles()
    {
        puzzle1Solved = false;
        puzzle2Solved = false;
        puzzle3Solved = false;

        levelUnlocked = false;

        if (winWall != null)
            winWall.SetActive(true);

        Debug.Log("Puzzles reset - WinWall tilbage!");
    }
}
