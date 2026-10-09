
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GhostReplay : MonoBehaviour
{
    [Header("Gun")]
    public float gunRange = 100f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;

    [Header("Button Detection")]
    public float detectionRadius = 0.6f;
    public float detectionHeightOffset = -0.8f;

    private PlayerRecording recording;
    private int currentFrame = 0;

    private AudioSource audioSource;

    private PressureButton currentButton;
    private LaserSwitch currentLaserSwitch;
    private SequencePlatform currentSequencePlatform;
    private Puzzle2Finish currentPuzzle2Finish;
    private Puzzle3Finish currentPuzzle3Finish;

    private bool dead = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Setup(PlayerRecording newRecording)
    {
        recording = newRecording;
        currentFrame = 0;
        dead = false;

        currentButton = null;
        currentLaserSwitch = null;
        currentSequencePlatform = null;
        currentPuzzle2Finish = null;
        currentPuzzle3Finish = null;

        if (recording != null &&
            recording.positions.Count > 0)
        {
            transform.position = recording.positions[0];
            transform.rotation = recording.rotations[0];
        }

        if (recording != null &&
            recording.audioClip != null)
        {
            audioSource.clip = recording.audioClip;
            audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        if (dead || recording == null)
            return;

        if (currentFrame >= recording.positions.Count)
        {
            ReleaseButtons();
            return;
        }

        transform.position =
            recording.positions[currentFrame];

        transform.rotation =
            recording.rotations[currentFrame];

        Physics.SyncTransforms();

        CheckButtons();

        if (currentFrame < recording.shots.Count &&
            recording.shots[currentFrame])
        {
            ReplayShot(currentFrame);
        }

        currentFrame++;
    }

    void CheckButtons()
    {
        Vector3 checkPosition =
            transform.position +
            Vector3.up * detectionHeightOffset;

        Collider[] hits = Physics.OverlapSphere(
            checkPosition,
            detectionRadius,
            ~0,
            QueryTriggerInteraction.Collide
        );

        PressureButton foundButton = null;
        LaserSwitch foundLaserSwitch = null;
        SequencePlatform foundSequence = null;
        Puzzle2Finish foundPuzzle2 = null;
        Puzzle3Finish foundPuzzle3 = null;

        foreach (Collider hit in hits)
        {
            if (foundButton == null)
                foundButton =
                    hit.GetComponentInParent<PressureButton>();

            if (foundLaserSwitch == null)
                foundLaserSwitch =
                    hit.GetComponentInParent<LaserSwitch>();

            if (foundSequence == null)
                foundSequence =
                    hit.GetComponentInParent<SequencePlatform>();

            if (foundPuzzle2 == null)
                foundPuzzle2 =
                    hit.GetComponentInParent<Puzzle2Finish>();

            if (foundPuzzle3 == null)
                foundPuzzle3 =
                    hit.GetComponentInParent<Puzzle3Finish>();
        }

        // Almindelig PressureButton
        if (currentButton != foundButton)
        {
            if (currentButton != null)
                currentButton.GhostExit(gameObject);

            if (foundButton != null)
                foundButton.GhostEnter(gameObject);

            currentButton = foundButton;
        }

        // LaserSwitch
        if (currentLaserSwitch != foundLaserSwitch)
        {
            if (currentLaserSwitch != null)
                currentLaserSwitch.GhostExit(gameObject);

            if (foundLaserSwitch != null)
                foundLaserSwitch.GhostEnter(gameObject);

            currentLaserSwitch = foundLaserSwitch;
        }

        // Puzzle 1 - platforme
        if (currentSequencePlatform != foundSequence)
        {
            if (currentSequencePlatform != null)
                currentSequencePlatform.GhostExit(gameObject);

            if (foundSequence != null)
                foundSequence.GhostEnter(gameObject);

            currentSequencePlatform = foundSequence;
        }

        // Puzzle 2 - slutknap
        if (currentPuzzle2Finish != foundPuzzle2)
        {
            if (foundPuzzle2 != null)
                foundPuzzle2.GhostEnter(gameObject);

            currentPuzzle2Finish = foundPuzzle2;
        }

        // Puzzle 3 - slutknap
        if (currentPuzzle3Finish != foundPuzzle3)
        {
            if (foundPuzzle3 != null)
                foundPuzzle3.GhostEnter(gameObject);

            currentPuzzle3Finish = foundPuzzle3;
        }
    }

    // =========================
    // REPLAY SHOTS
    // =========================

    void ReplayShot(int frame)
    {
        if (frame >= recording.shotOrigins.Count ||
            frame >= recording.shotDirections.Count)
        {
            return;
        }

        Vector3 origin =
            recording.shotOrigins[frame];

        Vector3 direction =
            recording.shotDirections[frame];

        if (muzzleFlash != null)
            muzzleFlash.Play();

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            direction,
            gunRange
        );

        System.Array.Sort(
            hits,
            (a, b) => a.distance.CompareTo(b.distance
        ));

        foreach (RaycastHit hit in hits)
        {
            GhostReplay hitGhost =
                hit.collider.GetComponentInParent<GhostReplay>();

            if (hitGhost == this)
                continue;

            if (hitGhost != null)
            {
                hitGhost.KillGhost();
                return;
            }

            bool hitPlayer =
                hit.collider.CompareTag("Player") ||
                hit.collider.transform.root.CompareTag("Player");

            if (hitPlayer)
            {
                TimeLoopManager manager =
                    FindFirstObjectByType<TimeLoopManager>();

                if (manager != null)
                    manager.KillCurrentPlayer();

                return;
            }

            return;
        }
    }

    // =========================
    // KILL GHOST
    // =========================

    public void KillGhost()
    {
        if (dead)
            return;

        dead = true;

        Debug.Log(gameObject.name + " blev skudt!");

        ReleaseButtons();

        if (audioSource != null)
            audioSource.Stop();

        Renderer[] renderers =
            GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
            rend.enabled = false;

        Collider[] colliders =
            GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
            col.enabled = false;
    }

    void ReleaseButtons()
    {
        if (currentButton != null)
        {
            currentButton.GhostExit(gameObject);
            currentButton = null;
        }

        if (currentLaserSwitch != null)
        {
            currentLaserSwitch.GhostExit(gameObject);
            currentLaserSwitch = null;
        }

        if (currentSequencePlatform != null)
        {
            currentSequencePlatform.GhostExit(gameObject);
            currentSequencePlatform = null;
        }

        currentPuzzle2Finish = null;
        currentPuzzle3Finish = null;
    }

    void OnDestroy()
    {
        ReleaseButtons();
    }
}
