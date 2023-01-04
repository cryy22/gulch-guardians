using UnityEngine;

namespace GulchGuardians.Abilities
{
    [CreateAssetMenu(fileName = "AbilityIndex", menuName = "Indexes/Ability Index")]
    public class AbilityIndex : ScriptableObject
    {
        [SerializeField] private AbilityType ArcherType;
        [SerializeField] private AbilityType BossType;
        [SerializeField] private AbilityType EvasiveType;
        [SerializeField] private AbilityType SpikyType;
        [SerializeField] private AbilityType SturdyType;
        [SerializeField] private AbilityType ToughType;
        [SerializeField] private AbilityType TrapperType;

        public AbilityType Archer => ArcherType;
        public AbilityType Boss => BossType;
        public AbilityType Evasive => EvasiveType;
        public AbilityType Spiky => SpikyType;
        public AbilityType Sturdy => SturdyType;
        public AbilityType Tough => ToughType;
        public AbilityType Trapper => TrapperType;
    }
}
