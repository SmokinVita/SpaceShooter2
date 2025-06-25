using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _orgiginalPos;
    [SerializeField] private float _shakeDuration = .7f;
    private float _currentShakeDuration;
    [SerializeField] private float _shakeStrength = 2f;
    private bool _isShakeCameraTime = false;


    // Start is called before the first frame update
    void Start()
    {
        _orgiginalPos = transform.position;
        _currentShakeDuration = _shakeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShakeCameraTime)
        {
            if (_currentShakeDuration > 0f)
            {
                transform.position = _orgiginalPos + (Vector3)Random.insideUnitCircle * _shakeStrength * Time.deltaTime;
                _currentShakeDuration -= Time.deltaTime;
            }
            else
            {
                transform.position = _orgiginalPos;
                _isShakeCameraTime = false;
                _currentShakeDuration = _shakeDuration;
            }
        }
    }

    public void ShakeCamera()
    {
        _isShakeCameraTime = true;
    }
}
