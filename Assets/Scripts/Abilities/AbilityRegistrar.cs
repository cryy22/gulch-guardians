using Crysc.Registries;

namespace GulchGuardians.Abilities
{
    public class AbilityRegistrar : MouseEventRegistrar<AbilityType>
    {
        private IAbilityProvider _abilityProvider;

        public override AbilityType Registrant => _abilityProvider.Ability;

        protected override void Awake()
        {
            base.Awake();
            _abilityProvider = GetComponent<IAbilityProvider>();
        }
    }
}
