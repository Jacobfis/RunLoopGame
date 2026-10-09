using System.Collections.Generic;
using UnityEngine;

public class PlayerRecording
{
    public List<Vector3> positions =
        new List<Vector3>();

    public List<Quaternion> rotations =
        new List<Quaternion>();

    public List<bool> shots =
        new List<bool>();

    public List<Vector3> shotOrigins =
        new List<Vector3>();

    public List<Vector3> shotDirections =
        new List<Vector3>();

    public AudioClip audioClip;
}