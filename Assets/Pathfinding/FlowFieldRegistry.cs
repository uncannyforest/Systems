using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldRegistry : MonoBehaviour {
    private static FlowFieldRegistry instance;
    FlowFieldRegistry(): base() {
        instance = this;
    }
    public static FlowFieldRegistry I { get => instance; }

    public BoundsInt bounds;
    public float barrierCost = 255;
    public float y;
    public float updateTime = 1;

    private DetectBarriers detectBarriers;
    private Action<FlowField> animate;

    private Dictionary<Transform, FlowField> values = new Dictionary<Transform, FlowField>();

    void Start() {
        this.detectBarriers = new DetectBarriers(y);
        animate = FindObjectOfType<FlowFieldsAnimation>().Animate;
    }

    public static float Get(Transform key, Vector3 follower) {
        FlowField ff = instance.GetKey(key);
        return ff.GetDirection(Mathf.RoundToInt(follower.x), Mathf.RoundToInt(follower.z), key.position);
    }
    private FlowField GetKey(Transform key) {
        if (!values.ContainsKey(key)) {
            values.Add(key, new FlowField(key.position, bounds, updateTime, detectBarriers, barrierCost, animate));
        }
        return values[key];
    }
}
