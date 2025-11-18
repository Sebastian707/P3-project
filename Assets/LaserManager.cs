using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserManager : MonoBehaviour
{
    public GameObject[] laserTargets;
    public int totalScore;
    public int targetScore;
    public GameObject blurryObject;
    public Material[] blurryMaterials;
    public GameObject door;
    public float doorLocation;
    public float doorTimer;
    public void Update()
    {
        foreach (GameObject target in laserTargets)
        {
           totalScore = target.GetComponent<LaserTarget>().touchPoint;
            blurryObject.GetComponent<Renderer>().material = blurryMaterials[totalScore];
            
        }
        if (totalScore >= targetScore)
        {
            Debug.Log("All targets hit");
            StartCoroutine(OpenDoorOrSomething());
            
        }
    }

    public IEnumerator OpenDoorOrSomething() 
    {
        float time = doorTimer;
        for(float i = 0; i < time; i += Time.deltaTime)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(door.transform.position.x, doorLocation, door.transform.position.z), i / time);

            yield return null;
        }
    }
}
