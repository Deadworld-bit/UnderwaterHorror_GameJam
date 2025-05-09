using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Settings")]
    [SerializeField] private float _flipRotationTime = 0.5f;

    private PlayerController _playerController;
    private bool _isFacingRight;
    private Coroutine _turnCoroutine;

    private void Awake()
    {
        _playerController = _playerTransform.GetComponent<PlayerController>();
        _isFacingRight = _playerController.IsFacingRight;
    }

    private void Update()
    {
        transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation =0f;

        float elapsedTime =0f;
        while (elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.LerpAngle(startRotation, endRotationAmount, elapsedTime / _flipRotationTime);
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        if (_isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
