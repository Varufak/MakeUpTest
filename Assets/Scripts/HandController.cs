using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 POSITION_DELTA_TO_HAND = new Vector2 (87f, -275f);
    private Vector2 POSITION_DELTA_TO_BRUSH = new Vector2(87f, -380f);

    [SerializeField] private RectTransform face;
    [SerializeField] private RectTransform hand;
    [SerializeField] private RectTransform brushPivot;
    [SerializeField] private GameObject[] brushes;
    [SerializeField] private GameObject[] brushesInHand;
    [SerializeField] private Color[] colors;
    private int colorIndex;
    private int tabIndex;
    private Canvas canvas;
    private Camera mainCamera;
    private Vector3 dragOffset;
    private bool isAutoMoving = true;

    void Start()
    {
        mainCamera = Camera.main;
        canvas = hand.GetComponentInParent<Canvas>();
    }

    public void EyeshadowColorButtonClicked(int color)
    {
        StartCoroutine(HandToBrushAndPick(brushes[0].transform.position, 1));
        colorIndex = color;
        tabIndex = 0;
    }
    public void BlushColorButtonClicked(int color)
    {
        StartCoroutine(HandToBrushAndPick(brushes[0].transform.position, 0));
        colorIndex = color;
        tabIndex = 1;
    }

    IEnumerator HandToBrushAndPick(Vector2 brushPos, int tabIndex)
    {
        float t = 0f;
        Vector2 start = hand.position;
        Vector2 end = brushPos;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            hand.position = Vector2.Lerp(start, end, t);
            yield return null;
        }
        for (int i = 0; i < brushes.Length; i++)
        {
            brushesInHand[i].SetActive(i == tabIndex);
        }
        brushes[tabIndex].SetActive(false);
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(HandToColor(clickedObject.transform.position, brushesInHand[tabIndex].transform.position , 0));
    }

    IEnumerator HandToColor(Vector2 colorPos, Vector2 brushPos, int tabIndex)
    {
        float t = 0f;
        Vector2 start = hand.position;
        Vector2 end = start + (colorPos - brushPos);

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            hand.position = Vector2.Lerp(start, end, t);
            yield return null;
        }
        StartCoroutine(PaintBrushShake(hand));
    }

    public IEnumerator PaintBrushShake(RectTransform hand)
    {
        float duration = 1f;
        float amplitude = 10f;
        float frequency = 6f;
        Vector2 startPos = hand.localPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float offset = Mathf.Sin(t * frequency * Mathf.PI * 2) * amplitude;
            hand.localPosition = startPos + Vector2.right * offset;
            yield return null;
        }

        hand.localPosition = startPos;
        StartCoroutine(HandToDragPosition());
    }

    IEnumerator HandToDragPosition()
    {
        float t = 0f;
        Vector2 start = hand.position;
        Vector2 brushPos = brushes[0].transform.position;
        Vector2 end = brushPos + new Vector2(0f, 200f);

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            hand.position = Vector2.Lerp(start, end, t);
            yield return null;
        }
        isAutoMoving = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isAutoMoving || mainCamera == null)
            return;

        Vector3 handScreenPos = mainCamera.WorldToScreenPoint(hand.position);
        dragOffset = hand.position - (new Vector3(eventData.position.x, eventData.position.y, handScreenPos.z));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isAutoMoving || mainCamera == null)
            return;

        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, hand.position.z);
        Vector2 newWorldPos = screenPoint + dragOffset;
        hand.position = newWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isAutoMoving)
            return;
        RectTransform brushPivot = brushesInHand[0].GetComponent<RectTransform>();
        if (RectTransformUtility.RectangleContainsScreenPoint(face, brushPivot.transform.position, canvas.worldCamera))
        {
            FaceController._instance.StartShow(tabIndex, colorIndex);
        }
    }
}

