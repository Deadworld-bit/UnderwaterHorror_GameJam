using UnityEngine;

public class PushObject : MonoBehaviour
{
    [SerializeField] private float _rayLength = 0.5f;
    [SerializeField] private LayerMask _pushableLayer;
    [SerializeField] private Vector3 _rayOffset = new Vector3(0f, -0.5f, 0f);

    private PlayerController _playerController;
    private bool _isPushing;
    private GameObject _pushingObject;

    public bool IsPushing => _isPushing;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        CheckPushing();
    }

    void FixedUpdate()
    {
        if (_isPushing && _pushingObject != null)
        {
            Rigidbody objRb = _pushingObject.GetComponent<Rigidbody>();
            if (objRb != null)
            {
                Rigidbody playerRb = _playerController.GetRigidbody();
                objRb.velocity = new Vector3(playerRb.velocity.x, objRb.velocity.y, 0f);
            }
        }
    }

    private void CheckPushing()
    {
        float horizontalInput = _playerController.GetHorizontalInput();
        if (Mathf.Abs(horizontalInput) > 0.1f) 
        {
            Vector3 rayDirection = horizontalInput > 0 ? Vector3.right : Vector3.left;
            Vector3 rayOrigin = transform.position + _rayOffset;
            Debug.DrawRay(rayOrigin, rayDirection * _rayLength, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, _rayLength, _pushableLayer))
            {
                if (((1 << hit.collider.gameObject.layer) & _pushableLayer) != 0)
                {
                    if (hit.collider.GetComponent<Rigidbody>() != null)
                    {
                        _isPushing = true;
                        _pushingObject = hit.collider.gameObject;
                    }
                }
                else if (hit.collider.CompareTag("Player"))
                {
                    _isPushing = true;
                    _pushingObject = hit.collider.gameObject;
                }
                else
                {
                    _isPushing = false;
                    _pushingObject = null;
                }
            }
            else
            {
                _isPushing = false;
                _pushingObject = null;
            }
        }
        else
        {
            _isPushing = false;
            _pushingObject = null;
        }
    }
}