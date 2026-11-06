using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Collections;

public class ComputerReveal : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomOutAmount = 2f;
    [SerializeField] private float zoomDuration = 3f;

    [Header("Computer Icon")]
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Collider2D iconCollider;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Black Screen")]
    [SerializeField] private GameObject blackScreen;

    [Header("Timing")]
    [SerializeField] private float zoomDelay = 0.5f;

    [Header("Final PNG Fade In")]
    [SerializeField] private SpriteRenderer finalPngRenderer;
    [SerializeField, Range(0f, 1f)] private float finalAlpha = 1f;
    [SerializeField] private float fadeDuration = 2f;

    private bool videoStarted = false;
    private bool revealStarted = false;

    private void Start()
    {
        if (blackScreen != null)
            blackScreen.SetActive(false);

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.Stop();

        videoPlayer.loopPointReached += OnVideoFinished;

        // Make the final PNG invisible at the beginning
        if (finalPngRenderer != null)
        {
            Color color = finalPngRenderer.color;
            color.a = 0f;
            finalPngRenderer.color = color;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void Update()
    {
        if (videoStarted ||
            Mouse.current == null ||
            mainCamera == null ||
            iconCollider == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Read the mouse position
        Vector2 screenPosition = Mouse.current.position.ReadValue();

        // Ignore invalid mouse positions
        if (float.IsNaN(screenPosition.x) ||
            float.IsNaN(screenPosition.y) ||
            float.IsInfinity(screenPosition.x) ||
            float.IsInfinity(screenPosition.y))
            return;

        // Ignore clicks outside the game window
        if (screenPosition.x < 0 ||
            screenPosition.y < 0 ||
            screenPosition.x > Screen.width ||
            screenPosition.y > Screen.height)
            return;

        // Convert screen position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            )
        );

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            if (hit == iconCollider)
            {
                StartVideo();
                return;
            }
        }
    }

    private void StartVideo()
    {
        videoStarted = true;

        // Hide the icon completely
        iconRenderer.gameObject.SetActive(false);

        // Start the video
        videoPlayer.Play();

        Debug.Log("VIDEO STARTED!");
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        if (revealStarted)
            return;

        revealStarted = true;

        Debug.Log("VIDEO FINISHED! Waiting before zoom...");

        // Activate the black screen
        if (blackScreen != null)
            blackScreen.SetActive(true);

        StartCoroutine(ZoomOut());
    }

    private IEnumerator ZoomOut()
    {
        // Wait before starting the zoom
        yield return new WaitForSecondsRealtime(zoomDelay);

        Debug.Log("ZOOM STARTED!");

        float startingSize = mainCamera.orthographicSize;
        float targetSize = startingSize + zoomOutAmount;
        float elapsedTime = 0f;

        // Slowly zoom out
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / zoomDuration);

            mainCamera.orthographicSize = Mathf.Lerp(
                startingSize,
                targetSize,
                progress
            );

            yield return null;
        }

        mainCamera.orthographicSize = targetSize;

        Debug.Log("CAMERA ZOOM OUT COMPLETE!");

        // Fade in the final PNG after zooming finishes
        StartCoroutine(FadeInFinalPng());
    }

    private IEnumerator FadeInFinalPng()
    {
        if (finalPngRenderer == null)
        {
            Debug.LogError("Final PNG Renderer has not been assigned!");
            yield break;
        }

        float elapsedTime = 0f;
        Color color = finalPngRenderer.color;

        // Gradually increase alpha
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            color.a = Mathf.Lerp(0f, finalAlpha, progress);
            finalPngRenderer.color = color;

            yield return null;
        }

        // Ensure the final alpha is exact
        color.a = finalAlpha;
        finalPngRenderer.color = color;

        Debug.Log("FINAL PNG FADE COMPLETE!");
    }
}