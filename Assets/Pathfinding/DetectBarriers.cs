using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBarriers {
    private float y;

    public DetectBarriers(float y) {
        this.y = y;
    }

    public bool At(int x, int z) {
        Collider[] colliders  = Physics.OverlapBox(new Vector3(x, y, z),
            Vector3.one * .5f, Quaternion.identity, LayerMask.GetMask("Walls"));
        return colliders.Length > 0;
    }
}
