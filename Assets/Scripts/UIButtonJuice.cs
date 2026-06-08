using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 clickScale = new Vector3(0.9f, 0.9f, 0.9f);
    public float animationSpeed = 10f;

    private Vector3 targetScale = Vector3.one;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = Vector3.Scale(originalScale, hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = Vector3.Scale(originalScale, clickScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = Vector3.Scale(originalScale, hoverScale);
    }
}
