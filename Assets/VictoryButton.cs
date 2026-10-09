using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryButton : MonoBehaviour
{
    [Header("Next Level")]
    public string nextSceneName;

    [Header("Settings")]
    public float delayBeforeNextScene = 0.5f;

    private bool levelCompleted = false;

    void OnTriggerEnter(Collider other)
    {
        // Kun den rigtige spiller må gennemføre banen
        if (!other.CompareTag("Player"))
            return;

        if (levelCompleted)
            return;

        levelCompleted = true;

        Debug.Log("LEVEL COMPLETE!");

        Invoke(nameof(LoadNextScene), delayBeforeNextScene);
    }

    void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next Scene Name er ikke sat!");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}