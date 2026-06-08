using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [System.Serializable]
    public class MaterialSlot
    {
        public MaterialData data;
        public int count;
    }

    public List<MaterialSlot> materials = new List<MaterialSlot>();
    public TextMeshProUGUI inventoryDisplayText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddMaterial(MaterialData data, int amount)
    {
        var slot = materials.Find(s => s.data == data);
        if (slot != null)
        {
            slot.count += amount;
        }
        else
        {
            materials.Add(new MaterialSlot { data = data, count = amount });
        }
        UpdateUI();
    }

    public bool HasMaterial(MaterialData data, int amount)
    {
        var slot = materials.Find(s => s.data == data);
        return slot != null && slot.count >= amount;
    }

    public void RemoveMaterial(MaterialData data, int amount)
    {
        var slot = materials.Find(s => s.data == data);
        if (slot != null)
        {
            slot.count -= amount;
            if (slot.count < 0) slot.count = 0;
            UpdateUI();
        }
    }

    public int GetCount(MaterialData data)
    {
        var slot = materials.Find(s => s.data == data);
        return slot != null ? slot.count : 0;
    }

    public void UpdateUI()
    {
        if (inventoryDisplayText != null)
        {
            string text = "Inventory:\n";
            foreach (var slot in materials)
            {
                if (slot.count > 0)
                {
                    text += $"{slot.data.materialName}: {slot.count}\n";
                }
            }
            inventoryDisplayText.text = text;
        }
    }
}
