using UnityEngine;
using UnityEngine.InputSystem; // 최신 인풋 시스템 사용
using StickEvolution;

public class CheatManager : MonoBehaviour
{
    void Update()
    {
        // 최신 인풋 시스템(Input System Package) 방식
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // F1: 골드 +1000
        if (keyboard.f1Key.wasPressedThisFrame)
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddGold(1000);
                Debug.Log("Cheat (New Input): Gold +1000 Added");
            }
        }

        // F2: 강화석 +1000
        if (keyboard.f2Key.wasPressedThisFrame)
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddEnhancementStones(1000);
                Debug.Log("Cheat (New Input): Stones +1000 Added");
            }
        }
    }
}
