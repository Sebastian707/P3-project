using UnityEngine;

public class BookItem : MonoBehaviour
{
    [Header("Book Data")]
    public string bookID; // e.g., "Red", "Blue", "Green"

    public bool IsPlaced { get; set; } = false; // runtime state
}
