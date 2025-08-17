using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;   

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI bodyText;
    //public Image portraitImage;

    private List<DialogueLine> lines;
    private int currentLine = 0;

    public bool isOnDialogue = false;

    public List<TextAsset> DialogueData;
    public int currentDNum = 0;
    public bool canGoToNext = false;

    Camera cam;
    public static DialogueManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("대화 시작");
        StartDialogue();
        cam = Camera.main;
    }

    void Update()
    {
        if (isOnDialogue)
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && canGoToNext == true)
            {
                NextDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        List<DialogueLine> dialogueLines = ParseCSV(DialogueData[currentDNum]);
        lines = dialogueLines;
        isOnDialogue = true;
        currentLine = 0;
        speakerText.gameObject.SetActive(true); 
        bodyText.gameObject.SetActive(true);
        dialoguePanel.gameObject.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        var line = lines[currentLine];
        canGoToNext = false;
        speakerText.text = line.speaker;
        StartCoroutine(TypeText(line));
        //portraitImage.sprite = LoadPortrait(line.portrait);
    }

    IEnumerator TypeText(DialogueLine line)
    {
        switch (line.evt)
        {
            case "B":
                yield return StartCoroutine(EyeBlinkEffect.Instance.BlinkSequence());
                break;
            case "ZI":
                StartCoroutine(ZoomCameraEffect.Instance.ZoomIO(20f, 1f));
                yield return new WaitForSeconds(1f);
                StartCoroutine(ZoomCameraEffect.Instance.ZoomIO(25f, 1f));
                break;
        }

        bodyText.text = "";
        foreach (char c in line.text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        if (line.evt == "S")
        {
            yield return new WaitForSeconds(3f);
        }
        canGoToNext = true;
    }

    public void NextDialogue()
    {
        if (currentLine < lines.Count - 1)
        {
            currentLine++;
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        // 대화 종료 처리
        speakerText.gameObject.SetActive(false);
        bodyText.gameObject.SetActive(false);
        dialoguePanel.gameObject.SetActive(false);
        Debug.Log("대화 종료");
        GameManager.Instance.EndDialogueStage();
    }
    
    public List<DialogueLine> ParseCSV(TextAsset csvFile)
    {
        List<DialogueLine> l = new List<DialogueLine>();
        string[] rows = csvFile.text.Split('\n');
        Debug.Log(rows[0]);

        foreach (string row in rows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            string[] fields = row.Split(',');

            if (fields.Length == 2)
            {
                DialogueLine line = new DialogueLine
                {
                    speaker = fields[0].Trim(),
                    text = fields[1].Trim(),
                };
                l.Add(line);
            }
            else if (fields.Length == 3)
            {
                DialogueLine line = new DialogueLine
                {
                    speaker = fields[0].Trim(),
                    text = fields[1].Trim(),
                    evt = fields[2].Trim()
                };
                l.Add(line);
            }
        }

        return l;
    }
}
