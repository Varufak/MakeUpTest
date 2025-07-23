using UnityEngine.UI;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class MakeUpTabs : MonoBehaviour
{
	private Image image;
	private RectTransform rectTransform;
	[SerializeField] private Sprite selected, unselected;
	[SerializeField] private List<MakeUpTabs> allTabs;

	private void Start()
	{
        allTabs = new List<MakeUpTabs>(transform.parent.GetComponentsInChildren<MakeUpTabs>());
        image = GetComponent<Image>();
		rectTransform = GetComponent<RectTransform>();
    }

	public void TabClicked()
	{
		foreach (MakeUpTabs tab in allTabs)
		{
			tab.SetSelected(false);
		}
		SetSelected(true);
	}

	public void SetSelected(bool selectedCond)
	{
		if (selectedCond)
		{
			image.sprite = selected;
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 10f);
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 143f);
		}
		else if (!selectedCond)
		{
			image.sprite = unselected;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0f);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 123f);
        }
	}
}
