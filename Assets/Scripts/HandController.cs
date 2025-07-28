using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

	private Canvas canvas;
	private Camera mainCamera;
	[SerializeField] private RectTransform face;
	[SerializeField] private RectTransform hand;
	[SerializeField] private GameObject[] brushes;
	[SerializeField] private GameObject[] brushesInHand;
	[SerializeField] private GameObject[] lipstickColorsInHand;
	private GameObject clickedObject;
	private int colorIndex;
	private int tabIndex;
	private bool isAutoMoving = true;
	private Vector3 dragOffset;

	void Start()
	{
		mainCamera = Camera.main;
		canvas = hand.GetComponentInParent<Canvas>();
	}

	public void BlushColorButtonClicked(int color)
	{
		clickedObject = EventSystem.current.currentSelectedGameObject;
		colorIndex = color;
		tabIndex = 0;
		StartCoroutine(SequenceCoroutine(tabIndex));
	}
	public void EyeshadowColorButtonClicked(int color)
	{
		clickedObject = EventSystem.current.currentSelectedGameObject;
		colorIndex = color;
		tabIndex = 1;
		StartCoroutine(SequenceCoroutine(tabIndex));
	}

	public void LipstickColorButtonClicked(int color)
	{
		clickedObject = EventSystem.current.currentSelectedGameObject;
		colorIndex = color;
		tabIndex = 2;
		StartCoroutine(SequenceCoroutine(tabIndex));
	}

	private IEnumerator SequenceCoroutine(int tabIndex)
	{
		if (tabIndex < 2)
		{
			yield return StartCoroutine(HandToBrushAndPick(tabIndex));
			yield return new WaitForSeconds(0.1f);
		}
		yield return StartCoroutine(HandToColor(tabIndex));
		yield return new WaitForSeconds(0.1f);
		if (tabIndex < 2)
		{
			yield return StartCoroutine(PaintBrushShake(hand));
			yield return new WaitForSeconds(0.1f);
		}
		yield return StartCoroutine(HandToDragPosition());
	}

	private IEnumerator HandToBrushAndPick(int tabIndex)
	{
		Vector2 start = hand.position;
		Vector2 end = brushes[tabIndex].transform.position;

		float t = 0f;
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
	}

	private IEnumerator HandToColor(int tabIndex)
	{
		Vector2 start = hand.position;
		Vector2 end;
		if (tabIndex < 2)
		{
			end = (Vector2)clickedObject.transform.position + start - (Vector2)brushesInHand[tabIndex].transform.position;
		}
		else
		{
			end = clickedObject.transform.position;
		}
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2f;
			hand.position = Vector2.Lerp(start, end, t);
			yield return null;
		}
		if (tabIndex == 2) ShowInHandColorLipstick();
    }

	private IEnumerator PaintBrushShake(RectTransform hand)
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

	private IEnumerator HandToDragPosition()
	{
		Vector2 start = hand.position;
		Vector2 end = (Vector2)brushes[0].transform.position + new Vector2(0f, 200f);

		float t = 0f;
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

	private void ShowInHandColorLipstick()
	{
		for (int i = 0; i < brushesInHand.Length; i++)
		{
			brushesInHand[i].SetActive(false);
		}
        for (int i = 0; i < lipstickColorsInHand.Length; i++)
        {
            lipstickColorsInHand[i].SetActive(i == colorIndex);
        }
	}
}

