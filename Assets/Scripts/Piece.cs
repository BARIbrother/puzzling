using UnityEngine;
using System.Collections.Generic;


public class Piece : MonoBehaviour
{
    public List<Vector3> relativePositions;
    public List<Piece> connectedPieces = new List<Piece>();
    public float answerAngle;
    public bool clicked = false;
    private Camera mainCamera;

    private Vector3 lastmouseWorld = new Vector3(0, 0, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.Rotate(0f, 0f, 90f); // 왼쪽으로 90도 회전
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.Rotate(0f, 0f, -90f); // 오른쪽으로 90도 회전
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log("clicked");
        clicked = true;
        lastmouseWorld = GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (clicked)
        {
            
            Vector3 currentmouseWorld = GetMouseWorldPosition();
            Vector3 delta = currentmouseWorld - lastmouseWorld;
            lastmouseWorld = currentmouseWorld;

            List<Piece> cg = PuzzleManager.Instance.GetConnectedGroup(this);
            for (int i = 0; i < cg.Count; i ++)
            {
                cg[i].transform.position += delta;
            }
        }
    }

    void OnMouseUp()
    {
        clicked = false;
        PuzzleManager.Instance.CheckConnection(this);
    }


    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(mainCamera.transform.position.z); // z값은 카메라에서의 거리
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

}
