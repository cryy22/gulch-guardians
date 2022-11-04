using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class UnitListDisplayer : MonoBehaviour
{
    private const float _unitSpacing = 2f;

    public bool IsInverted;

    private void OnTransformChildrenChanged()
    {
        var placedUnits = 0;
        foreach (Transform child in transform)
            EditorApplication.delayCall += () =>
            {
                if (child == null) return;
                child.localPosition = new Vector3(
                    x: placedUnits++ * _unitSpacing * (IsInverted ? -1 : 1),
                    y: 0f,
                    z: 0f
                );
            };
    }
}
