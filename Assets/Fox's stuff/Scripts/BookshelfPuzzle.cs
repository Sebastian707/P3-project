using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public KeyCode devOpenKey = KeyCode.F1;
    public List<BookSlot> slots;               // assign in Inspector
    public List<string> correctOrder;          // e.g., ["Red", "Green", "Blue"]
    public Transform door;                     // bookshelf door to animate
    //public Vector3 openOffset = new Vector3(2f, 0f, 0f); // how far to move open
    public float openSpeed = 2f;

    public float forwardDistance = 0.5f;   // how far forward/back before sliding
    public float sideDistance = 2f;        // how far it slides open
    public float moveDuration = 0.7f;

    [Header("Interaction")]
    public Transform bookshelfCameraPoint; // where the camera moves when interacting
    public Camera mainCamera;
    public float cameraMoveSpeed = 3f;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode exitKey = KeyCode.Escape;
    public KeyCode checkKey = KeyCode.Space;

    private bool isOpen = false;
    private bool isInteracting = false;
    //private Vector3 closedPos;
    //private Vector3 targetPos;
    private Vector3 cameraReturnPos;
    private Quaternion cameraReturnRot;
    private BookSlot selectedSlot;

    void Start()
    {
        //closedPos = door.position;
        //targetPos = closedPos;
    }

    void Update()
    {
        // --------------------
        // DEV FORCE OPEN SHORTCUT
        // --------------------
        if (Input.GetKeyDown(devOpenKey))
        {
            Debug.Log("DEV: Force-opening bookshelf.");
            isOpen = true;
            StartCoroutine(OpenDoorSequence());
        }

        //door.position = Vector3.Lerp(door.position, targetPos, Time.deltaTime * openSpeed);

        if (isInteracting)
        {
            // Smoothly move camera toward focus point
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                bookshelfCameraPoint.position,
                Time.deltaTime * cameraMoveSpeed
            );

            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                bookshelfCameraPoint.rotation,
                Time.deltaTime * cameraMoveSpeed
            );

            HandleSlotSelection();

            if (Input.GetKeyDown(exitKey))
                ExitInteraction();

            if (Input.GetKeyDown(checkKey))
                CheckPuzzle();
        }

        else
        {
            // Player can initiate bookshelf interaction
            if (Input.GetKeyDown(interactKey))
            {
                float dist = Vector3.Distance(mainCamera.transform.position, transform.position);
                if (dist < 3f) // only if close enough
                    EnterInteraction();
            }
        }
    }

    void EnterInteraction()
    {
        isInteracting = true;
        cameraReturnPos = mainCamera.transform.position;
        cameraReturnRot = mainCamera.transform.rotation;
        Debug.Log("Entered bookshelf interaction mode.");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ExitInteraction()
    {
        isInteracting = false;
        Debug.Log("Exited bookshelf interaction mode.");
        mainCamera.transform.position = cameraReturnPos;
        mainCamera.transform.rotation = cameraReturnRot;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reset any highlight
        if (selectedSlot)
        {
            selectedSlot.Highlight(false);
            selectedSlot = null;
        }
    }

    void HandleSlotSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                BookSlot clickedSlot = hit.collider.GetComponent<BookSlot>();
                if (clickedSlot != null)
                {
                    if (selectedSlot == null)
                    {
                        // Select this slot if it has a book
                        if (clickedSlot.currentBook != null)
                        {
                            selectedSlot = clickedSlot;
                            clickedSlot.Highlight(true);
                            Debug.Log($"Selected book: {clickedSlot.currentBook.bookID}");
                        }
                    }
                    
                    else
                    {
                        // Clicked same slot again deselect
                        if (clickedSlot == selectedSlot)
                        {
                            selectedSlot.Highlight(false);
                            selectedSlot = null;
                            return;
                        }

                        // case 1: Move selected book into the new slot if empty
                        if (clickedSlot.currentBook == null)
                        {
                            BookItem movingBook = selectedSlot.currentBook;
                            selectedSlot.SetBook(null);
                            clickedSlot.SetBook(movingBook);

                            Debug.Log($"Moved book {movingBook.bookID} from {selectedSlot.name} to {clickedSlot.name}");
                        }
                        // Case 2: clicked another occupied slot swap books
                        else if (clickedSlot.currentBook != null)
                        {
                            SwapBooks(selectedSlot, clickedSlot);
                        }

                        // Deselect in all cases
                        selectedSlot.Highlight(false);
                        selectedSlot = null;
                    }
                }
            }
        }
    }
    void SwapBooks(BookSlot slotA, BookSlot slotB)
    {
        BookItem bookA = slotA.currentBook;
        BookItem bookB = slotB.currentBook;

        if (bookA == null || bookB == null)
            return;

        // Temporarily detach both
        bookA.transform.SetParent(null);
        bookB.transform.SetParent(null);

        // Reassign them
        slotA.SetBook(bookB);
        slotB.SetBook(bookA);

        Debug.Log($"Swapped {bookA.bookID} with {bookB.bookID}");
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

            //targetPos = closedPos + openOffset;
            StartCoroutine(OpenDoorSequence());

            foreach (var slot in slots)
            {
                if (slot.currentBook != null)
                {
                    slot.currentBook.rb.isKinematic = true;
                    slot.currentBook.rb.useGravity = false;
                    slot.currentBook.rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }

            // Automatically exit interaction when solved
            if (isInteracting)
                ExitInteraction();
        }
        else
        {
            Debug.Log("Incorrect order. Try again.");
        }
    }

    private IEnumerator OpenDoorSequence()
    {
        Transform d = door;

        // Step 1: Move along local X (slide to the side)
        Vector3 start = d.localPosition;
        Vector3 sideTarget = start + Vector3.right * sideDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            d.localPosition = Vector3.Lerp(start, sideTarget, t);
            yield return null;
        }

        // Step 2: Move along local Z (move backwards)
        Vector3 slideStart = d.localPosition;
        Vector3 backwardTarget = slideStart + Vector3.back * forwardDistance;
        // if you want it to move *forward*, switch Vector3.back to Vector3.forward

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            d.localPosition = Vector3.Lerp(slideStart, backwardTarget, t);
            yield return null;
        }
    }


}
