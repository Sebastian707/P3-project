using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Serialization;

public class SilhouettePuzzle : Interactable
{
    [Header("Puzzle Settings")]
    [SerializeField] private Texture2D[] solutionImages = new Texture2D[8];
    private Texture2D currentSolutionImage;
    private System.Collections.Generic.List<Texture2D> remainingImages;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float alignmentThreshold = 0.05f;
    [SerializeField] private float initialRotationMin = 45f;
    [SerializeField] private float initialRotationMax = 60f;
    [SerializeField, Range(0, 100)] private float completionAlignmentPercentage = 98f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Camera & UI")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private TextMeshProUGUI alignmentText;
    [SerializeField] private float cameraTransitionDuration = 1f;
    [SerializeField] private float fadeTransitionDuration = 1f;

    [FormerlySerializedAs("promptMessage")]
    [Header("Prompt Message")]
    [SerializeField] private GameObject puzzlePromptMessage;

    [Header("Z-axis Scatter")]
    [SerializeField] private float zScatterMultiplier = 0.05f;

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    [Header("Puzzle Scaling")]
    [SerializeField] private float maxPuzzleSize = 2f;

    private GameObject cubeContainer;
    private Vector3[] solutionPositions;
    private Vector3[] solutionViewportPositions;
    private GameObject[] cubes;
    private float[] initialZOffsets;
    private float cubeSize;
    private bool puzzleActive = false;
    private bool puzzleCompleted = false;
    private bool completionCoroutineRunning = false;
    private Vector3 currentRotationEuler;
    private MeshRenderer meshRenderer;
    private bool interactionLocked = false;
    private Vector3 puzzleCameraTargetPosition;
    private Quaternion puzzleCameraTargetRotation;
    private bool isCameraTransitioning = false;
    private Material[] cubeMaterials;
    private Material objectMaterial;
    private Color[] cubeOriginalColors;
    private Color objectOriginalColor;

    // -----------------------------
    // Interact System
    // -----------------------------
    protected override void Interact()
    {
        if (interactionLocked || isCameraTransitioning) return;

        if (puzzleActive)
        {
            StopInteract();
            return;
        }

        puzzleActive = true;
        interactionLocked = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Hide prompt while interacting
        if (puzzlePromptMessage != null)
            puzzlePromptMessage.SetActive(false);

        // Store puzzle camera's target position/rotation
        if (puzzleCamera != null)
        {
            puzzleCameraTargetPosition = puzzleCamera.transform.position;
            puzzleCameraTargetRotation = puzzleCamera.transform.rotation;

            if (playerCamera != null)
            {
                puzzleCamera.transform.position = playerCamera.transform.position;
                puzzleCamera.transform.rotation = playerCamera.transform.rotation;
            }

            puzzleCamera.gameObject.SetActive(true);
            StartCoroutine(TransitionCamera(puzzleCamera.transform, puzzleCameraTargetPosition, puzzleCameraTargetRotation, true, true));
        }

        if (playerCamera != null) playerCamera.gameObject.SetActive(false);

        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        for (int i = 0; i < cubes.Length; i++)
            if (cubes[i] != null)
                cubes[i].SetActive(true);

        StartCoroutine(FadeCubes(0f, 1f, fadeTransitionDuration));
        if (meshRenderer != null)
            StartCoroutine(FadeObject(1f, 0f, fadeTransitionDuration));
    }

    public void StopInteract()
    {
        if (!puzzleActive && !puzzleCompleted) return;

        puzzleActive = false;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        // Fade out
        StartCoroutine(FadeCubes(1f, 0f, fadeTransitionDuration));
        if (meshRenderer != null)
            StartCoroutine(FadeObject(0f, 1f, fadeTransitionDuration));
        if (alignmentText != null)
            StartCoroutine(FadeAlignmentText(1f, 0f, fadeTransitionDuration));

        // Transition camera back
        if (puzzleCamera != null && playerCamera != null)
        {
            StartCoroutine(TransitionCameraAndSwitch());
        }
        else
        {
            if (playerCamera != null) playerCamera.gameObject.SetActive(true);
            if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(false);
        }

        // Show prompt again
        if (puzzlePromptMessage != null)
            puzzlePromptMessage.SetActive(true);

        ResetPuzzle();
        StartCoroutine(UnlockInteractionNextFrame());
    }

    private IEnumerator TransitionCameraAndSwitch()
    {
        Vector3 playerPos = playerCamera.transform.position;
        Quaternion playerRot = playerCamera.transform.rotation;

        yield return StartCoroutine(TransitionCamera(puzzleCamera.transform, playerPos, playerRot, false, true));

        playerCamera.gameObject.SetActive(true);
        puzzleCamera.gameObject.SetActive(false);

        puzzleCamera.transform.position = puzzleCameraTargetPosition;
        puzzleCamera.transform.rotation = puzzleCameraTargetRotation;

        for (int i = 0; i < cubes.Length; i++)
            if (cubes[i] != null)
                cubes[i].SetActive(false);
    }

    private IEnumerator TransitionCamera(Transform cam, Vector3 targetPos, Quaternion targetRot, bool isEntering, bool showAlignmentAfter = false)
    {
        isCameraTransitioning = true;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        float elapsed = 0f;

        while (elapsed < cameraTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cameraTransitionDuration;
            t = t * t * (3f - 2f * t);

            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        cam.position = targetPos;
        cam.rotation = targetRot;
        isCameraTransitioning = false;

        // Show alignment text after camera finishes moving
        if (isEntering && alignmentText != null)
        {
            alignmentText.text = "Alignment: 0%";
            StartCoroutine(FadeAlignmentText(0f, 1f, fadeTransitionDuration));
        }

        interactionLocked = false;
    }

    // -----------------------------
    // Fade Transitions
    // -----------------------------
    private IEnumerator FadeCubes(float startAlpha, float endAlpha, float duration)
    {
        if (cubeMaterials == null || cubeOriginalColors == null)
        {
            var matList = new System.Collections.Generic.List<Material>();
            var colorList = new System.Collections.Generic.List<Color>();
            for (int i = 0; i < cubes.Length; i++)
            {
                if (cubes[i] != null)
                {
                    Renderer renderer = cubes[i].GetComponent<Renderer>();
                    if (renderer != null && renderer.material != null)
                    {
                        matList.Add(renderer.material);
                        colorList.Add(renderer.material.color);
                    }
                }
            }
            cubeMaterials = matList.ToArray();
            cubeOriginalColors = colorList.ToArray();
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            for (int i = 0; i < cubeMaterials.Length; i++)
            {
                if (cubeMaterials[i] != null)
                {
                    Color col = cubeOriginalColors[i];
                    col.a = alpha;
                    cubeMaterials[i].color = col;
                }
            }

            yield return null;
        }

        for (int i = 0; i < cubeMaterials.Length; i++)
        {
            if (cubeMaterials[i] != null)
            {
                Color col = cubeOriginalColors[i];
                col.a = endAlpha;
                cubeMaterials[i].color = col;
            }
        }
    }

    private IEnumerator FadeObject(float startAlpha, float endAlpha, float duration)
    {
        if (meshRenderer == null) yield break;

        if (objectMaterial == null)
        {
            objectMaterial = meshRenderer.material;
            objectOriginalColor = objectMaterial.color;
        }

        if (endAlpha > startAlpha)
            meshRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            Color col = objectOriginalColor;
            col.a = Mathf.Lerp(startAlpha, endAlpha, t);
            objectMaterial.color = col;

            yield return null;
        }

        Color final = objectOriginalColor;
        final.a = endAlpha;
        objectMaterial.color = final;

        if (endAlpha == 0f)
            meshRenderer.enabled = false;
    }

    private IEnumerator FadeAlignmentText(float startAlpha, float endAlpha, float duration)
    {
        if (alignmentText == null) yield break;

        CanvasGroup cg = alignmentText.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = alignmentText.gameObject.AddComponent<CanvasGroup>();

        if (endAlpha > startAlpha)
            alignmentText.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        cg.alpha = endAlpha;

        if (endAlpha == 0f)
            alignmentText.gameObject.SetActive(false);
    }

    private IEnumerator UnlockInteractionNextFrame()
    {
        yield return null;
        interactionLocked = false;
    }

    // -----------------------------
    // Initialization
    // -----------------------------
    private void Start()
    {
        if (playerCamera == null) Debug.LogError("Player camera not assigned!");
        if (puzzleCamera == null) Debug.LogError("Puzzle camera not assigned!");
        if (solutionImages == null || solutionImages.Length == 0)
        {
            Debug.LogError("Solution images array is empty!");
            return;
        }
        if (cubePrefab == null) Debug.LogError("Cube prefab not assigned!");

        SelectRandomImage();

        meshRenderer = GetComponent<MeshRenderer>();
        if (alignmentText != null) alignmentText.gameObject.SetActive(false);

        if (playerCamera != null) playerCamera.gameObject.SetActive(true);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(false);

        if (puzzlePromptMessage != null)
            puzzlePromptMessage.SetActive(true);

        GeneratePuzzle();
        GenerateSolutionViewportPositions();
        DynamicScatterPuzzle();
    }

    // -----------------------------
    // Image Selection
    // -----------------------------
    private void SelectRandomImage()
    {
        if (remainingImages == null || remainingImages.Count == 0)
            InitializeRemainingImages();

        if (remainingImages.Count == 0)
        {
            Debug.LogError("No valid solution images found in array!");
            return;
        }

        int randomIndex = Random.Range(0, remainingImages.Count);
        currentSolutionImage = remainingImages[randomIndex];
        remainingImages.RemoveAt(randomIndex);

        if (remainingImages.Count == 0)
            Debug.Log("All solution images completed! Resetting pool.");
    }

    private void InitializeRemainingImages()
    {
        remainingImages = new System.Collections.Generic.List<Texture2D>();
        foreach (var img in solutionImages)
        {
            if (img != null)
                remainingImages.Add(img);
        }
    }

    // -----------------------------
    // Runtime Logic
    // -----------------------------
    private void Update()
    {
        if (!puzzleActive && !puzzleCompleted) return;

        HandleRotation();
        UpdateAlignment();

        if (puzzleActive && !interactionLocked && !isCameraTransitioning &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            StopInteract();
        }
    }

    private void HandleRotation()
    {
        if (!puzzleActive || isCameraTransitioning) return;

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentRotationEuler.y += mouseX * rotationSpeed * Time.deltaTime;
            currentRotationEuler.x -= mouseY * rotationSpeed * Time.deltaTime;
            cubeContainer.transform.eulerAngles = currentRotationEuler;
        }
    }

    // -----------------------------
    // Alignment + Z adjustment
    // -----------------------------
    private void UpdateAlignment()
    {
        if (cubes == null || solutionViewportPositions == null || puzzleCamera == null || puzzleCompleted) return;

        int aligned = 0;
        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null) continue;

            Vector3 screenPos = puzzleCamera.WorldToViewportPoint(cubes[i].transform.position);
            float dist = Vector2.Distance(
                new Vector2(screenPos.x, screenPos.y),
                new Vector2(solutionViewportPositions[i].x, solutionViewportPositions[i].y)
            );

            if (dist < alignmentThreshold) aligned++;
        }

        float percent = (float)aligned / cubes.Length;
        if (alignmentText != null && alignmentText.gameObject.activeSelf)
            alignmentText.text = $"Alignment: {percent * 100f:F0}%";

        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null) continue;
            Vector3 basePos = solutionPositions[i];
            float adjustedZ = initialZOffsets[i] * (1f - percent);
            cubes[i].transform.localPosition = new Vector3(basePos.x, basePos.y, adjustedZ);
        }

        if (percent >= completionAlignmentPercentage / 100f && !completionCoroutineRunning)
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

        yield return new WaitForSeconds(3f);

        StopInteract();
        completionCoroutineRunning = false;
    }

    private void ResetPuzzle()
    {
        puzzleCompleted = false;
        SelectRandomImage();
        GeneratePuzzle();
        GenerateSolutionViewportPositions();
        DynamicScatterPuzzle();
        if (alignmentText != null)
            alignmentText.text = "Alignment: 0%";
    }

    // -----------------------------
    // Puzzle Generation
    // -----------------------------
    private void GeneratePuzzle()
    {
        if (currentSolutionImage == null || cubePrefab == null) return;

        if (cubeContainer != null) Destroy(cubeContainer);
        cubeContainer = new GameObject("CubeContainer");
        cubeContainer.transform.SetParent(transform);
        cubeContainer.transform.localPosition = Vector3.zero;

        int width = currentSolutionImage.width;
        int height = currentSolutionImage.height;

        float cubeSizeX = maxPuzzleSize / width;
        float cubeSizeY = maxPuzzleSize / height;
        cubeSize = Mathf.Min(cubeSizeX, cubeSizeY);

        int blackPixelCount = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (currentSolutionImage.GetPixel(x, y).r < 0.5f)
                    blackPixelCount++;

        solutionPositions = new Vector3[blackPixelCount];
        cubes = new GameObject[blackPixelCount];
        initialZOffsets = new float[blackPixelCount];

        float imageWidth = width * cubeSize;
        float imageHeight = height * cubeSize;
        Vector3 origin = new Vector3(-imageWidth / 2f, -imageHeight / 2f, 0f);

        int index = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (currentSolutionImage.GetPixel(x, y).r < 0.5f)
                {
                    Vector3 localPos = origin + new Vector3((x + 0.5f) * cubeSize, (y + 0.5f) * cubeSize, 0f);
                    solutionPositions[index] = localPos;

                    GameObject cube = Instantiate(cubePrefab, cubeContainer.transform);
                    cube.transform.localPosition = localPos;
                    cube.transform.localScale = Vector3.one * cubeSize;
                    cube.SetActive(false);
                    cubes[index] = cube;

                    index++;
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
        if (cubes == null || cubeContainer == null || solutionPositions == null || puzzleCamera == null) return;

        float cameraDistance = Mathf.Abs(puzzleCamera.transform.position.z - cubeContainer.transform.position.z);
        float maxScatter = cameraDistance * zScatterMultiplier;

        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null) continue;
            Vector3 basePos = solutionPositions[i];

            float randomZ = Random.Range(-maxScatter, maxScatter);
            initialZOffsets[i] = randomZ;

            cubes[i].transform.localPosition = new Vector3(basePos.x, basePos.y, randomZ);
            cubes[i].transform.localScale = Vector3.one * cubeSize;
            cubes[i].SetActive(false);
        }

        float rotX = Random.Range(initialRotationMin, initialRotationMax);
        float rotY = Random.Range(initialRotationMin, initialRotationMax);
        cubeContainer.transform.eulerAngles = new Vector3(rotX, rotY, 0f);
        currentRotationEuler = cubeContainer.transform.eulerAngles;
    }
}
