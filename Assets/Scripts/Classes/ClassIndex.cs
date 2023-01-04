using UnityEngine;

namespace GulchGuardians.Classes
{
    [CreateAssetMenu(fileName = "ClassIndex", menuName = "Indexes/Class Index")]
    public class ClassIndex : ScriptableObject
    {
        private static ClassIndex _instance;

        public static ClassIndex I
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = Resources.Load<ClassIndex>("Indexes/ClassIndex");
                return _instance;
            }
        }

        [field: SerializeField] public ClassType Rookie { get; private set; }
        [field: SerializeField] public ClassType Healer { get; private set; }
        [field: SerializeField] public ClassType Saguaro { get; private set; }
    }
}
