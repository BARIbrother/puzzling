using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShakeCameraEffect : MonoBehaviour
{
    public static ShakeCameraEffect Instance { get; private set; }

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

    void Update()
    {
        
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("MainCamera가 씬에 없습니다!");
            yield break;
        }

        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float yOffset = Mathf.Sin(elapsed * Mathf.PI*2) * magnitude; 
            cam.transform.localPosition = originalPos + new Vector3(0, yOffset, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 카메라 원래 위치로 복구
        cam.transform.localPosition = originalPos;
    }

    public IEnumerator Faint()
    {
        Camera cam = Camera.main;
        float elapsed = 0f;

        Quaternion startRot = cam.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, 30f);

        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.5f;

            cam.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        cam.transform.rotation = endRot; // 최종 위치 고정
    }

    public IEnumerator Gasp()
    {
        Camera cam = Camera.main;
        Vector3 startPos = cam.transform.position;

        for (int i = 0; i < 5; i++)
        {
            // 위로 이동
            yield return MoveTo(startPos + Vector3.up * 1.5f, 1f);
            // 아래로 이동
            yield return MoveTo(startPos, 1f);
        }

        // 마지막에 원래 자리 복원
        transform.position = startPos;
    }

    IEnumerator MoveTo(Vector3 target, float time)
    {
        Camera cam = Camera.main;
        Vector3 initialPos = cam.transform.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            cam.transform.position = Vector3.Lerp(initialPos, target, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = target;
    }
}
