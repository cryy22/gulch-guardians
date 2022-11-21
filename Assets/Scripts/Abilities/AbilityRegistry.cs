using Crysc.Registries;
using UnityEngine;

namespace GulchGuardians.Abilities
{
    [CreateAssetMenu(fileName = "AbilityRegistry", menuName = "Registries/Ability")]
    public class AbilityRegistry : Registry<AbilityType>
    { }
}
