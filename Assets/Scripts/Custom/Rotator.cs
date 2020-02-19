using Utils;
using UnityEngine;
using PathCreation;
using System.Collections;

public class Rotator : MonoBehaviour {
    public enum RotationSpace {Local, World, Transform};

    #region editor
    [SerializeField] private EndOfPathInstruction endInstruction = EndOfPathInstruction.Loop;
    [SerializeField] private RotationSpace space = RotationSpace.Local;
    [SerializeField] private bool lookAt = default;
    [SerializeField] private Transform rotateAround = default;
    [SerializeField] private IgnoredAxis ignoredAxis = IgnoredAxis.up;
    [SerializeField] private RandomizedFloat maxAngle = 360f;
    [SerializeField] private RandomizedFloat forwardSpeed = 10f;
    [SerializeField] private RandomizedFloat backwardSpeed = 10f;
    [SerializeField] private RandomizedFloat startDelay = default;
    [SerializeField] private RandomizedFloat stopDelay = default;
    [SerializeField] private bool destroyOnStop = default;
    [SerializeField] private bool unscaledDeltaTime = default;
    #endregion

    private int lapsPassed = 0;
    private bool canRotate = true;
    private float totalAngle = 0f;
    private Vector3 startAngle = Vector3.zero;
    private Vector3 targetAngle = Vector3.zero;

    private int currentLap => Mathf.FloorToInt(Mathf.Abs(totalAngle / maxAngle.value));

    #region private
    private void Awake() {
        if(lookAt) transform.LookAt(rotateAround, Vector3.up);

        startAngle = transform.rotation.eulerAngles;
        targetAngle = startAngle + (ignoredAxis.axis * maxAngle.value);

        if (startDelay.value > 0f) StartCoroutine(_delay(startDelay.value));
    }

    void Update() {
        if (!canRotate) return;

        float speed = forwardSpeed.value;
        float deltaTime = unscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (endInstruction == EndOfPathInstruction.Reverse && lapsPassed % 2 == 1) speed = -backwardSpeed.value;

        speed *= Mathf.Sign(maxAngle.value);
        float angle = speed * deltaTime;
        totalAngle += Mathf.Abs(angle);

        if(space == RotationSpace.Local || space == RotationSpace.World) transform.Rotate(ignoredAxis.axis * angle, (Space)space);
        else if(rotateAround != null) {
            transform.RotateAround(rotateAround.position, ignoredAxis.axis, angle);
            if(lookAt) transform.LookAt(rotateAround, Vector3.up);
        }

        if (lapsPassed < currentLap) {
            lapsPassed = currentLap;

            // fixing delta issues
            if(endInstruction == EndOfPathInstruction.Reverse) transform.rotation = Quaternion.Euler(lapsPassed % 2 == 1 ? targetAngle : startAngle);
            else if(endInstruction == EndOfPathInstruction.Stop) transform.rotation = Quaternion.Euler(startAngle);

            if(destroyOnStop && lapsPassed > 0) Destroy(gameObject);
            else if (stopDelay.value > 0f) StartCoroutine(_delay(stopDelay.value));
        }
    }

    private IEnumerator _delay(float duration) {
        canRotate = false;
        float elapsed = 0f;
        while ((elapsed += Time.fixedDeltaTime) < duration) yield return null;
        canRotate = true;
    }
    #endregion
}