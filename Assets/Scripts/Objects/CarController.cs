using UnityEngine;
/// <summary>
/// 車の挙動
/// </summary>
public class CarController : MonoBehaviour
{
    [Header("基本設定")]
    [Tooltip("車の重心"),SerializeField] private Transform _centerOfMass;
    [Tooltip("タイヤ"),SerializeField] private WheelCollider[] _wheels;
    [Tooltip("パワー"), SerializeField] private float _accelPower = 1000f;
    [Tooltip("最大ハンドル角度"), SerializeField] private float _handleAngle = 30f;
    [Tooltip("ブレーキ力"), SerializeField] private float _brakePower = 1000f;
    [Tooltip("ステア変化スピード"), SerializeField] private float _steerLerpSpeed = 5f;
    [Tooltip("ドリフトステア乗数"), SerializeField] private float _driftSteerAngle = 1.8f;
    [Tooltip("ドリフト摩擦係数"), SerializeField] private float _driftFrictionFactor = 0.6f;
    private Transform[] _visuals;//タイヤの見た目
    private Rigidbody _rb;
    private Vector3 _pos;
    private Quaternion _dir;
    private WheelFrictionCurve _side;
    private bool _isDrifting;
    private float _brakeInput,_accelInput,_steerInput,_currentSteer,_steerAngle;
    private float[] _driveWheels = new float[] { 0f, 0f, 1.0f, 1.0f };
    private float[] _steerWheels = new float[] { 1.0f, 1.0f, 0f, 0f };
    private void Awake()
    {
        _wheels = GetComponentsInChildren<WheelCollider>();
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centerOfMass.localPosition;
        _visuals = new Transform[_wheels.Length];
        for(int i = 0; i < _wheels.Length; i++)
        {
            _visuals[i] = _wheels[i].transform.GetChild(0);
        }
    }

    /// <summary>
    /// コントロール時の入力
    /// </summary>
    public void ControllInput()
    {
        //基本入力
        _steerInput = Input.GetAxis("Horizontal");
        _accelInput = Input.GetAxis("Vertical");
        _brakeInput = Input.GetKey(KeyCode.Space) ? _brakePower : 0;
        _isDrifting = Input.GetKey(KeyCode.RightShift);

        _currentSteer = Mathf.Lerp(_currentSteer, _steerInput, Time.deltaTime * _steerLerpSpeed);
    }
    /// <summary>
    /// 車の挙動演算
    /// </summary>
    public void CarControll()
    {
        _steerAngle = Mathf.Lerp(_handleAngle,10f, _rb.linearVelocity.magnitude / 40f);

        //ドリフト中の処理
        if (_isDrifting)
        {
            _steerAngle *= _driftSteerAngle;
            AdjustWheelFriction(_driftFrictionFactor);
        }
        else
        {
            AdjustWheelFriction(1f);
        }

        for (int i = 0; i < _wheels.Length; i++)
        {
            _wheels[i].motorTorque = _accelInput * _driveWheels[i] * _accelPower;
            _wheels[i].steerAngle = _currentSteer * _steerWheels[i] * _handleAngle;
            _wheels[i].brakeTorque = _brakeInput;
            _wheels[i].GetWorldPose(out _pos, out _dir);
            _visuals[i] .position = _pos;
            _visuals[i].rotation = _dir;
        }
    }
    /// <summary>
    /// 横摩擦の変更
    /// </summary>
    private void AdjustWheelFriction(float factor)
    {
        foreach(var wheel in _wheels)
        {
            _side = wheel.sidewaysFriction;
            _side.stiffness = Mathf.Lerp(_side.stiffness, factor, Time.deltaTime * 5f);
            wheel.sidewaysFriction = _side;
        }
    }
}
