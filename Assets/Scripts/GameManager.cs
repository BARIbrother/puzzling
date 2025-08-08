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
    public List<GameStage> stages = new List<GameStage>{ GameStage.D, GameStage.P, GameStage.D, GameStage.P, GameStage.P };
    public static GameManager Instance { get; private set; }

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

    }

    void StartPuzzleStage()
    {
        Debug.Log("Start Puzzle");
        pm.LoadPuzzle();
    }

    public void EndPuzzleStage()
    {
        Debug.Log("End Puzzle");
        if (pm.currentPuzzleIndex < pm.totalPuzzleNumber - 1)
        {
            pm.currentPuzzleIndex += 1;
        }

        NextStage();
    }

    void StartDialogueStage()
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
        }    

        
    }
}
