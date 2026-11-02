
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ComputerIntro : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Zoom Targets")]
    [SerializeField] private Transform zoomPoint;
    [SerializeField] private Transform screenCenter;

    [Header("Zoom Settings")]
    [SerializeField] private float startingSize = 5f;
    [SerializeField] private float firstZoomSize = 2f;
    [SerializeField] private float finalZoomSize = 1f;

    [SerializeField] private float firstZoomDuration = 12f;
    [SerializeField] private float secondZoomDuration = 5f;

    [Header("Computer Sprites")]
    [SerializeField] private SpriteRenderer computerRenderer;
    [SerializeField] private Sprite computerOff;

    [Header("Startup Animation")]
    [SerializeField] private Sprite[] startupFrames;
    [SerializeField] private float frameDuration = 1.3f;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "ComputerScreen";
    [SerializeField] private float transitionDelay = 0.5f;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve zoomCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Start()
    {
        mainCamera.orthographicSize = startingSize;
        computerRenderer.sprite = computerOff;

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Start computer animation independently
        Coroutine animation = StartCoroutine(PlayStartupAnimation());

        // First camera zoom
        yield return StartCoroutine(ZoomCamera(
            zoomPoint.position,
            firstZoomSize,
            firstZoomDuration
        ));

        // Second camera zoom
        yield return StartCoroutine(ZoomCamera(
            screenCenter.position,
            finalZoomSize,
            secondZoomDuration
        ));

        // Wait for animation to finish
        yield return animation;

        // Small pause before changing scenes
        yield return new WaitForSeconds(transitionDelay);

        // Load the computer screen scene
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator PlayStartupAnimation()
    {
        for (int i = 0; i < startupFrames.Length; i++)
        {
            computerRenderer.sprite = startupFrames[i];

            yield return new WaitForSeconds(frameDuration);
        }

        // Keep the final frame displayed
        computerRenderer.sprite =
            startupFrames[startupFrames.Length - 1];
    }

    private IEnumerator ZoomCamera(
        Vector3 targetPosition,
        float targetSize,
        float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;

        targetPosition.z = startPosition.z;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = zoomCurve.Evaluate(t);

            mainCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                easedT
            );

            mainCamera.orthographicSize = Mathf.Lerp(
                startSize,
                targetSize,
                easedT
            );

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = targetSize;
    }
}
