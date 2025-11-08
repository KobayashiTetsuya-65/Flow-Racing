using UnityEngine;

/// <summary>
/// 車の挙動
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("基本設定")]
    [Tooltip("車のモデル"),SerializeField] private Transform _carModel;
    [Tooltip("地面を検出する位置"), SerializeField] private Transform _groundCheck;

    [Header("挙動パラメータ")]
    [SerializeField, Tooltip("最高速度")] private float _maxSpeed = 50f;
    [SerializeField, Tooltip("加速力")] private float _acceleration = 100f;
    [SerializeField, Tooltip("旋回スピード")] private float _turnSpeed = 5f;
    [SerializeField, Tooltip("ドリフト時の旋回倍率")] private float _driftTurnMultiplier = 1.5f;
    [SerializeField, Tooltip("地面への吸い付き力")] private float _groundStickForce = 30f;
    [SerializeField, Tooltip("車体の傾き")] private float _tiltAngle = 15f;

    [Header("ブレーキ設定")]
    [SerializeField, Tooltip("ドリフト時の減速率（0.9=少し減速, 0.5=かなり減速）")]
    private float _driftBrakeFactor = 0.85f;
    [SerializeField, Tooltip("ブレーキ時の減速率")]
    private float _brakeFactor = 0.7f;

    [Header("物理設定")]
    [SerializeField, Tooltip("接地判定間隔")] private float _groundCheckInterval = 0.1f;
    [SerializeField, Tooltip("地面判定距離")] private float _groundRayLength = 1.2f;
    [SerializeField, Tooltip("地面判定のLayer")] private LayerMask _groundLayer;

    private Rigidbody _rb;
    private Transform _tr;
    private bool _isGrounded, _isDrifting, _isBraking;
    private float _steerInput, _accelInput, _currentSpeed, _speedFactor, _turn, _turnAdjusted, _targetTilt,_groundCheckTimer = 0;
    private Vector3 _groundNormal;
    private Quaternion _align, _targetRot;
    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = Vector3.down * 0.3f; // 少し低めに重心を設定
        _rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    /// <summary>
    /// 入力処理
    /// </summary>
    public void ControllInput()
    {
        _steerInput = Input.GetAxis("Horizontal");
        _accelInput = Input.GetAxis("Vertical");
        _isDrifting = Input.GetKey(KeyCode.Space);
        _isBraking = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
    /// <summary>
    /// 接地確認処理
    /// </summary>
    public void CheckGround()
    {
        _groundCheckTimer += Time.deltaTime;
        if(_groundCheckInterval <= _groundCheckTimer)
        {
            _isGrounded = Physics.CheckSphere(_groundCheck.position, 0.8f, _groundLayer);
            if (_isGrounded)
            {
                Physics.Raycast(_groundCheck.position, -_tr.up, out RaycastHit hit, _groundRayLength, _groundLayer);
                _groundNormal = hit.normal;
            }
            else
            {
                _groundNormal = Vector3.up;
            }
            _groundCheckTimer = 0;
        }
    }
    /// <summary>
    /// 前進・減速の処理
    /// </summary>
    public void Move()
    {
        if (!_isGrounded) return;

        _currentSpeed = Vector3.Dot(_rb.linearVelocity, _tr.forward);

        if (_isBraking)
        {
            _rb.linearVelocity *= _brakeFactor;
        }
        // ドリフト中は速度を少し落とす
        if (_isDrifting)
        {
            _rb.linearVelocity *= _driftBrakeFactor;
        }

        // 前進力を計算
        if (Mathf.Abs(_currentSpeed) < _maxSpeed)
        {
            _rb.AddForce(_tr.forward * _accelInput * _acceleration, ForceMode.Acceleration);
        }

        // 空気抵抗
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, _rb.linearVelocity * 0.98f, Time.fixedDeltaTime);
    }
    /// <summary>
    /// 旋回・ドリフト処理
    /// </summary>
    public void Rotate()
    {
        if (!_isGrounded) return;

        _speedFactor = Mathf.Clamp01(_rb.linearVelocity.magnitude / _maxSpeed);
        _turn = _steerInput * _turnSpeed * (_isDrifting ? _driftTurnMultiplier : 1f);

        // スピードに応じて旋回量を補正
        _turnAdjusted = _turn * Mathf.Lerp(0.5f, 1f, 1f - _speedFactor * 0.7f);
        _tr.Rotate(Vector3.up, _turnAdjusted * Time.fixedDeltaTime * 50f);
    }
    /// <summary>
    /// 地面に吸い付ける処理
    /// </summary>
    public void StickToGround()
    {
        if (_isGrounded)
        {
            // 地面の角度に合わせる
            _align = Quaternion.FromToRotation(_tr.up, _groundNormal) * _tr.rotation;
            _tr.rotation = Quaternion.Lerp(_tr.rotation, _align, Time.fixedDeltaTime * 10f);

            _rb.AddForce(-_groundNormal * _groundStickForce, ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
        }
    }
    /// <summary>
    /// 車体の傾き
    /// </summary>
    public void UpdateVisuals()
    {
        if (_carModel == null) return;

        _targetTilt = -_steerInput * _tiltAngle * (_isDrifting ? 1.3f : 1f);
        _targetRot = Quaternion.Euler(_carModel.localEulerAngles.x, _carModel.localEulerAngles.y, _targetTilt);
        _carModel.localRotation = Quaternion.Lerp(_carModel.localRotation, _targetRot, Time.deltaTime * 5f);
    }
}
