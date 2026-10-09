using UnityEngine;
using TMPro;

public class SequencePuzzle : MonoBehaviour
{
    [Header("References")]
    public PuzzleManager puzzleManager;
    public TMP_Text puzzle1Text;

    [Header("Settings")]
    public int[] correctOrder = new int[9];

    private int currentStep = 0;
    private bool solved = false;

    void Start()
    {
        GenerateNewSequence();
    }

    public void GenerateNewSequence()
    {
        correctOrder = new int[9];

        for (int i = 0; i < 9; i++)
        {
            correctOrder[i] = i + 1;
        }

        // Bland rækkefølgen
        for (int i = correctOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = correctOrder[i];
            correctOrder[i] = correctOrder[randomIndex];
            correctOrder[randomIndex] = temp;
        }

        ResetPuzzle();
        UpdatePuzzleText();

        Debug.Log(
            "Ny Puzzle 1 rækkefølge: " +
            string.Join(", ", correctOrder)
        );
    }

    void UpdatePuzzleText()
    {
        if (puzzle1Text == null)
            return;

        puzzle1Text.text =
            "ROOM 1 - PLATFORM SEQUENCE\n\n" +
            correctOrder[0] + " -> " +
            correctOrder[1] + " -> " +
            correctOrder[2] + "\n" +
            correctOrder[3] + " -> " +
            correctOrder[4] + " -> " +
            correctOrder[5] + "\n" +
            correctOrder[6] + " -> " +
            correctOrder[7] + " -> " +
            correctOrder[8];
    }

    public void PressPlatform(int platformNumber)
    {
        if (solved)
            return;

        if (platformNumber == correctOrder[currentStep])
        {
            currentStep++;

            Debug.Log(
                "Korrekt! " +
                currentStep + "/9"
            );

            if (currentStep >= correctOrder.Length)
            {
                solved = true;

                Debug.Log("PUZZLE 1 KLARET!");

                if (puzzleManager != null)
                    puzzleManager.SetPuzzle1(true);
            }
        }
        else
        {
            Debug.Log("Forkert platform! Start forfra.");

            currentStep = 0;
        }
    }

    public void ResetPuzzle()
    {
        currentStep = 0;
        solved = false;

        if (puzzleManager != null)
            puzzleManager.SetPuzzle1(false);
    }
}