using UnityEngine;

namespace GulchGuardians.Abilities
{
    [CreateAssetMenu(fileName = "AbilityIndex", menuName = "Indexes/Ability Index")]
    public class AbilityIndex : ScriptableObject
    {
        [SerializeField] private AbilityType ArcherType;
        [SerializeField] private AbilityType BossType;
        [SerializeField] private AbilityType EvasiveType;
        [SerializeField] private AbilityType HealerType;
        [SerializeField] private AbilityType SpikyType;
        [SerializeField] private AbilityType SturdyType;

        public AbilityType Archer => ArcherType;
        public AbilityType Boss => BossType;
        public AbilityType Evasive => EvasiveType;
        public AbilityType Healer => HealerType;
        public AbilityType Spiky => SpikyType;
        public AbilityType Sturdy => SturdyType;
    }
}
