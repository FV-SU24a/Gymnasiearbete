
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class BrainScanInteract : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Popups")]
    [SerializeField] private GameObject brainScanPopup;
    [SerializeField] private GameObject loadingPopup;

    [Header("Brain Scan Icon")]
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Collider2D iconCollider;
    [SerializeField] private TextMeshPro folderName;

    [Header("Loading Animation")]
    [SerializeField] private SpriteRenderer loadingRenderer;
    [SerializeField] private Sprite[] loadingFrames;
    [SerializeField] private float frameDuration = 0.1f;
    [SerializeField] private float finishDelay = 0.5f;

    [Header("Chapter 3")]
    [SerializeField] private ArtificialBodyInteract artificialBodyInteract;

    private bool isProcessing = false;
    private bool scanFinished = false;

    private void Start()
    {
        brainScanPopup.SetActive(false);
        loadingPopup.SetActive(false);
    }

    private void Update()
    {
        // Detect mouse or trackpad click on Brain Scan icon
        if (!isProcessing &&
            !scanFinished &&
            !brainScanPopup.activeSelf &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            RaycastHit2D hit = Physics2D.Raycast(
                mousePosition,
                Vector2.zero
            );

            if (hit.collider == iconCollider)
            {
                Debug.Log("BRAIN SCAN CLICKED!");
                brainScanPopup.SetActive(true);
            }
        }

        // Detect Y or N while popup is visible
        if (isProcessing ||
            !brainScanPopup.activeSelf ||
            Keyboard.current == null)
            return;

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            StartCoroutine(PlayBrainScan());
        }
        else if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            brainScanPopup.SetActive(false);
        }
    }

    private IEnumerator PlayBrainScan()
    {
        isProcessing = true;

        // Hide confirmation popup and show loading popup
        brainScanPopup.SetActive(false);
        loadingPopup.SetActive(true);

        // Check loading frames
        if (loadingFrames == null ||
            loadingFrames.Length == 0 ||
            loadingRenderer == null)
        {
            Debug.LogError("Loading frames or Loading Renderer are missing!");
            loadingPopup.SetActive(false);
            isProcessing = false;
            yield break;
        }

        // Play all loading frames in order
        for (int i = 0; i < loadingFrames.Length; i++)
        {
            loadingRenderer.sprite = loadingFrames[i];

            yield return new WaitForSeconds(frameDuration);
        }

        // Wait half a second after animation
        yield return new WaitForSeconds(finishDelay);

        // Hide loading popup
        loadingPopup.SetActive(false);

        // Hide Brain Scan icon
        if (iconRenderer != null)
            iconRenderer.enabled = false;

        if (iconCollider != null)
            iconCollider.enabled = false;

        // Hide the folder name
        if (folderName != null)
            folderName.enabled = false;

        // Show the Artificial Body folder
        if (artificialBodyInteract != null)
        {
            artificialBodyInteract.ShowFolder();
        }
        else
        {
            Debug.LogError("Artificial Body Interact is not assigned!");
        }

        scanFinished = true;
        isProcessing = false;

        Debug.Log("BRAIN SCAN COMPLETE!");
    }
}
