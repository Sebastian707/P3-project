using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public List<BookSlot> slots;               // assign in Inspector
    public List<string> correctOrder;          // e.g., ["Red", "Green", "Blue"]
    public Transform door;                     // bookshelf door to animate
    public Vector3 openOffset = new Vector3(2f, 0f, 0f); // how far to move open
    public float openSpeed = 2f;

    private bool isOpen = false;
    private Vector3 closedPos;
    private Vector3 targetPos;

    void Start()
    {
        closedPos = door.position;
        targetPos = closedPos;
    }

    void Update()
    {
        door.position = Vector3.Lerp(door.position, targetPos, Time.deltaTime * openSpeed);
    }

    public void CheckPuzzle()
    {
        if (isOpen) return;

        // Get the placed book IDs in slot order
        var placedBooks = slots.Select(s => s.currentBook ? s.currentBook.bookID : "").ToList();

        if (placedBooks.SequenceEqual(correctOrder))
        {
            Debug.Log("Puzzle solved! Bookshelf opening...");
            isOpen = true;
            targetPos = closedPos + openOffset;
        }
    }
}
