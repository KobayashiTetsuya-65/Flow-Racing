using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("車の重心"),SerializeField] private Transform _centerOfMass;
    [Header("タイヤ"),SerializeField] private WheelCollider[] _wheels;
    [Header("パワー"), SerializeField] private float _accelPower = 1000f;
    [Header("最大ハンドル角度"), SerializeField] private float _handleAngle = 45f;
    [Header("ブレーキ力"), SerializeField] private float _brakePower = 1000f;
    private Transform[] _obj;//タイヤの見た目
    private Rigidbody _rb;
    private Vector2 _inputVector;
    private Vector3 _pos;
    private Quaternion _dir;
    private float _brakeInput = 0;
    private float[] _driveWheels = new float[] { 0f, 0f, 1.0f, 1.0f };
    private float[] _steerWheels = new float[] { 1.0f, 1.0f, 0f, 0f };
    private void Awake()
    {
        _wheels = GetComponentsInChildren<WheelCollider>();
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centerOfMass.localPosition;
        _obj = new Transform[_wheels.Length];
        for(int i = 0; i < _wheels.Length; i++)
        {
            _obj[i] = _wheels[i].transform.GetChild(0);
        }
    }

    /// <summary>
    /// コントロール時の入力
    /// </summary>
    public void ControllInput()
    {
        _inputVector = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        _brakeInput = Input.GetKey(KeyCode.Space) ? _brakePower : 0;
    }
    /// <summary>
    /// 車の挙動
    /// </summary>
    public void CarControll()
    {
        for(int i = 0; i < _wheels.Length; i++)
        {
            _wheels[i].motorTorque = _inputVector.y * _driveWheels[i] * _accelPower;
            _wheels[i].steerAngle = _inputVector.x * _steerWheels[i] * _handleAngle;
            _wheels[i].brakeTorque = _brakeInput;
            _wheels[i].GetWorldPose(out _pos, out _dir);
            _obj[i] .position = _pos;
            _obj[i].rotation = _dir;
        }
    }
}
