
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeLoopManager : MonoBehaviour
{
    [Header("Loop Settings")]
    public float loopDuration = 30f;

    [Header("References")]
    public Transform player;
    public GameObject ghostPrefab;

    [Header("Puzzle References")]
    public PuzzleManager puzzleManager;
    public SequencePuzzle sequencePuzzle;

    private float timeRemaining;

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;

    private PlayerRecording currentRecording;

    private readonly List<PlayerRecording> recordings =
        new List<PlayerRecording>();

    private readonly List<GameObject> activeGhosts =
        new List<GameObject>();

    private int currentLoop = 1;

    private string microphoneDevice;
    private AudioClip microphoneRecording;

    private const int sampleRate = 44100;
    private bool microphoneIsRecording = false;
    private bool endingLoop = false;

    void Start()
    {
        if (player == null || ghostPrefab == null)
        {
            Debug.LogError(
                "TimeLoopManager: Player eller Ghost Prefab mangler!"
            );
            enabled = false;
            return;
        }

        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<PuzzleManager>();

        if (sequencePuzzle == null)
            sequencePuzzle = FindFirstObjectByType<SequencePuzzle>();

        playerStartPosition = player.position;
        playerStartRotation = player.rotation;

        SetupMicrophone();
        StartNewLoop();
    }

    void Update()
    {
        if (endingLoop)
            return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetLevel();
                return;
            }

            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                EndLoop();
                return;
            }
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
            EndLoop();
    }

    void FixedUpdate()
    {
        if (endingLoop || currentRecording == null)
            return;

        currentRecording.positions.Add(player.position);
        currentRecording.rotations.Add(player.rotation);

        PlayerGun gun = player.GetComponent<PlayerGun>();

        if (gun != null)
        {
            currentRecording.shots.Add(gun.GetShotThisFrame());
            currentRecording.shotOrigins.Add(gun.GetShotOrigin());
            currentRecording.shotDirections.Add(gun.GetShotDirection());
            gun.ClearShot();
        }
        else
        {
            currentRecording.shots.Add(false);
            currentRecording.shotOrigins.Add(Vector3.zero);
            currentRecording.shotDirections.Add(Vector3.zero);
        }
    }

    void SetupMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Ingen mikrofon fundet.");
            return;
        }

        microphoneDevice = Microphone.devices[0];
    }

    void StartMicrophoneRecording()
    {
        if (microphoneDevice == null)
            return;

        microphoneRecording = Microphone.Start(
            microphoneDevice,
            false,
            Mathf.CeilToInt(loopDuration) + 1,
            sampleRate
        );

        microphoneIsRecording = microphoneRecording != null;
    }

    AudioClip StopMicrophoneRecording()
    {
        if (!microphoneIsRecording ||
            microphoneDevice == null ||
            microphoneRecording == null)
            return null;

        int position = Microphone.GetPosition(microphoneDevice);
        Microphone.End(microphoneDevice);
        microphoneIsRecording = false;

        if (position <= 0)
        {
            microphoneRecording = null;
            return null;
        }

        int channels = microphoneRecording.channels;
        float[] samples = new float[position * channels];
        microphoneRecording.GetData(samples, 0);

        AudioClip clip = AudioClip.Create(
            "Loop_" + currentLoop + "_Audio",
            position,
            channels,
            sampleRate,
            false
        );

        clip.SetData(samples, 0);
        microphoneRecording = null;
        return clip;
    }

    public void KillCurrentPlayer()
    {
        if (!endingLoop)
            EndLoop();
    }

    public void EndLoop()
    {
        if (endingLoop || currentRecording == null)
            return;

        endingLoop = true;

        currentRecording.audioClip = StopMicrophoneRecording();
        recordings.Add(currentRecording);
        currentRecording = null;

        currentLoop++;

        StartNewLoop();

        endingLoop = false;
    }

    void StartNewLoop()
    {
        timeRemaining = loopDuration;

        // RESET ALLE PUZZLES OG WINWALL
        if (puzzleManager != null)
            puzzleManager.ResetPuzzles();

        // Reset PressureButtons
        foreach (PressureButton button in
            FindObjectsByType<PressureButton>(FindObjectsSortMode.None))
        {
            button.ForceReset();
        }

        // Reset LaserSwitches
        foreach (LaserSwitch laserSwitch in
            FindObjectsByType<LaserSwitch>(FindObjectsSortMode.None))
        {
            laserSwitch.ForceReset();
        }

        // Reset Puzzle 1 progress
        if (sequencePuzzle != null)
            sequencePuzzle.ResetPuzzle();

        // Reset alle 9 platforme
        foreach (SequencePlatform platform in
            FindObjectsByType<SequencePlatform>(FindObjectsSortMode.None))
        {
            platform.ForceReset();
        }

        // Reset Puzzle 2
        foreach (Puzzle2Finish finish in
            FindObjectsByType<Puzzle2Finish>(FindObjectsSortMode.None))
        {
            finish.ResetPuzzle();
        }

        // Reset Puzzle 3
        foreach (Puzzle3Finish finish in
            FindObjectsByType<Puzzle3Finish>(FindObjectsSortMode.None))
        {
            finish.ResetPuzzle();
        }

        // Fjern eksisterende Ghosts
        foreach (GameObject ghost in activeGhosts)
        {
            if (ghost != null)
                Destroy(ghost);
        }

        activeGhosts.Clear();

        // Reset Player
        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = playerStartPosition;
        player.rotation = playerStartRotation;

        PlayerMovement movement =
            player.GetComponent<PlayerMovement>();

        if (movement != null)
            movement.ResetMovement();

        if (controller != null)
            controller.enabled = true;

        // Spawn Ghosts fra tidligere loops
        foreach (PlayerRecording recording in recordings)
        {
            GameObject ghost = Instantiate(
                ghostPrefab,
                playerStartPosition,
                playerStartRotation
            );

            GhostReplay replay = ghost.GetComponent<GhostReplay>();

            if (replay != null)
                replay.Setup(recording);

            activeGhosts.Add(ghost);
        }

        currentRecording = new PlayerRecording();
        StartMicrophoneRecording();

        Debug.Log(
            "Loop " + currentLoop +
            " startet | Ghosts: " + recordings.Count
        );
    }

    public void ResetLevel()
    {
        endingLoop = true;

        if (microphoneIsRecording && microphoneDevice != null)
        {
            Microphone.End(microphoneDevice);
            microphoneIsRecording = false;
        }

        microphoneRecording = null;

        foreach (GameObject ghost in activeGhosts)
        {
            if (ghost != null)
                Destroy(ghost);
        }

        activeGhosts.Clear();
        recordings.Clear();

        currentRecording = null;
        currentLoop = 1;

        if (puzzleManager != null)
            puzzleManager.ResetPuzzles();

        // Ny tilfældig rækkefølge kun ved R
        if (sequencePuzzle != null)
            sequencePuzzle.GenerateNewSequence();

        StartNewLoop();

        endingLoop = false;

        Debug.Log("LEVEL RESET!");
    }

    public float GetTimeRemaining()
    {
        return Mathf.Max(0f, timeRemaining);
    }

    public int GetCurrentLoop()
    {
        return currentLoop;
    }

    public int GetCloneCount()
    {
        return recordings.Count;
    }
}
