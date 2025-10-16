using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("ターゲット"),SerializeField] private Transform _target;
    [Header("カメラ設定")]
    [SerializeField] private float _distance = 6f;
    [SerializeField] private float _height = 3f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    private Vector3 _targetPos;
    private Quaternion _targetRot;
    private Transform _tr;
    private void Awake()
    {
        _tr = GetComponent<Transform>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(_target == null) return;

        //カメラの座標計算
        _targetPos = _target.position - _target.forward * _distance + Vector3.up * _height;
        _tr.position = Vector3.Lerp(_tr.position, _targetPos, Time.deltaTime * _followSpeed);

        //カメラの向き計算
        _targetRot = Quaternion.LookRotation(_target.position - _tr.position, Vector3.up);
        _tr.rotation = Quaternion.Slerp(_tr.rotation,_targetRot,Time.deltaTime * _rotationSpeed);
    }
}
