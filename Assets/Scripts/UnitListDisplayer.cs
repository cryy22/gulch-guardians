using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class UnitListDisplayer : MonoBehaviour
{
    private const float _unitSpacing = 2f;

    private void OnTransformChildrenChanged()
    {
        var placedUnits = 0;
        foreach (Transform child in transform)
            EditorApplication.delayCall += () =>
            {
                if (child == null) return;
                child.localPosition = new Vector3(x: placedUnits++ * _unitSpacing, y: 0f, z: 0f);
            };
    }
}
