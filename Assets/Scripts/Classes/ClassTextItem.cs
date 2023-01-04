using TMPro;
using UnityEngine;

namespace GulchGuardians.Classes
{
    public class ClassTextItem : MonoBehaviour, IClassProvider
    {
        [SerializeField] private TMP_Text Text;

        public void SetClass(ClassType @class)
        {
            Class = @class;
            Text.text = @class.Name;
        }

        public ClassType Class { get; private set; }
    }
}
