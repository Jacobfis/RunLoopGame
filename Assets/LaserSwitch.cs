
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [Header("Lasers")]
    public GameObject[] lasersToDisable;
    public GameObject[] lasersToEnable;

    [Header("Visual")]
    public Renderer buttonRenderer;
    public Color inactiveColor = Color.red;
    public Color activeColor = Color.green;

    private HashSet<GameObject> objectsOnButton =
        new HashSet<GameObject>();

    private bool isPressed = false;

    void Start()
    {
        ForceReset();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return;

        GameObject activator = GetPlayer(other);

        if (activator != null)
        {
            objectsOnButton.Add(activator);
            UpdateButton();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return;

        GameObject activator = GetPlayer(other);

        if (activator != null)
        {
            objectsOnButton.Add(activator);
            UpdateButton();
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject activator = GetPlayer(other);

        if (activator != null)
        {
            objectsOnButton.Remove(activator);
            UpdateButton();
        }
    }

    GameObject GetPlayer(Collider other)
    {
        if (other.GetComponentInParent<GhostReplay>() != null)
            return null;

        if (other.CompareTag("Player"))
            return other.transform.root.gameObject;

        if (other.transform.root.CompareTag("Player"))
            return other.transform.root.gameObject;

        return null;
    }

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

    void UpdateButton()
    {
        objectsOnButton.RemoveWhere(obj => obj == null);

        bool pressed = objectsOnButton.Count > 0;

        if (pressed == isPressed)
            return;

        isPressed = pressed;

        foreach (GameObject laser in lasersToDisable)
        {
            if (laser != null)
                laser.SetActive(!isPressed);
        }

        foreach (GameObject laser in lasersToEnable)
        {
            if (laser != null)
                laser.SetActive(isPressed);
        }

        if (buttonRenderer != null)
            buttonRenderer.material.color =
                isPressed ? activeColor : inactiveColor;
    }

    public void ForceReset()
    {
        objectsOnButton.Clear();
        isPressed = false;

        foreach (GameObject laser in lasersToDisable)
        {
            if (laser != null)
                laser.SetActive(true);
        }

        foreach (GameObject laser in lasersToEnable)
        {
            if (laser != null)
                laser.SetActive(false);
        }

        if (buttonRenderer != null)
            buttonRenderer.material.color = inactiveColor;
    }
}
