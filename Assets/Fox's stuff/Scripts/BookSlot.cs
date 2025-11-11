using UnityEngine;
using System.Collections;

public class BookSlot : MonoBehaviour
{
    [Header("Slot Setup")]
    public string expectedBookID; // the correct book for this slot
    public Transform snapPoint;   // where the book should sit (child transform)
    [HideInInspector] public BookItem currentBook;

    private Renderer rend;
    private Color defaultColor;

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (currentBook != null) return; // already filled

        BookItem book = other.GetComponent<BookItem>();
        if (book != null && !book.IsPlaced)
        {
            // Snap book into place
            other.attachedRigidbody.isKinematic = true;
            other.transform.position = snapPoint.position;
            other.transform.rotation = snapPoint.rotation;

            book.IsPlaced = true;
            currentBook = book;

            Debug.Log($"{book.bookID} placed in {name}");
        }
    }
    */
    private void Start()
    {
        // Optional: visually indicate selection during interaction
        rend = GetComponent<Renderer>();
        if (rend)
            defaultColor = rend.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        BookItem book = other.GetComponent<BookItem>();

        // Only act if a book is being held and this slot is free
        if (currentBook == null && book != null && !book.IsPlaced)
        {
            // Try to get the Interactable_Pickup from the book
            Interactable_Pickup pickup = book.GetComponent<Interactable_Pickup>();
            if (pickup != null)
            {
                // Force-drop it instantly (so it stops following the player)
                pickup.ForceDrop();
            }

            // Snap book into place
            Rigidbody rb = other.attachedRigidbody;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;



            // Snap book into place
            other.transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
            other.transform.SetParent(snapPoint);
            //other.transform.position = snapPoint.position;
            //other.transform.rotation = snapPoint.rotation;

            // Parent it so it moves with the bookshelf door
            other.transform.SetParent(snapPoint);

            book.IsPlaced = true;
            currentBook = book;

            Debug.Log($"{book.bookID} placed in {name}");

            Object.FindFirstObjectByType<BookshelfPuzzle>()?.CheckPuzzle();
            
        }
    }

    // Called by BookshelfPuzzle to manually assign a book (used during swapping)
    public void SetBook(BookItem newBook)
    {
        if (currentBook != null)
        {
            // Unparent and unfreeze previous book
            currentBook.IsPlaced = false;
            currentBook.transform.SetParent(null);
        }

        currentBook = newBook;

        if (newBook != null)
        {
            newBook.IsPlaced = true;

            // Stop physics completely so it doesn’t drop
            newBook.rb.isKinematic = true;
            newBook.rb.useGravity = false;
            newBook.rb.constraints = RigidbodyConstraints.FreezeAll;

            newBook.transform.SetParent(snapPoint);
            newBook.transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);

            
        }
    }

    // Highlight slot (used when selected in interaction mode)
    public void Highlight(bool state)
    {
        var rend = GetComponent<Renderer>();
        if (rend)
            rend.material.color = state ? Color.yellow : defaultColor;
    }
}
