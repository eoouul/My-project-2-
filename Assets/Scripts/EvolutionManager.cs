using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 최신 인풋 시스템 사용
using StickEvolution;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager Instance { get; private set; }

    [Header("무기 상태 (Weapon Status)")]
    public EvolutionNode currentNode;
    public int enhancementLevel = 0;
    public float growthFactor = 1.2f;

    [Header("UI 참조 (UI References)")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI enhanceCostText;
    public TextMeshProUGUI evolutionCostText;
    public Image weaponImage;
    
    [Header("진화 UI (Evolution UI)")]
    public GameObject evolutionChoicePanel;
    public Button[] choiceButtons;
    public Image[] choicePreviewImages;
    public TextMeshProUGUI[] choiceButtonTexts;

    [Header("시각 효과 (Visual Effects)")]
    public GameObject floatingTextPrefab;
    public Canvas uiCanvas;

    [Header("밸런스 설정 (Balance Settings)")]
    public long upgradeBaseCostGold = 100;
    public int evolutionBaseCostStone = 10;
    public long stoneBuyCostGold = 500; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (currentNode == null)
        {
            currentNode = Resources.Load<EvolutionNode>("Stick_00_Root");
        }

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnGoldChanged += UpdateResourceUI;
            ResourceManager.Instance.OnEnhancementStonesChanged += (_) => UpdateResourceUI(ResourceManager.Instance.Gold);
        }

        if (evolutionChoicePanel != null) evolutionChoicePanel.SetActive(false);
        UpdateWeaponUI();
        if (ResourceManager.Instance != null) UpdateResourceUI(ResourceManager.Instance.Gold);
    }

    // [클릭] 재화(골드) 획득 및 연출
    public void OnWeaponClick()
    {
        long goldPerClick = GetGoldPerClick();
        ResourceManager.Instance.AddGold(goldPerClick);

        // 최신 인풋 시스템으로 마우스 위치 가져오기
        Vector2 mousePos = Vector2.zero;
        if (Pointer.current != null)
        {
            mousePos = Pointer.current.position.ReadValue();
        }
        
        // 플로팅 텍스트 생성
        ShowFloatingTextAt($"+{goldPerClick:N0}G", mousePos);
        
        if (weaponImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(PunchScale(weaponImage.transform));
        }
    }

    public long GetGoldPerClick()
    {
        if (currentNode == null) return 1;
        return (long)(currentNode.resourcePerClick * Mathf.Pow(growthFactor, enhancementLevel));
    }

    public void TryEnhance()
    {
        long goldCost = GetUpgradeGoldCost();

        if (ResourceManager.Instance.Gold >= goldCost)
        {
            ResourceManager.Instance.SpendGold(goldCost);
            
            enhancementLevel++;
            
            // 강화 시에도 현재 마우스 위치 사용
            Vector2 clickPos = Pointer.current != null ? Pointer.current.position.ReadValue() : (Vector2)transform.position;
            ShowFloatingTextAt("<color=yellow>강화 성공!</color>", clickPos, Color.yellow);
            
            UpdateWeaponUI();
        }
        else
        {
            string msg = "골드 부족!";
            Vector2 clickPos = Pointer.current != null ? Pointer.current.position.ReadValue() : (Vector2)transform.position;
            ShowFloatingTextAt($"<color=red>{msg}</color>", clickPos, Color.red);
        }
    }

    public long GetUpgradeGoldCost() => (long)(upgradeBaseCostGold * Mathf.Pow(1.5f, enhancementLevel));
    public int GetEvolutionStoneCost()
    {
        // 티어에 비례하여 진화 비용 증가
        // 공식: 기본 비용 * 현재 티어
        int weaponTier = (currentNode != null) ? currentNode.tier : 1;
        return evolutionBaseCostStone * weaponTier;
    }

    public void OpenEvolutionPanel()
    {
        if (currentNode.nextPossibleEvolutions == null || currentNode.nextPossibleEvolutions.Length == 0)
        {
            ShowFloatingTextAt("<color=orange>최종 진화 단계입니다!</color>", new Vector2(Screen.width / 2, Screen.height / 2), Color.red);
            return;
        }

        if (enhancementLevel < 10)
        {
            ShowFloatingTextAt($"<color=orange>강화 레벨 10이 필요합니다! (현재: {enhancementLevel})</color>", new Vector2(Screen.width / 2, Screen.height / 2), Color.red);
        }

        ShowEvolutionChoice();
    }

    public void TryDismantle()
    {
        // 무한 파밍 방지: 이미 초기 무기이고 강화도 안 되어 있다면 실행 불가
        if (currentNode != null && currentNode.tier <= 1 && enhancementLevel == 0)
        {
            ShowFloatingTextAt("<color=yellow>이미 초기 상태입니다!</color>", new Vector2(Screen.width / 2, Screen.height / 2), Color.yellow);
            return;
        }

        // 분해 시 강화석 지급 (밸런스 조정: 티어 배율 적용)
        // 공식: (기본 15 + 현재 강화 수치) * 무기 티어
        int weaponTier = (currentNode != null) ? currentNode.tier : 1;
        int returnStones = (15 + enhancementLevel) * weaponTier;
        
        ResourceManager.Instance.AddEnhancementStones(returnStones);

        // 초기화
        currentNode = Resources.Load<EvolutionNode>("Stick_00_Root");
        enhancementLevel = 0;

        UpdateWeaponUI();
        ShowFloatingTextAt($"<color=green>무기 분해! ({weaponTier}티어 보너스) 강화석 +{returnStones}</color>", new Vector2(Screen.width / 2, Screen.height / 2), Color.green);
    }

    public void BuyEnhance()
    {
        TryEnhance();
    }

    private void ShowEvolutionChoice()
    {
        if (evolutionChoicePanel == null) return;
        evolutionChoicePanel.SetActive(true);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentNode.nextPossibleEvolutions.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                var nextNode = currentNode.nextPossibleEvolutions[i];
                if (choiceButtonTexts.Length > i) choiceButtonTexts[i].text = nextNode.stageName;
                if (choicePreviewImages.Length > i) choicePreviewImages[i].sprite = nextNode.stageSprite;
                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => FinalizeEvolution(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void FinalizeEvolution(int index)
    {
        int stoneCost = GetEvolutionStoneCost();
        if (ResourceManager.Instance.SpendEnhancementStones(stoneCost))
        {
            if (index < currentNode.nextPossibleEvolutions.Length)
            {
                currentNode = currentNode.nextPossibleEvolutions[index];
                enhancementLevel = 0;
                if (evolutionChoicePanel != null) evolutionChoicePanel.SetActive(false);
                UpdateWeaponUI();
                ShowFloatingTextAt($"<size=150%>{currentNode.stageName} 진화!</size>", new Vector2(Screen.width / 2, Screen.height / 2), Color.white);
            }
        }
        else
        {
            ShowFloatingTextAt("<color=red>강화석 부족!</color>", new Vector2(Screen.width / 2, Screen.height / 2), Color.red);
        }
    }

    private void UpdateWeaponUI()
    {
        if (weaponNameText != null)
            weaponNameText.text = $"{currentNode.stageName} +{enhancementLevel}";

        if (weaponImage != null && currentNode.stageSprite != null)
            weaponImage.sprite = currentNode.stageSprite;

        // 강화 비용 UI 업데이트
        if (enhanceCostText != null)
            enhanceCostText.text = $"강화 비용: {GetUpgradeGoldCost():N0}G";

        // 진화 비용 UI 업데이트
        if (evolutionCostText != null)
            evolutionCostText.text = $"진화 비용: {GetEvolutionStoneCost():N0}개";
    }

    private void UpdateResourceUI(long gold)
    {
        if (goldText != null) goldText.text = $"골드: {gold:N0}";
        if (stoneText != null && ResourceManager.Instance != null)
            stoneText.text = $"강화석: {ResourceManager.Instance.EnhancementStones:N0}";
    }

    private void ShowFloatingTextAt(string content, Vector2 screenPos, Color? color = null)
    {
        if (floatingTextPrefab == null || uiCanvas == null) return;

        GameObject go = Instantiate(floatingTextPrefab, uiCanvas.transform);
        go.transform.position = screenPos; 
        
        FloatingText ft = go.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.SetText(content);
            if (color.HasValue) go.GetComponent<TextMeshProUGUI>().color = color.Value;
        }
    }

    private System.Collections.IEnumerator PunchScale(Transform t)
    {
        Vector3 baseScale = Vector3.one;
        t.localScale = baseScale * 1.15f;
        yield return new WaitForSeconds(0.05f);
        t.localScale = baseScale;
    }
}