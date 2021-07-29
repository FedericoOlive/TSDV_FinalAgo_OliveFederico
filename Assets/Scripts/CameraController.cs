using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform[] positions;

    public enum CameraMode
    {
        Global,
        TopDown,
        ThirdPerson,
        FirstPerson,
        TopDownNear
    }

    public CameraMode cameraMode = CameraMode.ThirdPerson;
    private CameraMode lastCameraMode = CameraMode.ThirdPerson;

    private Vector3 offset;
    [SerializeField] private float angle;
    [SerializeField] private float distance;
    [SerializeField] private float height;
    private float direction;

    void Start()
    {

    }

    void Update()
    {
        UpdateCameraPosition();
    }

    void SetCameraMode()
    {
        switch (cameraMode)
        {
            case CameraMode.Global:
            case CameraMode.TopDown:
                transform.position = positions[(int) cameraMode].position;
                transform.rotation = positions[(int) cameraMode].rotation;
                break;
            case CameraMode.ThirdPerson:
                UpdateCameraPosition();
                break;
            case CameraMode.FirstPerson:
                UpdateCameraPosition();
                break;
            default:
                break;
        }

        lastCameraMode = cameraMode;
    }

    void UpdateCameraPosition()
    {
        switch (cameraMode)
        {
            case CameraMode.FirstPerson:
            case CameraMode.TopDownNear:
            case CameraMode.Global:
            case CameraMode.TopDown:
                transform.position = positions[(int)cameraMode].position;
                transform.rotation = positions[(int)cameraMode].rotation;
                break;
            case CameraMode.ThirdPerson:
                // Utilizo las configuraciones del usuario:
                Vector3 target = positions[(int)CameraMode.ThirdPerson].position;
                target.y += height;
                positions[(int)CameraMode.ThirdPerson].TransformDirection(target);
                transform.position = new Vector3(target.x, target.y + angle, target.z - distance);
                transform.LookAt(target);

                // Seteo la cámara detras del tanque
                direction = positions[(int)CameraMode.ThirdPerson].rotation.eulerAngles.y;
                transform.RotateAround(target, Vector3.up, direction);
                break;
            default:
                break;
        }
    }
}