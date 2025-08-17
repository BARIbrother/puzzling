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
}
