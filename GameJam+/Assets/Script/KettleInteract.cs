using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleInteract : MonoBehaviour
{
    public bool CanInteract = false;
    public Transform zoomTarget;      // posisi teko untuk zoom
    public float zoomDuration = 0.5f;
    public Camera mainCamera;
    public GameObject kettlePanel;    // panel teko untuk puzzle
    public Vector3 originalCamPos;
    public float originalCamSize;

    private bool isZoomed = false;
    private bool panelOpened = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;
    }

    void OnMouseDown()
    {
        if (!CanInteract) return;

        if (!isZoomed)
        {
            StartCoroutine(ZoomToKettle());
        }
        else if (isZoomed && !panelOpened)
        {
            OpenPanel();
        }
    }

    IEnumerator ZoomToKettle()
    {
        isZoomed = true;
        Vector3 targetPos = new Vector3(zoomTarget.position.x, zoomTarget.position.y, originalCamPos.z);
        float targetSize = originalCamSize * 0.4f; // zoom in

        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float p = t / zoomDuration;

            mainCamera.transform.position = Vector3.Lerp(originalCamPos, targetPos, p);
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetSize, p);

            yield return null;
        }
    }

    private void OpenPanel()
    {
        if (kettlePanel != null)
            kettlePanel.SetActive(true);

        panelOpened = true;
        Debug.Log("Panel teko terbuka!");
    }

    public void ClosePanel()
    {
        if (!isZoomed) return;
        StartCoroutine(ZoomOut());
    }

    IEnumerator ZoomOut()
    {
        float t = 0f;
        Vector3 currentPos = mainCamera.transform.position;
        float currentSize = mainCamera.orthographicSize;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float p = t / zoomDuration;

            mainCamera.transform.position = Vector3.Lerp(currentPos, originalCamPos, p);
            mainCamera.orthographicSize = Mathf.Lerp(currentSize, originalCamSize, p);

            yield return null;
        }

        isZoomed = false;
        panelOpened = false;
    }
}
