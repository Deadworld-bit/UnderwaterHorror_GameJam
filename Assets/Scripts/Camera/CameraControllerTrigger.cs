using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEditor;

public class CameraControllerTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                //pan the camera
                CameraController.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraController.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCamera = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera _cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera _cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection
{
    Left,
    Right,
    Up,
    Down
}

[CustomEditor(typeof(CameraControllerTrigger))]
public class MyScriptEditor : Editor
{
    CameraControllerTrigger cameraControllerTrigger;

    private void OnEnable()
    {
        cameraControllerTrigger = (CameraControllerTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (cameraControllerTrigger.customInspectorObjects.swapCamera)
        {
            cameraControllerTrigger.customInspectorObjects._cameraOnLeft = EditorGUILayout.ObjectField("Camera On Left", cameraControllerTrigger.customInspectorObjects._cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cameraControllerTrigger.customInspectorObjects._cameraOnRight = EditorGUILayout.ObjectField("Camera On Right", cameraControllerTrigger.customInspectorObjects._cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControllerTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControllerTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Pan Direction", cameraControllerTrigger.customInspectorObjects.panDirection);
            cameraControllerTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControllerTrigger.customInspectorObjects.panDistance);
            cameraControllerTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControllerTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControllerTrigger);
        }
    }
}
