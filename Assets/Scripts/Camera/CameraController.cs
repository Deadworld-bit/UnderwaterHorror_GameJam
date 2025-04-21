using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private CinemachineVirtualCamera[] _virtualCameras;

    [Header("Control for lerping the Y Damping during player jump/ fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;
    private float _normYPanAmount;
    private Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _virtualCameras.Length; i++)
        {
            if (_virtualCameras[i].enabled)
            {
                _currentCamera = _virtualCameras[i];
                //set the framing transposer to the current camera
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
        //set the YDamping to the current camera's framing transposer
        _normYPanAmount = _framingTransposer.m_YDamping;

        //set the starting position of the tracked object offset
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }

    #region Lerp the Y Damping
    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //grab the starting damping amount
        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        //determine the end damping amount
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
            LerpedFromPlayerFalling = false;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallYPanTime);
            _framingTransposer.m_YDamping = lerpedPanAmount;
            yield return null;
        }
        IsLerpingYDamping = false;
    }
    #endregion

    #region Pan Camera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPosition)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPosition));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPosition)
    {
        Vector2 endPosition = Vector2.zero;
        Vector2 startPosition = Vector2.zero;

        //handle pan from trigger
        if(!panToStartingPosition){
            switch(panDirection){
                case PanDirection.Left:
                    endPosition = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPosition = Vector2.right;
                    break;
                case PanDirection.Up:
                    endPosition = Vector3.up;
                    break;
                case PanDirection.Down:
                    endPosition = Vector3.down;
                    break;
            }

            endPosition *= panDistance;
            startPosition = _startingTrackedObjectOffset;
            endPosition += startPosition;
        }
        //handle pan to starting position
        else{
            endPosition = _startingTrackedObjectOffset;
            startPosition = _framingTransposer.m_TrackedObjectOffset;
        }

        //handel the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 lerpedPanAmount = Vector3.Lerp(startPosition, endPosition, elapsedTime / panTime);
            _framingTransposer.m_TrackedObjectOffset = lerpedPanAmount;
            yield return null;
        }
    }
    #endregion
}
