using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Vector2 cursorPos;
    [SerializeField] private Transform cursorTransform;
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        cursorTransform.position = GetCursorPos();
    }
    public Vector2 GetCursorPos()
    {
        cursorPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return cursorPos;
    }
}