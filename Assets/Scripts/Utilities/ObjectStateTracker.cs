using UnityEngine;

public abstract class ObjectStateTracker : MonoBehaviour
{
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;
    protected bool initialActiveState;

    protected virtual void Start()
    {
        // Store the initial state of the GameObject
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialActiveState = gameObject.activeSelf;
    }

    // Abstract method to be implemented by derived classes
    public abstract void ResetToInitialState();
}
