using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private CarController _carController;
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
    }

    // Update is called once per frame
    void Update()
    {
        _carController.ControllInput();
        _carController.CarControll();
    }
}
