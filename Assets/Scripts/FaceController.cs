using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class FaceController : MonoBehaviour
{
    public static FaceController _instance;

    [SerializeField] GameObject[] blushMakeUps;
    [SerializeField] GameObject[] eyeshadowMakeUps;
    [SerializeField] GameObject[] lipstickMakeUps;

    private void Start()
    {
        _instance = this;
    }

    public void StartShow(int tabIndex, int colorIndex)
    {
        switch (tabIndex)
        {
            case 0:
                EyeshadowShow(colorIndex);
                break;
            case 1:
                BlushShow(colorIndex);
                break;
            case 2:
                LipstickShow(colorIndex);
                break;
            default:
                break;
        }
    }

    public void BlushShow(int index)
    {
        for (int i = 0; i < blushMakeUps.Length; i++)
        {
            blushMakeUps[i].SetActive(i == index);
        }
    }

    public void EyeshadowShow(int index)
    {
        for (int i = 0; i < eyeshadowMakeUps.Length; i++)
        {
            eyeshadowMakeUps[i].SetActive(i == index);
        }
    }

    public void LipstickShow(int index)
    {
        for (int i = 0; i < lipstickMakeUps.Length; i++)
        {
            lipstickMakeUps[i].SetActive(i == index);
        }
    }
}
