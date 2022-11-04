using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Unit = GulchGuardians.Unit;

[ExecuteAlways]
public class UnitListDisplayer : MonoBehaviour
{
    private const float _unitSpacing = 1.8825f;

    public bool IsInverted;

    private bool _isUpdateEnqueued;

    private void OnTransformChildrenChanged()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        
        if (_isUpdateEnqueued) return;

        StartCoroutine(UpdateUnitPositions());
        _isUpdateEnqueued = true;
    }

    private IEnumerator UpdateUnitPositions()
    {
        yield return new WaitForEndOfFrame();
        
        var positionedUnits = 0;
        foreach (Transform child in transform)
        {
            var unit = child.GetComponent<Unit>();
            if (unit == null) continue;
            
            unit.transform.localPosition = new Vector3(
                x: positionedUnits * _unitSpacing * (IsInverted ? -1 : 1),
                y: 0f,
                z: 0f
            );
            unit.transform.localScale = Vector3.one;
            unit.gameObject.SetActive(true);
            
            positionedUnits++;
            _isUpdateEnqueued = false;
        }
    }
}
