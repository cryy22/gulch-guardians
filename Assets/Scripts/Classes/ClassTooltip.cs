using Crysc.UI;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Classes
{
    public class ClassTooltip : Tooltip<ClassType>
    {
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text DescriptionText;

        protected override void ShowTooltip(ClassType target)
        {
            if (target == null) return;

            base.ShowTooltip(target);
            SetClass(target);
        }

        private void SetClass(ClassType @class)
        {
            TitleText.text = @class.Name;
            DescriptionText.text = @class.Description;
        }
    }
}
