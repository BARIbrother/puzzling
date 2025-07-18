using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PositionData
{
    public List<Vector3> AnswerPositions;

    public PositionData(List<Vector3> source)
    {
        AnswerPositions = source;
    }
}
