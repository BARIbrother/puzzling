using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class PuzzleJsonExportor : MonoBehaviour
{
    [ContextMenu("Export Puzzle Data to JSON")]
    public void ExportToJson()
    {
        // 예: 현재 위치들을 정답으로 저장
        List<Vector3> answerPositions = new List<Vector3>();
        foreach (Transform child in transform)
        {
            answerPositions.Add(child.position);
        }

        PositionData ans = new PositionData(answerPositions);
        string json = JsonUtility.ToJson(ans, true);

        string path = Application.dataPath + "/Resources/PuzzleData/Tutorial/answerPos.json";
        File.WriteAllText(path, json);

        Debug.Log("Puzzle JSON 저장 완료: " + path);

        // Unity에 파일 변경 알림
        AssetDatabase.Refresh();
    }
}
