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

    private Coroutine progressCoroutine;
    private Action onComplete;


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
        fillImage.fillAmount = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        fillImage.fillAmount = 1f;
        onComplete?.Invoke();
        progressCoroutine = null;
    }
}
