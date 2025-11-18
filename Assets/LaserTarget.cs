using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public bool lightTouched = false;
    public int touchPoint = 0;
    public void Update()
    {
        if (lightTouched)
        {
            touchPoint = 1;
        }
        else
        {
            touchPoint = 0;
        }
    }

}
