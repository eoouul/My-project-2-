using UnityEngine;
using System;

namespace StickEvolution
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [SerializeField] private long _gold = 0;
        [SerializeField] private int _enhancementStones = 0;

        public event Action<long> OnGoldChanged;
        public event Action<int> OnEnhancementStonesChanged;

        public long Gold => _gold;
        public int EnhancementStones => _enhancementStones;

        private void Awake()
        {
            Instance = this;
        }

        public void AddGold(long amount)
        {
            _gold += amount;
            OnGoldChanged?.Invoke(_gold);
        }

        public bool SpendGold(long amount)
        {
            if (_gold >= amount)
            {
                _gold -= amount;
                OnGoldChanged?.Invoke(_gold);
                return true;
            }
            return false;
        }

        public void AddEnhancementStones(int amount)
        {
            _enhancementStones += amount;
            OnEnhancementStonesChanged?.Invoke(_enhancementStones);
        }

        public bool SpendEnhancementStones(int amount)
        {
            if (_enhancementStones >= amount)
            {
                _enhancementStones -= amount;
                OnEnhancementStonesChanged?.Invoke(_enhancementStones);
                return true;
            }
            return false;
        }
    }
}
