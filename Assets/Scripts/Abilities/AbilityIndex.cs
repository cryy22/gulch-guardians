using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GulchGuardians.Abilities
{
    [CreateAssetMenu(fileName = "AbilityIndex", menuName = "Indexes/Ability Index")]
    public class AbilityIndex : ScriptableObject
    {
        [SerializeField] private AbilityType ArcherType;
        [SerializeField] private AbilityType BossType;
        [SerializeField] private AbilityType EvasiveType;
        [SerializeField] private AbilityType SturdyType;
        [SerializeField] private AbilityType ToughType;
        [SerializeField] private AbilityType TrapperType;

        private static AbilityIndex _instance;

        public static AbilityIndex I
        {
            get
            {
                if (_instance == null)
                    _instance = Addressables.LoadAssetAsync<AbilityIndex>("AbilityIndex").WaitForCompletion();
                return _instance;
            }
        }

        public AbilityType Archer => ArcherType;
        public AbilityType Boss => BossType;
        public AbilityType Evasive => EvasiveType;
        public AbilityType Sturdy => SturdyType;
        public AbilityType Tough => ToughType;
        public AbilityType Trapper => TrapperType;
    }
}
