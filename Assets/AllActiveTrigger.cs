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

    private void Update()
    {
        if (objectsToCheck.Count == 0) return;

        // Check if all objects are active
        foreach (var obj in objectsToCheck)
        {
            if (obj == null || !obj.activeSelf)
                return; // Stop here if any object is inactive
        }

        // All objects are active
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

        Debug.Log("Coroutine finished!");
    }
}
