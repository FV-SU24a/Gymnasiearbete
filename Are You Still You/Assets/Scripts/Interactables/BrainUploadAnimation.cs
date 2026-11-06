
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BrainUploadAnimation : MonoBehaviour
{
    [Header("Loading Bar")]
    [SerializeField] private SpriteRenderer loadingBarRenderer;
    [SerializeField] private Sprite[] loadingFrames;

    [Header("Animation Settings")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float frameDuration = 0.5f;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float sceneLoadDelay = 0.5f;

    private void Start()
    {
        loadingBarRenderer.enabled = false;

        StartCoroutine(PlayLoadingAnimation());
    }

    private IEnumerator PlayLoadingAnimation()
    {
        // Wait half a second after entering Chapter 4
        yield return new WaitForSeconds(startDelay);

        // Make the loading bar visible
        loadingBarRenderer.enabled = true;

        // Play all frames
        for (int i = 0; i < loadingFrames.Length; i++)
        {
            loadingBarRenderer.sprite = loadingFrames[i];

            yield return new WaitForSeconds(frameDuration);
        }

        Debug.Log("BRAIN UPLOAD COMPLETE!");

        // Wait half a second after the animation finishes
        yield return new WaitForSeconds(sceneLoadDelay);

        // Load the next scene
        if (!string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next Scene Name has not been entered!");
        }
    }
}
