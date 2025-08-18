using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI bodyText;
    public Image portraitImage;
    public Image backgroundImage;
    public Image popupImage;
    //public Image portraitImage;

    public List<DialogueLine> lines;
    private int currentLine = 0;

    public bool isOnDialogue = false;
    public List<Sprite> ImagesToPopUp;
    public int CPIindex = 0;

    public List<TextAsset> DialogueData;
    public List<TextAsset> DialogueData_callback;
    public int currentDNum = 0;
    public int currentCDNum = 0;
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
        //StartDialogue();
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
        backgroundImage.gameObject.SetActive(true);
        portraitImage.gameObject.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        Debug.Log(lines.Count);
        var line = lines[currentLine];
        canGoToNext = false;
        speakerText.text = line.speaker;
        StartCoroutine(TypeText(line));

        if (!string.IsNullOrEmpty(line.speaker))
        {
            Sprite portrait = LoadPortrait(line.speaker);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                Debug.Log("portrait Loaded");    
            }
            else{
                Debug.Log("portrait Load failed");
            }
        }
    }

    IEnumerator TypeText(DialogueLine line)
    {
        Debug.Log("Dialogue started");
        switch (line.evt)
        {
            case "B":
                yield return StartCoroutine(EyeBlinkEffect.Instance.BlinkSequence());
                break;
            case "ZI":
                //yield return StartCoroutine(ZoomCameraEffect.Instance.ZoomIO(20f, 1f));
                popupImage.gameObject.SetActive(true);
                popupImage.sprite = ImagesToPopUp[CPIindex];
                CPIindex ++;
                break;
            case "ZO":
                //yield return StartCoroutine(ZoomCameraEffect.Instance.ZoomIO(25f, 1f));
                popupImage.sprite = null;
                popupImage.gameObject.SetActive(false);
                break;
            case "TH":
                Debug.Log("TH activated");
                speakerText.gameObject.SetActive(false);
                bodyText.gameObject.SetActive(false);
                dialoguePanel.gameObject.SetActive(false);
                backgroundImage.gameObject.SetActive(false);
                portraitImage.gameObject.SetActive(false);
                canGoToNext = false;
                Debug.Log("closed conv ui");

                GameManager.Instance.StartPuzzleStage();
                PuzzleManager.Instance.can_click = false;
                GameManager.Instance.AlreadyPassed = true;
                Debug.Log("started puzzle");

                yield return new WaitForSeconds(2f);
                //yield return StartCoroutine(ShakeCameraEffect.Instance.Shake(0.5f, 1.5f));
                PuzzleManager.Instance.ChangeToBlurredPiece();
                yield return new WaitForSeconds(2f);
                Debug.Log("shake cam");

                speakerText.gameObject.SetActive(true);
                bodyText.gameObject.SetActive(true);
                dialoguePanel.gameObject.SetActive(true);
                backgroundImage.gameObject.SetActive(true);
                portraitImage.gameObject.SetActive(true);
                canGoToNext = true;
                break;

            case "FA":
                //yield return StartCoroutine(ShakeCameraEffect.Instance.Faint());
                yield return StartCoroutine(EyeBlinkEffect.Instance.FadeFill(0f,1f));
                yield return new WaitForSeconds(5f);
                break;

            case "PO":
                portraitImage.gameObject.SetActive(false);
                break;
            case "PI":
                portraitImage.gameObject.SetActive(true);
                break;
            case "ON":
                backgroundImage.gameObject.SetActive(false);
                portraitImage.gameObject.SetActive(false);

                GameManager.Instance.StartPuzzleStage();
                PuzzleManager.Instance.can_click = false;
                GameManager.Instance.AlreadyPassed = true;
                yield return new WaitForSeconds(5f);
                break;
            case "BA":
                backgroundImage.gameObject.SetActive(true);
                portraitImage.gameObject.SetActive(true);
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
            bodyText.gameObject.SetActive(false);
            dialoguePanel.gameObject.SetActive(false);
            yield return new WaitForSeconds(3f);
            bodyText.gameObject.SetActive(true);
            dialoguePanel.gameObject.SetActive(true);
        }
        if(line.evt == "GA")
        {
            yield return ShakeCameraEffect.Instance.Gasp();
        }

        canGoToNext = true;
        if(line.evt == "ST")
        {
            bodyText.gameObject.SetActive(false);
            dialoguePanel.gameObject.SetActive(false);
            backgroundImage.gameObject.SetActive(false);
            portraitImage.gameObject.SetActive(false);
            canGoToNext = false;

            GameManager.Instance.StartPuzzleStage();
            GameManager.Instance.AlreadyPassed = true;
            StartCoroutine(ExecuteSTAfterDelay(15f));
        }
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

    public void EndDialogue()
    {
        // 대화 종료 처리
        speakerText.gameObject.SetActive(false);
        bodyText.gameObject.SetActive(false);
        dialoguePanel.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        portraitImage.gameObject.SetActive(false);
        lines.Clear();
        EyeBlinkEffect.Instance.blackOverlay.gameObject.SetActive(false);
        GameManager.Instance.EndDialogueStage();
    }
    
    public List<DialogueLine> ParseCSV(TextAsset csvFile)
    {
        List<DialogueLine> l = new List<DialogueLine>();
        string[] rows = csvFile.text.Split('\n');

        foreach (string row in rows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            string[] fields = row.Split(',');

            
            if (fields.Length >= 3)
            {
                DialogueLine line = new DialogueLine
                {
                    speaker = fields[0].Trim(),
                    text = fields[1].Trim(),
                    evt = fields[2].Trim()
                };
                l.Add(line);
            }
            else
            {
                DialogueLine line = new DialogueLine
                {
                    speaker = fields[0].Trim(),
                    text = fields[1].Trim(),
                };
                l.Add(line);
            }
        }

        return l;
    }


    private Sprite LoadPortrait(string portraitName)
    {
        Debug.Log((portraitName == "???"));
        // Resources/Portraits 폴더에서 불러오기
        switch(portraitName)
        {
            case "???":
                return Resources.Load<Sprite>("Portraits/StoryA");
        }
        return null;
    }

    IEnumerator ExecuteSTAfterDelay(float delay)
    {
        PuzzleManager.Instance.ThirdGimic(15f);
        yield return new WaitForSeconds(delay);
        bodyText.gameObject.SetActive(true);
        dialoguePanel.gameObject.SetActive(true);
        canGoToNext = false;
        NextDialogue();
        PuzzleManager.Instance.FireThirdGimic(30f);
    }

    public void ProgressCallback()
    {
        isOnDialogue = true;
        currentLine = 0;
        speakerText.gameObject.SetActive(true); 
        bodyText.gameObject.SetActive(true);
        dialoguePanel.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);
        portraitImage.gameObject.SetActive(true);
        
    }
}
