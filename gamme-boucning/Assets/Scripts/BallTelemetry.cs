using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public sealed class BallTelemetry : MonoBehaviour
{
    public int Bounces { get; private set; }
    public Rigidbody Body { get; private set; }
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground Plane") Bounces++;
    }

    public void ResetBall()
    {
        Body.position = initialPosition;
        Body.rotation = initialRotation;
        Body.linearVelocity = Vector3.zero;
        Body.angularVelocity = Vector3.zero;
        Body.WakeUp();
        Bounces = 0;
    }
}
