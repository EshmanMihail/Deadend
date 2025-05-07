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
}
