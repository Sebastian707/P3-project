using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public GameObject[] laserTargets;
    public int totalScore;
    public int targetScore;
    public GameObject blurryObject;
    public Material[] blurryMaterials;
    public void Update()
    {
        foreach (GameObject target in laserTargets)
        {
           totalScore = target.GetComponent<LaserTarget>().touchPoint;
            blurryObject.GetComponent<Renderer>().material = blurryMaterials[totalScore];
            if (totalScore >= targetScore)
            {
                Debug.Log("All targets hit");
            }
        }
    }
}
