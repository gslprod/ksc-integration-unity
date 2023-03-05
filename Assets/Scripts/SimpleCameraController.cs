using UIGroups.DialogBox;
using UnityEngine;

//not mine
public class SimpleCameraController : MonoBehaviour
{
    class CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

            x += rotatedTranslation.x;
            y += rotatedTranslation.y;
            z += rotatedTranslation.z;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }
    }

    CameraState m_TargetCameraState = new CameraState();
    CameraState m_InterpolatingCameraState = new CameraState();

    [Header("Movement Settings")]
    [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
    public float boost = 3.5f;

    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation. ХРЕНЬ. НЕ ИСПОЛЬЗОВАТЬ")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

    public bool CameraRotationAllowed { get; private set; } = false;
    public bool MovingAllowed { get; private set; } = true;

    [SerializeField] private UserInput _input;
    [SerializeField] private UICore _uiCore;
    [SerializeField] private float mouseSensitivity;

    void OnEnable()
    {
        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);
    }

    void Update()
    {
        // Exit Sample  

        if (_input.IsEscapeDown())
        {
            DialogBoxButton[] dbButtons =
            {
                new DialogBoxButton("Закрыть", () =>
                {
                    Application.Quit();

                    #if UNITY_EDITOR

                    UnityEditor.EditorApplication.isPlaying = false;

                    #endif
                }, false, Color.red),
                new DialogBoxButton("Отмена", () =>{ }, true, new Color(0.6f, 0.6f, 0.6f))
            };

            DialogBoxElement.Factory.Create(_uiCore, "Закрыть приложение?", "Перед этим убедитесь, что вы сохранили все необходимое.", dbButtons);
        }

        if (_input.IsCameraRotationButtonDown())
            SetCameraRotationAvailability(!CameraRotationAllowed);

        // Rotation
        if (CameraRotationAllowed)
        {
            var mouseMovement = 5 /* * Time.deltaTime*/ * 0.005f * _input.GetInputLookRotation();
            if (invertY)
                mouseMovement.y = -mouseMovement.y;

            var mouseSensitivityFactor = mouseSensitivity /*mouseSensitivityCurve.Evaluate(mouseMovement.magnitude)*/;

            m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
            m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

            m_TargetCameraState.pitch = Mathf.Clamp(m_TargetCameraState.pitch + mouseMovement.y * mouseSensitivityFactor, -90, 90);
        }

        if (!MovingAllowed)
            return;
        // Translation
        var translation = _input.GetInputTranslationDirection() * Time.deltaTime;

        // Speed up movement when shift key held
        if (_input.IsBoostPressed())
        {
            translation *= 10.0f;
        }

        translation *= Mathf.Pow(2.0f, boost);

        m_TargetCameraState.Translate(translation);

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

        m_InterpolatingCameraState.UpdateTransform(transform);
    }

    private void SetCameraRotationAvailability(bool isAvailable)
    {
        if (CameraRotationAllowed == isAvailable)
            return;

        CameraRotationAllowed = isAvailable;
        Cursor.lockState = isAvailable ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetMovementAvailability(bool isAvailable)
    {
        if (MovingAllowed == isAvailable)
            return;

        MovingAllowed = isAvailable;
    }
}