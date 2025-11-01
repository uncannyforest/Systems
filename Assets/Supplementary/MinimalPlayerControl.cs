using UnityEngine;

public class MinimalPlayerControl : MonoBehaviour {
    public float speed = 1;

    // Update is called once per frame
    void Update() {
        if (Input.GetKey("d"))
            transform.position += Vector3.right * speed * Time.deltaTime;
        if (Input.GetKey("w"))
            transform.position += Vector3.forward * speed * Time.deltaTime;
        if (Input.GetKey("a"))
            transform.position += Vector3.left * speed * Time.deltaTime;
        if (Input.GetKey("s"))
            transform.position += Vector3.back * speed * Time.deltaTime;
    }
}
