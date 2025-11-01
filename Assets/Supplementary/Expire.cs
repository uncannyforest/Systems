using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expire : MonoBehaviour {
    public float time = 60;

    void Start() {
        Invoke("Despawn", time);
    }

    private void Despawn() {
        GameObject.Destroy(gameObject);
    }
}
