using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum GameStage { D, P }
public class GameManager : MonoBehaviour
{
    public DialogueManager dm;
    public PuzzleManager pm;
    public int currentStageIndex = -1;
    public List<GameStage> stages = new List<GameStage>{ GameStage.D, GameStage.P, GameStage.D, GameStage.D, GameStage.P,GameStage.D,GameStage.D,GameStage.P,GameStage.D,GameStage.D,GameStage.P,GameStage.D,GameStage.P, GameStage.D};
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
            else
            {
                PuzzleManager.Instance.ClearPuzzle();
                pm.currentPuzzleIndex += 1;
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
        else
        {
            StartPuzzleStage();
            PuzzleManager.Instance.can_click = true;
        }    

        
    }
}
