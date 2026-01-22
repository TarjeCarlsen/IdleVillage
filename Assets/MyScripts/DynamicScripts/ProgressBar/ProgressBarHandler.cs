using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
public class ProgressBarHandler : MonoBehaviour
{

    [Header("UI Reference")]
    [SerializeField] private Image fillImage;

    [Header("Options")]
    [SerializeField] private bool autoStart = false;
    [SerializeField] private float duration = 5f;
    [SerializeField] private bool reversed = false;

    private Coroutine progressCoroutine;
    private event Action onComplete;


    /// <To Call function>
    ///                     myProgressBar.StartProgress(timeToComplete, () => Debug.Log("Done!"));
    /// </To call function>


    private void Start()
    {
        if (autoStart)
            StartProgress(duration);
    }

    public void StartProgress(float time, Action onCompleteCallback = null)
    {
        if (progressCoroutine != null)
        {
            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        duration = time;
        onComplete = onCompleteCallback;
        progressCoroutine = StartCoroutine(FillRoutine());
    }

    public void ResetProgress()
    {
        if (progressCoroutine != null)
            StopCoroutine(progressCoroutine);

        fillImage.fillAmount = 0f;
    }

    private IEnumerator FillRoutine()
    {
        float elapsed = 0f;
        if (reversed)
        {
        // Start full and drain to empty
        fillImage.fillAmount = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }

        fillImage.fillAmount = 0f;

        }
        else
        {
            fillImage.fillAmount = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            fillImage.fillAmount = 1f;
        }
        onComplete?.Invoke();
        progressCoroutine = null;
    }

    /// <summary>
    /// Instantly sets the progress bar to a given percentage (0 to 1).
    /// </summary>
    /// <param name="percentage">The target fill percentage (0.0 to 1.0)</param>
    public void SetProgress(float percentage)
    {
        fillImage.fillAmount = Mathf.Clamp01(percentage);
    }
}
