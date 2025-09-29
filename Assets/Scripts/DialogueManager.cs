using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine.UI;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{

    public GameObject dialogueCanvas;
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
    public bool cbMode = false;
    public int currentDNum = 0;
    public int currentCDNum = 0;
    public bool canGoToNext = false;

    //log
    public GameObject logPanel;
    public TextMeshProUGUI logText;
    private List<string> dialogueLog = new List<string>();
    public RectTransform LogRect;
    public bool logMode = false;

    //setting
    public GameObject SettingPanel;
    public bool settingMode = false;

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
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && canGoToNext == true && !logMode && !settingMode)
            {
                NextDialogue();
            }

            if (Input.GetKeyDown(KeyCode.L) && canGoToNext)
            {
                logPanel.SetActive(!logPanel.activeSelf);
                logMode = !logMode;

                if (SettingPanel.activeSelf)
                {
                    SettingPanel.SetActive(!SettingPanel.activeSelf);
                    settingMode = !settingMode;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && canGoToNext)
            {
                SettingPanel.SetActive(!SettingPanel.activeSelf);
                settingMode = !settingMode;
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

        if (line.soundname.Length > 0) SFXManager.Instance.Play(line.soundname, 0.3f, true);
        StartCoroutine(TypeText(line));

        if (!string.IsNullOrEmpty(line.speaker))
        {
            Sprite portrait = LoadPortrait(line.speaker);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                Debug.Log("portrait Loaded");
            }
            else
            {
                Debug.Log("portrait Load failed");
            }
        }

        string logEntry = $"<b>{line.speaker}</b>\n {line.text}";
        dialogueLog.Add(logEntry);
        logText.text = string.Join("\n\n_____________________________________________", dialogueLog);

        Vector2 size = LogRect.sizeDelta;

        // 높이를 100 늘리기
        size.y += 100f;
        LogRect.sizeDelta = size;
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
                popupImage.gameObject.SetActive(true);
                popupImage.sprite = ImagesToPopUp[CPIindex];
                CPIindex++;
                break;
            case "ZO":
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
                yield return StartCoroutine(EyeBlinkEffect.Instance.FadeFill(0f, 1f));
                yield return new WaitForSeconds(1f);
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
            case "S":
                bodyText.gameObject.SetActive(false);
                dialoguePanel.gameObject.SetActive(false);
                yield return new WaitForSeconds(3f);
                bodyText.gameObject.SetActive(true);
                dialoguePanel.gameObject.SetActive(true);
                break;
            case "BWOD":
                yield return StartCoroutine(EyeBlinkEffect.Instance.BlinkSequence());
                portraitImage.gameObject.SetActive(false);
                break;
            case "DOWN":
                yield return StartCoroutine(P_Moveup());
                break;
            case "UP":
                yield return StartCoroutine(P_Movedown());
                break;
            case "P2":
                portraitImage.sprite = Resources.Load<Sprite>("Portraits/StoryB");
                break;
            case "P3":
                portraitImage.sprite = Resources.Load<Sprite>("Portraits/StoryC");
                break;
        }

        bodyText.text = "";
        foreach (char c in line.text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        if (line.evt == "GA")
        {
            yield return ShakeCameraEffect.Instance.Gasp();
        }

        canGoToNext = true;
        if (line.evt == "ST")
        {
            bodyText.gameObject.SetActive(false);
            bodyText.text = "";
            dialoguePanel.gameObject.SetActive(false);
            backgroundImage.gameObject.SetActive(false);
            portraitImage.gameObject.SetActive(false);
            canGoToNext = false;

            GameManager.Instance.StartPuzzleStage();
            GameManager.Instance.AlreadyPassed = true;

            PuzzleManager.Instance.ThirdGimic(15f);
            yield return new WaitForSeconds(3f);
            bodyText.gameObject.SetActive(true);
            dialoguePanel.gameObject.SetActive(true);
            canGoToNext = true;
        }
        if (line.evt == "ED")
        {
            StartCoroutine(ZoomCameraEffect.Instance.Zoomout_Image(portraitImage, new Vector2(240, 404), new Vector2(0, 202)));
            StartCoroutine(ZoomCameraEffect.Instance.Zoomout_Background(backgroundImage, new Vector3(1, 1, 1)));
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
        bodyText.text = "";
        bodyText.gameObject.SetActive(false);
        dialoguePanel.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        portraitImage.gameObject.SetActive(false);
        canGoToNext = false;
        lines.Clear();
        EyeBlinkEffect.Instance.blackOverlay.gameObject.SetActive(false);
        if (!cbMode)
        {
            GameManager.Instance.EndDialogueStage();
        }
        else
        {
            cbMode = true;
        }
    }

    public List<DialogueLine> ParseCSV(TextAsset csvFile)
    {
        List<DialogueLine> l = new List<DialogueLine>();
        string[] rows = csvFile.text.Split('\n');

        foreach (string row in rows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            string[] fields = row.Split(',');
            DialogueLine line = new DialogueLine { };
            if (fields[0].Trim().Length == 0)
            {
                break;
            }
            switch (fields.Length)
            {
                case 2:
                    line.speaker = fields[0].Trim();
                    line.text = fields[1].Trim();
                    l.Add(line);
                    break;
                case 3:
                    line.speaker = fields[0].Trim();
                    line.text = fields[1].Trim();
                    line.evt = fields[2].Trim();
                    l.Add(line);
                    break;
                default:
                    line.speaker = fields[0].Trim();
                    line.text = fields[1].Trim();
                    line.evt = fields[2].Trim();
                    line.soundname = fields[3].Trim();
                    l.Add(line);
                    break;
            }
        }

        return l;
    }


    private Sprite LoadPortrait(string portraitName)
    {
        Debug.Log((portraitName == "???"));
        // Resources/Portraits 폴더에서 불러오기
        switch (portraitName)
        {
            case "???":
                return Resources.Load<Sprite>("Portraits/story_regular_0");
        }
        return null;
    }

    public void ProgressCallback()
    {
        isOnDialogue = true;
        currentLine = 0;
        speakerText.gameObject.SetActive(true);
        bodyText.gameObject.SetActive(true);
        dialoguePanel.gameObject.SetActive(true);
        EyeBlinkEffect.Instance.blackOverlay.gameObject.SetActive(true);
        EyeBlinkEffect.Instance.blackOverlay.fillAmount = 1f;
        cbMode = true;
        ShowLine();
    }
    
    public IEnumerator P_Movedown()
    {
        yield return P_Moveto(Vector3.down * 300f, 1f);
    }


    public IEnumerator P_Moveup()
    {
        yield return P_Moveto(Vector3.up * 60f, 1f);
    }

    IEnumerator P_Moveto(Vector3 howmuch, float time)
    {
        Vector3 initialPos = portraitImage.transform.position;
        Vector3 BinitialPos = backgroundImage.transform.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            portraitImage.transform.position = Vector3.Lerp(initialPos, initialPos + howmuch, elapsed / time);
            backgroundImage.transform.position = Vector3.Lerp(BinitialPos, BinitialPos + howmuch, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        portraitImage.transform.position = initialPos + howmuch;
        backgroundImage.transform.position = BinitialPos + howmuch;
    }
}
