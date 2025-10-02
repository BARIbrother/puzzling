using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;


public enum GameStage { D, P , E, T}
public class GameManager : MonoBehaviour
{
    public DialogueManager dm;
    public PuzzleManager pm;
    public MainScreenManager mm;
    public TutorialManager tm;
    public int currentStageIndex = -1;
    public List<GameStage> stages = new List<GameStage>{ GameStage.D, GameStage.T, GameStage.P, GameStage.D, GameStage.D, GameStage.P,GameStage.D,GameStage.D,GameStage.P,GameStage.D,GameStage.D,GameStage.P,GameStage.D,GameStage.P, GameStage.D, GameStage.E};
    public static GameManager Instance { get; private set; }

    public bool AlreadyPassed = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        dm = DialogueManager.Instance;
        pm = PuzzleManager.Instance;
        mm = MainScreenManager.Instance;
        tm = TutorialManager.Instance;
        NextStage();
    }


    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (stages[currentStageIndex] == GameStage.D)
            {
                DialogueManager.Instance.EndDialogue();
                dm.currentDNum += 1;
            }
            else if(stages[currentStageIndex] == GameStage.P)
            {
                PuzzleManager.Instance.ClearPuzzle();
                pm.currentPuzzleIndex += 1;
            }   
            else if(stages[currentStageIndex] == GameStage.E)
            {
                mm.ChangeToEndingScene();
            }
            NextStage(); 
        }
    }

    public void StartPuzzleStage()
    {
        Debug.Log("Start Puzzle");
        if(!AlreadyPassed)
        {
            pm.LoadPuzzle();
        }
        else
        {
            AlreadyPassed = false;
        }
    }

    public void EndPuzzleStage()
    {
        Debug.Log("End Puzzle");
        AlreadyPassed = false;
        if (pm.currentPuzzleIndex < pm.totalPuzzleNumber - 1)
        {
                pm.currentPuzzleIndex += 1;
        }

        NextStage();
    }

    public void StartDialogueStage()
    {
        Debug.Log("Start Dialogue");
        dm.StartDialogue();
    }

    public void EndDialogueStage()
    {
        Debug.Log("End Dialogue");
        if (dm.currentDNum < dm.DialogueData.Count - 1)
        {
            dm.currentDNum += 1;
        }
        NextStage();
    }

    public void StartTutorialStage()
    {
        Debug.Log("Start Tutorial");
        tm.ShowTutorial();
        StartPuzzleStage();
    }

    public void EndTutorialStage()
    {
        NextStage();
    }



    void NextStage()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            Debug.Log("going to stage:" + (currentStageIndex + 1));
            currentStageIndex += 1;
        }

        if (stages[currentStageIndex] == GameStage.D)
        {
            StartDialogueStage();
        }
        else if (stages[currentStageIndex] == GameStage.P)
        {
            StartPuzzleStage();
            PuzzleManager.Instance.can_click = true;
        }
        else if (stages[currentStageIndex] == GameStage.E)
        {
            mm.ChangeToEndingScene();
        }
        else if (stages[currentStageIndex] == GameStage.T)
        {
            StartTutorialStage();
        }

        
    }
}
