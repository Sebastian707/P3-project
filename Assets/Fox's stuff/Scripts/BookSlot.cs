using UnityEngine;

public class BookSlot : MonoBehaviour
{
    public string expectedBookID; // the correct book for this slot
    public Transform snapPoint;   // where the book should sit (child transform)
    [HideInInspector] public BookItem currentBook;

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
            other.transform.position = snapPoint.position;
            other.transform.rotation = snapPoint.rotation;

            // Parent it so it moves with the bookshelf door
            other.transform.SetParent(snapPoint);

            book.IsPlaced = true;
            currentBook = book;

            Debug.Log($"{book.bookID} placed in {name}");

            Object.FindFirstObjectByType<BookshelfPuzzle>()?.CheckPuzzle();

        }
    }

}
