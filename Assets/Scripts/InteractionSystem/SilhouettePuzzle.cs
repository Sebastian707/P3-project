using UnityEngine;
using TMPro;
using System.Collections;

public class SilhouettePuzzle : Interactable
{
    [Header("Puzzle Settings")]
    [SerializeField] private Texture2D solutionImage;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float cubeSize = 0.1f;
    [SerializeField, Range(1, 8)] private int pixelSkip = 1;
    [SerializeField] private float alignmentThreshold = 0.05f;
    [SerializeField] private float initialRotationMin = 45f;
    [SerializeField] private float initialRotationMax = 60f;
    [SerializeField, Range(0, 100)] private float completionAlignmentPercentage = 98f; // % needed for win

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Camera & UI")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private TextMeshProUGUI alignmentText;

    [Header("Z-axis Scatter")]
    [SerializeField] private float zScatterMultiplier = 0.05f;

    private GameObject cubeContainer;
    private Vector3[] solutionPositions;
    private Vector3[] solutionViewportPositions;
    private GameObject[] cubes;
    private bool puzzleActive = false;
    private bool puzzleCompleted = false;
    private bool completionCoroutineRunning = false;
    private Vector3 currentRotationEuler;
    private MeshRenderer meshRenderer;

    // Interaction
    protected override void Interact()
    {
        if (puzzleActive)
        {
            StopInteract();
            return;
        }

        puzzleActive = true;
        puzzleCompleted = false;

        if (playerCamera != null) playerCamera.enabled = false;
        if (puzzleCamera != null) puzzleCamera.enabled = true;

        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = false;

        if (alignmentText != null)
        {
            alignmentText.gameObject.SetActive(true);
            alignmentText.text = "Alignment: 0%";
        }
    }

    public void StopInteract()
    {
        puzzleActive = false;

        if (playerCamera != null) playerCamera.enabled = true;
        if (puzzleCamera != null) puzzleCamera.enabled = false;

        if (meshRenderer != null) meshRenderer.enabled = true;

        if (alignmentText != null)
            alignmentText.gameObject.SetActive(false);

        ResetPuzzle();
    }

    // Initialization
    private void Start()
    {
        if (playerCamera == null) Debug.LogError("Player camera not assigned!");
        if (puzzleCamera == null) Debug.LogError("Puzzle camera not assigned!");
        if (solutionImage == null) Debug.LogError("Solution image not assigned!");
        if (cubePrefab == null) Debug.LogError("Cube prefab not assigned!");

        meshRenderer = GetComponent<MeshRenderer>();
        if (alignmentText != null) alignmentText.gameObject.SetActive(false);

        GeneratePuzzle();
        GenerateSolutionViewportPositions();
        DynamicScatterPuzzle();
    }

    private void GeneratePuzzle()
    {
        if (solutionImage == null || cubePrefab == null) return;

        if (cubeContainer != null) Destroy(cubeContainer);
        cubeContainer = new GameObject("CubeContainer");
        cubeContainer.transform.SetParent(transform);
        cubeContainer.transform.localPosition = Vector3.zero;

        int width = solutionImage.width;
        int height = solutionImage.height;

        int blackPixelCount = 0;
        for (int y = 0; y < height; y += pixelSkip)
            for (int x = 0; x < width; x += pixelSkip)
                if (solutionImage.GetPixel(x, y).r < 0.5f) blackPixelCount++;

        solutionPositions = new Vector3[blackPixelCount];
        cubes = new GameObject[blackPixelCount];

        float imageWidth = width * cubeSize;
        float imageHeight = height * cubeSize;
        Vector3 origin = new Vector3(-imageWidth / 2f, -imageHeight / 2f, 0f);

        int index = 0;
        for (int y = 0; y < height; y += pixelSkip)
        {
            for (int x = 0; x < width; x += pixelSkip)
            {
                if (solutionImage.GetPixel(x, y).r < 0.5f)
                {
                    Vector3 localPos = origin + new Vector3((x + 0.5f) * cubeSize, (y + 0.5f) * cubeSize, 0f);
                    solutionPositions[index] = localPos;

                    GameObject cube = Instantiate(cubePrefab, cubeContainer.transform);
                    cube.transform.localPosition = localPos;
                    cube.transform.localScale = Vector3.one * cubeSize;
                    cubes[index] = cube;
                    index++;
                }
            }
        }

        currentRotationEuler = cubeContainer.transform.eulerAngles;
    }

    private void GenerateSolutionViewportPositions()
    {
        if (cubes == null || puzzleCamera == null) return;

        solutionViewportPositions = new Vector3[cubes.Length];
        Vector3 prevRotation = cubeContainer.transform.eulerAngles;

        cubeContainer.transform.eulerAngles = Vector3.zero;

        for (int i = 0; i < cubes.Length; i++)
            solutionViewportPositions[i] = puzzleCamera.WorldToViewportPoint(cubeContainer.transform.TransformPoint(solutionPositions[i]));

        cubeContainer.transform.eulerAngles = prevRotation;
        currentRotationEuler = cubeContainer.transform.eulerAngles;
    }

    private void DynamicScatterPuzzle()
    {
        if (cubes == null || cubeContainer == null || solutionViewportPositions == null || puzzleCamera == null) return;

        float cameraDistance = Mathf.Abs(puzzleCamera.transform.position.z - cubeContainer.transform.position.z);
        float scatterDepthMax = cameraDistance * zScatterMultiplier;

        for (int i = 0; i < cubes.Length; i++)
        {
            Vector3 solutionPos = solutionPositions[i];
            float randomZ = Random.Range(-scatterDepthMax, scatterDepthMax);
            cubes[i].transform.localPosition = new Vector3(solutionPos.x, solutionPos.y, randomZ);
            cubes[i].transform.localScale = Vector3.one * cubeSize;
        }

        float rotX = Random.Range(initialRotationMin, initialRotationMax);
        float rotY = Random.Range(initialRotationMin, initialRotationMax);
        cubeContainer.transform.eulerAngles = new Vector3(rotX, rotY, 0f);
        currentRotationEuler = cubeContainer.transform.eulerAngles;
    }

    private void Update()
    {
        if (!puzzleActive && !puzzleCompleted) return;

        HandleRotation();
        UpdateAlignment();
    }

    private void HandleRotation()
    {
        if (!puzzleActive) return;

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentRotationEuler.y += mouseX * rotationSpeed * Time.deltaTime;
            currentRotationEuler.x -= mouseY * rotationSpeed * Time.deltaTime;
            cubeContainer.transform.eulerAngles = currentRotationEuler;
        }
    }

    private void UpdateAlignment()
    {
        if (cubes == null || solutionViewportPositions == null || puzzleCamera == null) return;
        if (puzzleCompleted) return;

        int aligned = 0;
        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null) continue;

            Vector3 screenPos = puzzleCamera.WorldToViewportPoint(cubes[i].transform.position);
            float dist = Vector2.Distance(new Vector2(screenPos.x, screenPos.y),
                                          new Vector2(solutionViewportPositions[i].x, solutionViewportPositions[i].y));
            if (dist < alignmentThreshold) aligned++;
        }

        float percent = (float)aligned / cubes.Length * 100f;
        if (alignmentText != null && alignmentText.gameObject.activeSelf)
            alignmentText.text = $"Alignment: {percent:F0}%";

        if (percent >= completionAlignmentPercentage && !completionCoroutineRunning)
            StartCoroutine(PuzzleCompleteRoutine());
    }

    private IEnumerator PuzzleCompleteRoutine()
    {
        completionCoroutineRunning = true;
        puzzleCompleted = true;

        for (int i = 0; i < cubes.Length; i++)
            if (cubes[i] != null)
                cubes[i].transform.localPosition = solutionPositions[i];

        puzzleActive = false;

        if (alignmentText != null)
        {
            alignmentText.gameObject.SetActive(true);
            alignmentText.text = "Puzzle Complete!";
        }

        Debug.Log("Puzzle Completed!");

        yield return new WaitForSeconds(3f);

        StopInteract();
        completionCoroutineRunning = false;
    }

    private void ResetPuzzle()
    {
        puzzleCompleted = false;
        DynamicScatterPuzzle();
        if (alignmentText != null)
            alignmentText.text = "Alignment: 0%";
    }
}
