using UnityEngine;

namespace Abilities
{
    public class AbilityLookup : MonoBehaviour
    {
        [SerializeField] private AbilityInfo ArcherAbility;
        [SerializeField] private AbilityInfo BossAbility;
        [SerializeField] private AbilityInfo SturdyAbility;

        private static AbilityLookup _instance;

        public void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public AbilityInfo Find(Ability ability)
        {
            return ability switch
            {
                Ability.Archer => ArcherAbility,
                Ability.Boss   => BossAbility,
                Ability.Sturdy => SturdyAbility,
                _              => null,
            };
        }
    }
}
