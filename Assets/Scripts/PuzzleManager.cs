using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public List<Piece> pieces;
    public List<Vector3> AnswerPositions;

    //instance template
    public static PuzzleManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintAllRelativeLocations();
        }
    }

    //checking if released piece fits into other pieces
    public void CheckConnection(Piece currentPiece)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] == currentPiece || Mathf.Abs(currentPiece.relativePositions[i].x - 987654321) <= 0.1f) continue;
            //when answer contains p as relative piece for currentpiece
            Vector3 currentRelativePos = pieces[i].transform.position - currentPiece.transform.position;
            Vector3 answerRelativePos = RotateVector(new Vector3(currentPiece.relativePositions[i].x, currentPiece.relativePositions[i].y, 0), currentPiece.transform.eulerAngles.z);
            //checking if angle is right
            if (Mathf.Abs(Mathf.DeltaAngle(currentPiece.transform.eulerAngles.z, pieces[i].transform.eulerAngles.z)) <= 1f)
            {
                if (Vector3.Distance(currentRelativePos, answerRelativePos) < 0.1f)
                {
                    Connect(currentPiece, i);
                }
            }
        }
    }

    //정답과 놓은 곳의 오차를 원래 움직이던 piece 기준으로 보정해줌 + 두 조각 하나로 연결
    public void Connect(Piece dragged, int i)
    {

        dragged.transform.position = pieces[i].transform.position + RotateVector(new Vector3(pieces[i].relativePositions[pieces.IndexOf(dragged)].x, pieces[i].relativePositions[pieces.IndexOf(dragged)].y, 0), dragged.transform.eulerAngles.z);
        if (!(dragged.connectedPieces.Contains(pieces[i]) || pieces[i] == dragged))
        {
            dragged.connectedPieces.Add(pieces[i]);
            pieces[i].connectedPieces.Add(dragged);
            Debug.Log("connected to piece " + (i + 1));
        }
    }

    public List<Piece> GetConnectedGroup(Piece start)
    {
        List<Piece> visited = new List<Piece>();
        Stack<Piece> stack = new Stack<Piece>();

        visited.Add(start);
        stack.Push(start);

        while (stack.Count > 0)
        {
            Piece current = stack.Pop();

            foreach (Piece neighbor in current.connectedPieces)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
        }

        return visited;
    }

    void PrintAllRelativeLocations()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            for (int j = 0; j < pieces.Count; j++)
            {
                if (i != j)
                {
                    Debug.Log("piece from" + i + "to" + j + ": " + (pieces[j].transform.position - pieces[i].transform.position).ToString("F3"));
                }
            }
        }
    }

    public Vector3 RotateVector(Vector3 v, float degree)
    {
        float radians = degree * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float x = v.x * cos - v.y * sin;
        float y = v.x * sin + v.y * cos;

        return new Vector3(x, y, 0);
    }

    public void AdjustRotation(Piece center, float degree)
    {
        List<Piece> cp = GetConnectedGroup(center);
        foreach (Piece p in cp)
        {
            Vector3 rp = p.transform.position - center.transform.position;
            p.transform.position -= rp;
            p.transform.position += RotateVector(rp, degree);
            p.transform.Rotate(0f, 0f, degree);
        }
    }

    public void CheckAnswer(Piece p)
    {
        if (Vector3.Distance(p.transform.position, AnswerPositions[pieces.IndexOf(p)]) < 0.1f)
        {
            p.inRightPos = true;
        }
    }
}
