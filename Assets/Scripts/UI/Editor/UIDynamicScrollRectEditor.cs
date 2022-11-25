using UnityEditor;
using UnityEditor.UI;

namespace GulchGuardians.UI.Editor
{
    [CustomEditor(inspectedType: typeof(UIDynamicScrollRect), editorForChildClasses: true)]
    public class UIDynamicScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty _maxHeight;

        protected override void OnEnable()
        {
            base.OnEnable();
            _maxHeight = serializedObject.FindProperty("MaxHeight");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_maxHeight);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
