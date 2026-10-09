using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Gun")]
    public float range = 100f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;

    private bool shotThisFixedFrame = false;
    private Vector3 lastShotOrigin;
    private Vector3 lastShotDirection;

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (playerCamera == null)
            return;

        Vector3 origin =
            playerCamera.transform.position;

        Vector3 direction =
            playerCamera.transform.forward;

        lastShotOrigin = origin;
        lastShotDirection = direction;

        shotThisFixedFrame = true;

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        RaycastHit hit;

        if (Physics.Raycast(
            origin,
            direction,
            out hit,
            range))
        {
            GhostReplay ghost =
                hit.collider.GetComponentInParent<GhostReplay>();

            if (ghost != null)
            {
                ghost.KillGhost();
            }
        }
    }

    public bool GetShotThisFrame()
    {
        return shotThisFixedFrame;
    }

    public Vector3 GetShotOrigin()
    {
        return lastShotOrigin;
    }

    public Vector3 GetShotDirection()
    {
        return lastShotDirection;
    }

    public void ClearShot()
    {
        shotThisFixedFrame = false;
    }
}