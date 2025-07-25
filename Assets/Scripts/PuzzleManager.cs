using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    public List<Piece> pieces;
    public List<GameObject> blurredpieces;
    public List<Vector3> AnswerPositions;

    public int currentPuzzleIndex = 4;
    private int totalPuzzleNumber = 4;


    private Shader solidColorShader;

    //var for third gimic
    public bool onThirdGimic = false;
    private int piececount = 0;
    private List<Piece> piecesToChangeWhite;
    private static System.Random rng = new System.Random();
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
        LoadPuzzle(currentPuzzleIndex);
        solidColorShader = Shader.Find("GUI/Text Shader");
        if (solidColorShader == null)
        {
            Debug.LogError("내장 셰이더 'GUI/Text Shader'를 찾을 수 없습니다! 단색 변경이작동하지 않을 수 있습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintAllRelativeLocations();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentPuzzleIndex -= 1;
            LoadPuzzle(currentPuzzleIndex% totalPuzzleNumber + 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentPuzzleIndex += 1;
            LoadPuzzle(currentPuzzleIndex % totalPuzzleNumber + 1);
        }
        //first gimic
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(ChangeColorAFterDelay(5f));
        }
        //second gimic
        //activates when starting game
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("second gimic");
            ChangeToBlurredPiece();
        }
        //third gimic
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartThirdGimic();
        }
    }

    //checking if released piece fits into other pieces
    public void CheckConnection(Piece currentPiece)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] == currentPiece || !currentPiece.answerCP.Contains(i) || currentPiece.inRightPos) continue;
            //when answer contains p as relative piece for currentpiece
            Vector3 currentRelativePos = pieces[i].transform.position - currentPiece.transform.position;
            Vector3 answerRelativePos = RotateVector(new Vector3(AnswerPositions[pieces.IndexOf(pieces[i])].x - AnswerPositions[pieces.IndexOf(currentPiece)].x,
                                                                AnswerPositions[pieces.IndexOf(pieces[i])].y - AnswerPositions[pieces.IndexOf(currentPiece)].y, 0),
                                                                currentPiece.transform.eulerAngles.z);
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

        dragged.transform.position = pieces[i].transform.position + RotateVector(new Vector3(AnswerPositions[pieces.IndexOf(dragged)].x - AnswerPositions[pieces.IndexOf(pieces[i])].x,
                                                                                             AnswerPositions[pieces.IndexOf(dragged)].y - AnswerPositions[pieces.IndexOf(pieces[i])].y, 0),
                                                                                              dragged.transform.eulerAngles.z);
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
        if (Vector3.Distance(p.transform.position, AnswerPositions[pieces.IndexOf(p)]) < 0.1f && Mathf.Abs(p.transform.rotation.z) < 1f)
        {
            p.inRightPos = true;
            p.transform.position = AnswerPositions[pieces.IndexOf(p)];
            if (onThirdGimic)
            {
                piececount++;
                CheckPieceCount();
            }
        }
    }

    public void LoadPuzzle(int index)
    {
        //clear previous data
        pieces = new List<Piece>();
        blurredpieces = new List<GameObject>();
        AnswerPositions = new List<Vector3>();

        //loading puzzle position data
        TextAsset jsonFile = Resources.Load<TextAsset>($"PuzzleData/{index}/answerPos");
        PositionData data = JsonUtility.FromJson<PositionData>(jsonFile.text);
        AnswerPositions = data.AnswerPositions;

        //loading puzzle piece prefab
        GameObject[] prefabs = Resources.LoadAll<GameObject>($"Prefabs/Puzzle_{index}");

        var sortedPrefabs = prefabs.OrderBy(p => p.name).ToArray();
        foreach (GameObject prefab in sortedPrefabs)
        {
            if (!(prefab.tag == "Blurred"))
            {
                GameObject pieceObj = Instantiate(prefab);
                Piece piece = pieceObj.GetComponent<Piece>();
                if (piece != null)
                {
                    pieces.Add(piece);
                }
                else
                {
                    Debug.LogWarning($"Piece 컴포넌트가 없는 프리팹: {prefab.name}");
                }
            }
            else
            {
                blurredpieces.Add(prefab);
            }
        }
        Debug.Log("pieces loaded");
    }

    private IEnumerator ChangeColorAFterDelay(float delaySeconds)
    {
        Debug.Log($"changing puzzle color to white in {delaySeconds} seconds");
        yield return new WaitForSeconds(delaySeconds);
        ChangeObjectsToWhite();
    }

    public void ChangeObjectsToWhite()
    {
        foreach (Piece piece in pieces)
        {
            if (piece == null)
            {
                Debug.LogWarning("리스트에 비어있는(null) 오브젝트가 있습니다.");
                continue;
            }

            SpriteRenderer sr = piece.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.material.shader = solidColorShader;
                sr.material.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"'{piece.name}' 오브젝트에 Renderer 컴포넌트가 없어 색상을 변경할 수 없습니다.");
            }
        }
    }

    public void ChangeToBlurredPiece()
    {
        foreach (GameObject g in blurredpieces)
        {
            foreach (Piece p in pieces)
            {
                Debug.Log(p.name);
                if (g.name.Contains(p.name.Substring(0,p.name.Length-7)))
                {
                    GameObject pieceObj = Instantiate(g);
                    Piece piece = pieceObj.GetComponent<Piece>();

                    piece.transform.position = p.transform.position;
                    piece.transform.rotation = p.transform.rotation;
                    piece.connectedPieces = p.connectedPieces;
                    piece.clicked = p.clicked;
                    piece.inRightPos = p.inRightPos;
                    pieces[pieces.IndexOf(p)] = piece;
                    Destroy(p.gameObject);
                    break;
                }
            }
        }
    }

    public static List<T> ShuffleList<T>(List<T> originalList)
    {
        if (originalList == null || originalList.Count <= 1)
        {
            return new List<T>(originalList);
        }

        List<T> shuffledList = new List<T>(originalList);

        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (shuffledList[k], shuffledList[n]) = (shuffledList[n], shuffledList[k]);
        }

        return shuffledList;
    }

    public void StartThirdGimic()
    {
        onThirdGimic = true;
    }
    public void CheckPieceCount()
    {
        if (piececount % 2 == 0)
        {
            List<Piece> imsi = ShuffleList(pieces);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (!imsi[i].inRightPos)
                {
                    SpriteRenderer sr = imsi[i].GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.material.shader = solidColorShader;
                        sr.material.color = Color.white;
                    }
                    break;
                }
            }
        }
    }
}
