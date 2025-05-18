using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image healthFillingBar;
    [SerializeField] private Image flashlightFillingBar;

    public Image[] slots;
    public Image[] slotsImages;
    public Sprite emptySlotSprite;

    public GameObject startLeverUI;
    public Image startLeverFillingBar;
    public Text startLeverErrorText;
    public Image fadeImage;

    [SerializeField] private Vector3 hintOffset = new (0, 2, 0);
    public Text hint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public Image GetFlashlightFillingBar()
    {
        return flashlightFillingBar;
    }

    public Image GetFillingBar()
    {
        return healthFillingBar;
    }

    public void ShowTextHint(Vector3 pos, string text)
    {
        if (hint != null)
        {
            hint.text = text;
            hint.gameObject.transform.position = pos + hintOffset;
            hint.gameObject.SetActive(true);
        }     
    }

    public void HideTextHint()
    {
         if (hint != null) hint.gameObject.SetActive(false);
    }
}
