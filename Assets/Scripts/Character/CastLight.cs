using UnityEngine;
using System.Collections;

public class CastLight : MonoBehaviour
{
    [Header("Light Ball Settings")]
    [SerializeField] private GameObject lightBallPrefab;
    [SerializeField] private float headHeight = 1.5f;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float cancelAnimationDuration = 2f;

    private Animator _animator;
    private PlayerController _playerController;
    private GameObject lightBallInstance;
    private bool isLightBallActive = false;
    private Coroutine followCoroutine;
    private Coroutine cancelCoroutine;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();

        if (_animator == null || _playerController == null || lightBallPrefab == null)
        {
            Debug.LogError("SpellCasting: Missing required components or prefab assignment.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!isLightBallActive && _playerController.CanCastSpell())
            {
                _animator.SetTrigger("CastSpell");
            }
            else if (isLightBallActive && cancelCoroutine == null)
            {
                cancelCoroutine = StartCoroutine(CancelSpell());
            }
        }
    }

    public void CreateLightBall()
    {
        if (lightBallInstance == null)
        {
            Transform rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            if (rightHand == null)
            {
                Debug.LogError("SpellCasting: Could not find right hand bone.");
                return;
            }

            lightBallInstance = Instantiate(lightBallPrefab, rightHand.position, Quaternion.identity);
            Vector3 targetPosition = transform.position + Vector3.up * headHeight;

            followCoroutine = StartCoroutine(MoveAndFollowLightBall(lightBallInstance, targetPosition, 0.5f));
            isLightBallActive = true;
        }
    }

    private IEnumerator CancelSpell()
    {
        _animator.SetTrigger("CancelSpell");

        yield return new WaitForSeconds(cancelAnimationDuration);

        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }

        Destroy(lightBallInstance);
        lightBallInstance = null;
        isLightBallActive = false;
        cancelCoroutine = null;
    }

    private IEnumerator MoveAndFollowLightBall(GameObject lightBall, Vector3 initialTargetWorldPosition, float initialDuration)
    {
        if (lightBall == null) yield break;

        Vector3 startPosition = lightBall.transform.position;
        float time = 0f;

        while (time < initialDuration && lightBall != null)
        {
            time += Time.deltaTime;
            lightBall.transform.position = Vector3.Lerp(startPosition, initialTargetWorldPosition, time / initialDuration);
            yield return null;
        }

        if (lightBall == null) yield break;

        while (lightBall != null)
        {
            Vector3 targetPosition = transform.position + Vector3.up * headHeight;
            lightBall.transform.position = Vector3.Lerp(lightBall.transform.position, targetPosition, followSpeed * Time.deltaTime);
            yield return null;
        }
    }
}