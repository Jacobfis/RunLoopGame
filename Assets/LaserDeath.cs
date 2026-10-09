
using UnityEngine;

public class LaserDeath : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Room 2 Respawn Position")]
    public Vector3 respawnPosition =
        new Vector3(0.01229f, 6.53f, 36.4f);

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip deathSound;

    private bool resetting = false;

    void OnTriggerEnter(Collider other)
    {
        // Ghosts følger deres optagede teleportering
        // og skal ikke påvirke den nuværende spiller
        GhostReplay ghost =
            other.GetComponentInParent<GhostReplay>();

        if (ghost != null)
        {
            PlayDeathSound();
            return;
        }

        // Kun den rigtige Player
        if (player == null)
            return;

        bool isPlayer =
            other.transform == player ||
            other.transform.IsChildOf(player);

        if (!isPlayer || resetting)
            return;

        ResetPlayer();
    }

    void ResetPlayer()
    {
        resetting = true;

        PlayDeathSound();

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = respawnPosition;

        PlayerMovement movement =
            player.GetComponent<PlayerMovement>();

        if (movement != null)
            movement.ResetMovement();

        if (controller != null)
            controller.enabled = true;

        Debug.Log("Player ramt af laser! Tilbage til rum 2.");

        resetting = false;
    }

    void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);
    }
}
