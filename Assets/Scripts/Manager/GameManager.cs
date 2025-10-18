using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private CarController _carController;
    private CameraMove _cameraMove;
    void Awake()
    {
        Application.targetFrameRate = 100;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _carController = FindAnyObjectByType<CarController>();
        _cameraMove = FindAnyObjectByType<CameraMove>();
    }

    // Update is called once per frame
    void Update()
    {
        _carController.ControllInput();
    }
    private void LateUpdate()
    {
        _cameraMove.CameraChase();
    }
    private void FixedUpdate()
    {
        _carController.CarControll();
    }
}
