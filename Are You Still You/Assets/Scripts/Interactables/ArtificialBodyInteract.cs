
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ArtificialBodyInteract : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Artificial Body Folder")]
    [SerializeField] private GameObject artificialBodyFolder;
    [SerializeField] private Collider2D folderCollider;

    [Header("Popups")]
    [SerializeField] private GameObject imagePopup;
    [SerializeField] private GameObject bodySelectionPopup;
    [SerializeField] private GameObject bodyModificationPopup;

    [Header("Image Popup - Close Button")]
    [SerializeField] private Collider2D closeButtonCollider;

    [Header("Body Selection")]
    [SerializeField] private Collider2D body01Collider;
    [SerializeField] private Collider2D body02Collider;
    [SerializeField] private Collider2D body03ToggleCollider;

    [Header("Body Modification")]
    [SerializeField] private Collider2D loadBrainScanCollider;

    [Header("Chapter 4")]
    [SerializeField] private string chapter4SceneName;

    private bool folderOpened = false;
    private bool bodySelected = false;
    private bool uploadSelected = false;

    private void Start()
    {
        // Make sure all Chapter 3 popups start hidden
        imagePopup.SetActive(false);
        bodySelectionPopup.SetActive(false);
        bodyModificationPopup.SetActive(false);

        // Folder starts hidden until the Brain Scan finishes
        artificialBodyFolder.SetActive(false);
    }

    // Called by the Brain Scan script
    public void ShowFolder()
    {
        artificialBodyFolder.SetActive(true);

        Debug.Log("ARTIFICIAL BODY FOLDER APPEARED!");
    }

    private void Update()
    {
        if (Mouse.current == null || mainCamera == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Get mouse position in world space
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        // Find all colliders underneath the mouse
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePosition);

        // STEP 1: Click Artificial Body folder
        if (!folderOpened && artificialBodyFolder.activeSelf)
        {
            if (WasClicked(folderCollider, hits))
            {
                Debug.Log("ARTIFICIAL BODY FOLDER CLICKED!");

                folderOpened = true;
                imagePopup.SetActive(true);
                return;
            }
        }

        // STEP 2: Close creepy image popup
        if (imagePopup.activeSelf)
        {
            if (WasClicked(closeButtonCollider, hits))
            {
                Debug.Log("IMAGE POPUP CLOSED!");

                imagePopup.SetActive(false);
                bodySelectionPopup.SetActive(true);
                return;
            }
        }

        // STEP 3: Select an artificial body
        if (bodySelectionPopup.activeSelf && !bodySelected)
        {
            if (WasClicked(body01Collider, hits))
            {
                Debug.Log("BODY 01 IS TAKEN!");
                return;
            }

            if (WasClicked(body02Collider, hits))
            {
                Debug.Log("BODY 02 IS TAKEN!");
                return;
            }

            if (WasClicked(body03ToggleCollider, hits))
            {
                Debug.Log("BODY 03 SELECTED!");

                bodySelected = true;

                bodySelectionPopup.SetActive(false);
                bodyModificationPopup.SetActive(true);
                return;
            }
        }

        // STEP 4: Body modification menu
        if (bodyModificationPopup.activeSelf && !uploadSelected)
        {
            if (WasClicked(loadBrainScanCollider, hits))
            {
                Debug.Log("LOAD BRAIN SCAN INTO BODY SELECTED!");

                uploadSelected = true;

                // Load Chapter 4 using the scene name entered in the Inspector
                if (!string.IsNullOrWhiteSpace(chapter4SceneName))
                {
                    SceneManager.LoadScene(chapter4SceneName);
                }
                else
                {
                    Debug.LogError("Chapter 4 Scene Name has not been entered!");
                    uploadSelected = false;
                }

                return;
            }
        }
    }

    // Checks whether a particular collider was clicked
    private bool WasClicked(Collider2D target, Collider2D[] hits)
    {
        if (target == null)
            return false;

        foreach (Collider2D hit in hits)
        {
            if (hit == target)
                return true;
        }

        return false;
    }
}
