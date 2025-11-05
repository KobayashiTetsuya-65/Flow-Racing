using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayableDirector playableDirector;
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
        if (Input.GetMouseButtonDown(0))
        {
            playableDirector.Play();
        }
    }
    private void LateUpdate()
    {
        _cameraMove.CameraChase();
    }
    private void FixedUpdate()
    {
        _carController.CheckGround();
        _carController.Move();
        _carController.Rotate();
        _carController.StickToGround();
        _carController.UpdateVisuals();
    }
}
