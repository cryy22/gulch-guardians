using UnityEngine;

namespace GulchGuardians.Classes
{
    [CreateAssetMenu(fileName = "ClassIndex", menuName = "Indexes/Class Index")]
    public class ClassIndex : ScriptableObject
    {
        [field: SerializeField] public ClassType Rookie { get; private set; }
        [field: SerializeField] public ClassType Healer { get; private set; }
        [field: SerializeField] public ClassType Saguaro { get; private set; }
    }
}
