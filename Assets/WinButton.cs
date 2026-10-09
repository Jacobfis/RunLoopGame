
using UnityEngine;
using TMPro;

public class WinButton : MonoBehaviour
{
    [Header("Win UI")]
    public GameObject winPanel;
    public TMP_Text winText;

    [Header("References")]
    public PuzzleManager puzzleManager;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip winSound;

    private bool hasWon = false;

    void Awake()
    {
        Time.timeScale = 1f;

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Start()
    {
        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<PuzzleManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasWon)
            return;

        // Ghosts må ikke aktivere Win-knappen
        if (other.GetComponentInParent<GhostReplay>() != null)
            return;

        bool isPlayer =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (!isPlayer)
            return;

        if (puzzleManager == null)
            return;

        // Alle 3 puzzles skal være klaret
        if (!puzzleManager.puzzle1Solved ||
            !puzzleManager.puzzle2Solved ||
            !puzzleManager.puzzle3Solved)
        {
            Debug.Log("Alle puzzles er ikke klaret endnu!");
            return;
        }

        WinGame();
    }

    void WinGame()
    {
        hasWon = true;

        if (winText != null)
            winText.text = "YOU WIN!";

        if (winPanel != null)
            winPanel.SetActive(true);

        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("YOU WIN!");
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
