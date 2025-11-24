using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public bool lightTouched = false;
    public int touchPoint;
    public Material originalMat;
    public Material hitMat;
    public Material currentMat;

    public void Start()
    {
        originalMat = GetComponent<Renderer>().material;
        //currentMat = originalMat;

    }
    public void Update()
    {
        if (lightTouched)
        {
            touchPoint = 1;
            currentMat = hitMat;
            GetComponent<Renderer>().material = hitMat;
        }
        else if(!lightTouched)
        {
            touchPoint = 0;
            currentMat = originalMat;
            GetComponent<Renderer>().material = originalMat;
        }
    }

}
