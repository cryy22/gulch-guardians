using System.Collections.Generic;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Scriptable Objects/Config/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public List<RuntimeAnimatorController> RuntimeAnimatorControllers;
        public bool IsBoss;
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;

        public RuntimeAnimatorController GetRandomAnimatorController()
        {
            return RuntimeAnimatorControllers[Random.Range(
                minInclusive: 0,
                maxExclusive: RuntimeAnimatorControllers.Count
            )];
        }
    }
}
