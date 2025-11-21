using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllActiveTrigger : MonoBehaviour
{
    [Header("Objects to Monitor")]
    public List<GameObject> objectsToCheck = new List<GameObject>();

    [Header("Trigger Once?")]
    public bool triggerOnce = true;
    private bool hasTriggered = false;
    private bool doorOpen;
    public GameObject doorToUnlock;

    private void Update()
    {
        if (objectsToCheck.Count == 0) return;

        foreach (var obj in objectsToCheck)
        {
            if (obj == null || !obj.activeSelf)
                return; 
        }

        if (!triggerOnce || !hasTriggered)
        {
            StartCoroutine(CustomAction());
            hasTriggered = true;
        }
    }

    private IEnumerator CustomAction()
    {
        Debug.Log("All objects are active! Coroutine started.");

        yield return new WaitForSeconds(2f);
        doorToUnlock.GetComponent<Animator>().SetBool("IsOpen", true);

        Debug.Log("Coroutine finished!");
    }
}
