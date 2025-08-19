using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZoomCameraEffect : MonoBehaviour
{
    public static ZoomCameraEffect Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator ZoomIO(float targetSize, float duration)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetSize, t);
            yield return null;
        }
    }

    public IEnumerator Zoomout_Image(Image img, Vector2 targetSize, Vector2 targetPosition)
    {
        Vector2 startSize = img.rectTransform.sizeDelta;
        Vector2 startPos = img.rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 3f);

            // 크기 보간
            img.rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            // 위치 보간
            img.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);

            yield return null;
        }

        // 마지막에 정확히 목표값 보정
        img.rectTransform.sizeDelta = targetSize;
        img.rectTransform.anchoredPosition = targetPosition;
    }
    
    public IEnumerator Zoomout_Background(Image img, Vector3 newScale)
    {
        Vector3 startScale = img.transform.localScale;
        float elapsed = 0f;

        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 3f);

            img.transform.localScale = Vector3.Lerp(startScale, newScale, t);

            yield return null;
        }

        // 보정
        img.transform.localScale = newScale;
    }
}
