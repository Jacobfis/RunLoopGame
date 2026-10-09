using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Platforms")]
    public GameObject[] platforms;

    [Header("Button Visual")]
    public Renderer buttonRenderer;
    public Color inactiveColor = Color.red;
    public Color activeColor = Color.green;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip pressSound;
    public AudioClip releaseSound;

    private HashSet<GameObject> objectsOnButton =
        new HashSet<GameObject>();

    private bool wasPressed = false;

    void Start()
    {
        ForceReset();
    }

    void OnTriggerEnter(Collider other)
    {
        TryAddObject(other);
    }

    void OnTriggerStay(Collider other)
    {
        TryAddObject(other);
    }

    void OnTriggerExit(Collider other)
    {
        GameObject activator = GetActivator(other);

        if (activator != null)
        {
            objectsOnButton.Remove(activator);
            UpdateButton();
        }
    }

    void TryAddObject(Collider other)
    {
        GameObject activator = GetActivator(other);

        if (activator != null)
        {
            objectsOnButton.Add(activator);
            UpdateButton();
        }
    }

    GameObject GetActivator(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.CompareTag("Ghost"))
        {
            return other.gameObject;
        }

        Transform root = other.transform.root;

        if (root.CompareTag("Player") ||
            root.CompareTag("Ghost"))
        {
            return root.gameObject;
        }

        return null;
    }

    void UpdateButton()
    {
        objectsOnButton.RemoveWhere(
            obj => obj == null
        );

        bool isPressed =
            objectsOnButton.Count > 0;

        // =========================
        // SOUND
        // =========================

        // Knappen er lige blevet trykket
        if (isPressed && !wasPressed)
        {
            PlaySound(pressSound);
        }

        // Knappen er lige blevet sluppet
        if (!isPressed && wasPressed)
        {
            PlaySound(releaseSound);
        }

        wasPressed = isPressed;

        // =========================
        // PLATFORM
        // =========================

        SetPlatforms(isPressed);

        // =========================
        // COLOR
        // =========================

        if (buttonRenderer != null)
        {
            buttonRenderer.material.color =
                isPressed
                ? activeColor
                : inactiveColor;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
            return;

        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    void SetPlatforms(bool active)
    {
        foreach (GameObject platform in platforms)
        {
            if (platform != null)
            {
                platform.SetActive(active);
            }
        }
    }

    public void ForceReset()
    {
        objectsOnButton.Clear();

        wasPressed = false;

        SetPlatforms(false);

        if (buttonRenderer != null)
        {
            buttonRenderer.material.color =
                inactiveColor;
        }
    }

    // =========================
    // GHOST
    // =========================

    public void GhostEnter(GameObject ghost)
    {
        objectsOnButton.Add(ghost);

        UpdateButton();
    }

    public void GhostExit(GameObject ghost)
    {
        objectsOnButton.Remove(ghost);

        UpdateButton();
    }
}