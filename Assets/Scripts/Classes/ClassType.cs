using UnityEngine;

namespace GulchGuardians.Classes
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Types/Class")]
    public class ClassType : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField]
        [field: TextArea]
        public string Description { get; private set; }

        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
