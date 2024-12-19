﻿using UnityEngine;

public sealed class RotationCursor : MonoBehaviour
{
    [SerializeField, Range(0, 500)] float objectSize = 10f;

    private bool updateRotation = false;

    private Camera mainCamera;

    private void Awake() => mainCamera = Camera.main;

    private void Update() 
    {
        transform.localScale = PixelScaleUtility.GetWorldScaleFromPixels(objectSize, transform.position);

        if (updateRotation) transform.LookAt(mainCamera.transform.position);
    }

    public void Hide() => gameObject.SetActive(false);

    public void ShowAt(Vector3 getPoint)
    {
        transform.position = getPoint;

        updateRotation = gameObject.activeSelf == false;

        gameObject.SetActive(true);
    }
}