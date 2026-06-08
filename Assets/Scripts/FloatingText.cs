using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("이동 설정")]
    public float verticalSpeed = 200f;   // 위로 솟구치는 속도
    public float driftRange = 100f;      // 좌우 랜덤 이동 범위
    public float fadeSpeed = 1.5f;       // 사라지는 속도
    public float lifetime = 0.8f;        // 유지 시간

    private TextMeshProUGUI text;
    private float randomXSpeed;          // 무작위로 결정될 좌우 속도
    private float initialLifetime;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        // 생성 시 좌우 랜덤 속도 결정 (사용자 요청: "랜덤으로 막 이동시켜")
        randomXSpeed = Random.Range(-driftRange, driftRange);
        initialLifetime = lifetime;
    }

    void Update()
    {
        // 1. 이동: 위로 솟구치면서 좌우로 무작위 드리프트
        transform.Translate(new Vector3(randomXSpeed, verticalSpeed, 0) * Time.deltaTime);
        
        // 2. 시간 경과에 따른 처리
        lifetime -= Time.deltaTime;
        
        // 3. 페이드 아웃: 수명이 절반 이하로 떨어지면 서서히 투명해짐
        if (lifetime <= initialLifetime * 0.5f)
        {
            Color newColor = text.color;
            newColor.a -= fadeSpeed * Time.deltaTime;
            text.color = newColor;

            if (newColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetText(string value)
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        text.text = value;
    }
}
