using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject TutorialCanvas;
    public TextMeshProUGUI[] tutoMessages;
    public int currentIndex = 0;
    public bool isTutoState = false;

    public static TutorialManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        TutorialCanvas.SetActive(false);
        foreach (TextMeshProUGUI t in tutoMessages)
        {
            t.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTutoState)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                tutoMessages[currentIndex].enabled = false;
                currentIndex++;
                if (currentIndex >= tutoMessages.Length)
                {

                    TutorialCanvas.SetActive(false);
                    isTutoState = false;
                    GameManager.Instance.EndTutorialStage();
                }
                else tutoMessages[currentIndex].enabled = true;
            }
        }
    }

    public void ShowTutorial()
    {
        TutorialCanvas.SetActive(true);
        StartCoroutine(w1());
        tutoMessages[currentIndex].enabled = true;
    }

    public IEnumerator w1()
    {
        yield return new WaitForSeconds(0.1f);
        isTutoState = true;
    }
}
