using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CustomPassVolumeManager : MonoBehaviour
{
    public static CustomPassVolumeManager Instance;
    private List<CustomPassVolume> volumes = new List<CustomPassVolume>();

    void Awake()
    {
        Instance = this;
        volumes = FindObjectsOfType<CustomPassVolume>().ToList();
    }

    public CustomPassVolume GetActiveVolume(Vector3 cameraPos)
    {
        CustomPassVolume active = null;

        foreach (var v in volumes)
        {
            if (!v.IsCameraInside(cameraPos))
                continue;

            if (active == null || v.priority > active.priority)
                active = v;
        }

        return active;
    }
}
