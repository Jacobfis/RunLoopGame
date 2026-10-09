using UnityEngine;

public class DeathTile : MonoBehaviour
{
    [HideInInspector]
    public Transform player;

    [HideInInspector]
    public Vector3 respawnPosition;

    [HideInInspector]
    public Quaternion respawnRotation;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip deathSound;

    private bool resetting = false;

    void OnTriggerEnter(Collider other)
    {
        // =========================
        // GHOST
        // =========================

        GhostReplay ghost =
            other.GetComponentInParent<GhostReplay>();

        if (ghost != null)
        {
            // Ghostens teleport er allerede gemt
            // i dens optagede bevægelse.
            // Vi skal derfor KUN afspille lyden.
            PlayDeathSound();

            Debug.Log(
                ghost.gameObject.name +
                " ramte en DeathTile"
            );

            return;
        }

        // =========================
        // PLAYER
        // =========================

        bool isPlayer =
            other.CompareTag("Player");

        if (!isPlayer)
        {
            Transform root =
                other.transform.root;

            if (root.CompareTag("Player"))
            {
                isPlayer = true;
            }
        }

        if (!isPlayer)
            return;

        if (resetting)
            return;

        ResetPlayer();
    }

    void ResetPlayer()
    {
        if (player == null)
        {
            Debug.LogError(
                "DeathTile har ingen Player reference!"
            );

            return;
        }

        resetting = true;

        // =========================
        // SOUND
        // =========================

        PlayDeathSound();

        // =========================
        // RESET PLAYER
        // =========================

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        player.position =
            respawnPosition;

        player.rotation =
            respawnRotation;

        PlayerMovement movement =
            player.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.ResetMovement();
        }

        if (controller != null)
        {
            controller.enabled = true;
        }

        resetting = false;
    }

    void PlayDeathSound()
    {
        if (audioSource == null)
            return;

        if (deathSound == null)
            return;

        audioSource.PlayOneShot(
            deathSound
        );
    }
}