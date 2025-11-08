using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public Transform bookshelf;                  // Assign your Bookshelf model here
    public List<Color> correctColorOrder;         // The correct book colors
    public Vector3 openOffset = new Vector3(2f, 0f, 0f);
    public float slideSpeed = 2f;

    private Transform[] slotPoints;               // Automatically filled from child slots
    private bool puzzleSolved = false;
    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        // Automatically find slot points under the bookshelf model
        slotPoints = bookshelf.GetComponentsInChildren<Transform>()
            .Where(t => t.name.StartsWith("Slot_"))
            .OrderBy(t => t.name)
            .ToArray();

        closedPos = bookshelf.position;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        if (!puzzleSolved && CheckIfSolved())
        {
            puzzleSolved = true;
            StartCoroutine(SlideBookshelf(openPos));
        }
    }

    bool CheckIfSolved()
    {
        // Gather current book colors based on proximity
        List<Color> currentColors = new List<Color>();

        foreach (var slot in slotPoints)
        {
            Collider[] hits = Physics.OverlapSphere(slot.position, 0.1f);
            GameObject book = hits.FirstOrDefault(h => h.CompareTag("Book"))?.gameObject;

            if (book == null)
                return false; // missing book

            Color c = book.GetComponent<Renderer>().material.color;
            currentColors.Add(c);
        }

        // Compare color order
        if (currentColors.Count != correctColorOrder.Count) return false;

        for (int i = 0; i < correctColorOrder.Count; i++)
        {
            if (currentColors[i] != correctColorOrder[i])
                return false;
        }

        return true;
    }

    IEnumerator SlideBookshelf(Vector3 targetPos)
    {
        float t = 0;
        Vector3 start = bookshelf.position;
        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            bookshelf.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }
        bookshelf.position = targetPos;
        Debug.Log(" Puzzle Solved! Bookshelf opened.");
    }
}
