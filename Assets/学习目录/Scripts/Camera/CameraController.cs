using UnityEngine;

public class Came : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] Transform _target;
    [Header("Randen")]
    [SerializeField] private Vector2 _minMaxXY;

    private void LateUpdate()
    {
        if (_target == null)
        {
            Debug.LogWarning("没有目标");
            return;
        }
        Vector3 targetPosition = _target.position;
        targetPosition.z = -10;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -_minMaxXY.x, _minMaxXY.x);
        targetPosition.y=Mathf.Clamp(targetPosition.y, -_minMaxXY.y, _minMaxXY.y);

        transform.position = targetPosition;
    }
}
