using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    public float hoverScale = 1.1f;
    public Color hoverColor = Color.yellow;
    
    private Vector3 originalScale;
    private Color originalColor;
    private TextMeshProUGUI buttonText;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        originalScale = rectTransform.localScale;
        if (buttonText != null) originalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = originalScale * hoverScale;
        if (buttonText != null) buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = originalScale;
        if (buttonText != null) buttonText.color = originalColor;
    }
}
