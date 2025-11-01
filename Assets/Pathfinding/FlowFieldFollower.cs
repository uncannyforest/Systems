using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlowFieldFollower : MonoBehaviour {
    public GameObject target;
    public float speed = 1;

    new private Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        float angle = FlowFieldRegistry.Get(target.transform, transform.position);
        rigidbody.velocity = Quaternion.Euler(0, -angle, 0) * Vector3.right * speed; // -angle bc Unity clockwise
    }
}
