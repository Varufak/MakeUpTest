using UnityEngine;

public class MakeUpTypes : MonoBehaviour
{
    private GameObject[] panels;
    [SerializeField] private GameObject eyeBrush, blushBrush;

    private void Awake()
    {
        panels = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            panels[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        SelectTab(0);
    }

    public void SelectTab(int tabIndex)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == tabIndex);
            eyeBrush.SetActive(tabIndex == 3);
            blushBrush.SetActive(tabIndex == 1);
        }
    }
}
