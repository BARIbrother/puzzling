using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EyeBlinkEffect : MonoBehaviour
{
    public Image blackOverlay;
    public float speed = 1f;

    public static EyeBlinkEffect Instance { get; private set; }

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

    public IEnumerator BlinkSequence()
    {
        blackOverlay.gameObject.SetActive(true);
        // 눈을 뜸 (밑에서 위로 검은색 사라짐)
        yield return StartCoroutine(FadeFill(1f, 0.5f));

        // 잠깐 멈춤
        yield return new WaitForSeconds(1f);

        // 눈 감기 (위에서 아래로 다시 내려옴)
        yield return StartCoroutine(FadeFill(0.5f, 1f));

        // 다시 눈 뜸
        yield return StartCoroutine(FadeFill(1f, 0f));
        blackOverlay.gameObject.SetActive(false);
    }

    public IEnumerator FadeFill(float from, float to)
    {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.fillAmount = 1f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            blackOverlay.fillAmount = Mathf.Lerp(from, to, t);
            yield return null;
        }
        blackOverlay.fillAmount = to;
    }
}
