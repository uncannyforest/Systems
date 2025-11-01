using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class FlowField {
    private const float SQRT_2 = 1.4142135624f; 

    private Vector3 location;
    private float expiry;
    public Vector2Int boundsMin;
    public Vector2Int boundsMax;
    private DetectBarriers detectBarriers;
    private float barrierCost;
    private Action<FlowField> updateHook;

    private float expiryTime;
    private float[,] direction;
    private float[,] totalCost;

    public FlowField(Vector3 location, BoundsInt bounds, float expiry, DetectBarriers detectBarriers, float barrierCost, Action<FlowField> updateHook = null) {
        this.detectBarriers = detectBarriers;
        this.expiry = expiry;
        this.boundsMin = new Vector2Int(bounds.min.x, bounds.min.z);
        this.boundsMax = new Vector2Int(bounds.max.x, bounds.max.z);
        this.detectBarriers = detectBarriers;
        this.barrierCost = barrierCost;
        this.updateHook = updateHook;
        this.direction = new float[bounds.max.x - bounds.min.x + 1, bounds.max.z - bounds.min.z + 1];
        this.totalCost = new float[bounds.max.x - bounds.min.x + 1, bounds.max.z - bounds.min.z + 1];
        Update(location);
    }

    public float GetDirection(int x, int y, Vector3 location) {
        if (!InBounds(x, y))
            throw new IndexOutOfRangeException("Out of bounds: " + x + ", " + y);
        if (Time.time > expiryTime) Update(location);
        return Get(direction, new Vector2Int(x, y));
    }

    private bool InBounds(int x, int y) {
        return x >= boundsMin.x && y >= boundsMin.y && x <= boundsMax.x && y <= boundsMax.y;
    }

    public void Update(Vector3 location) {
        this.location = location;
        Update();
    }

    private void Update() {
        expiryTime = Time.time + expiry;

        Array.Clear(direction, 0, direction.Length);
        Array.Clear(totalCost, 0, totalCost.Length);

        Vector3Int start = Vector3Int.RoundToInt(location);
        SimplePriorityQueue<Vector2Int> q = new SimplePriorityQueue<Vector2Int>();
        q.Enqueue(new Vector2Int(start.x, start.z), 1);
        Set(direction, q.First, Vector3.SignedAngle(Vector3.right, location - start, Vector3.down));
        
        while (q.Count > 0) {
            Vector2Int current = q.First;
            float currentValue = q.GetPriority(current);
            q.Dequeue();
            Set(totalCost, current, currentValue);
            float currentAngle = Get(direction, current);

            ProcessAdjacent(current, (adj, dir, dist) => {
                if (!InBounds(adj.x, adj.y)) return;
                if (IsClosed(adj)) return;

                float possPriority = dist * GetCostMultiplier(adj) + currentValue;
                if (!q.Contains(adj)) {
                    q.Enqueue(adj, possPriority);
                    Set(direction, adj, AverageAngles(currentAngle, dir));
                } else if (q.GetPriority(adj) > possPriority) {
                    q.UpdatePriority(adj, possPriority);
                    Set(direction, adj, AverageAngles(currentAngle, dir));
                }
            });
        }

        if (updateHook != null) updateHook(this);
    }

    public T Get<T>(T[,] array, Vector2Int location) {
        Vector2Int index = location - boundsMin;
        return array[index.x, index.y];
    }
    private void Set(float[,] array, Vector2Int location, float value) {
        Vector2Int index = location - boundsMin;
        array[index.x, index.y] = value;
    }

    private bool IsClosed(Vector2Int location) => Get(totalCost, location) != 0;

    private void ProcessAdjacent(Vector2Int current, Action<Vector2Int, float, float> process) {
        process(current + new Vector2Int(1, 0), 180, 1);
        process(current + new Vector2Int(1, 1), 225, SQRT_2);
        process(current + new Vector2Int(0, 1), 270, 1);
        process(current + new Vector2Int(-1, 1), 315, SQRT_2);
        process(current + new Vector2Int(-1, 0), 0, 1);
        process(current + new Vector2Int(-1, -1), 45, SQRT_2);
        process(current + new Vector2Int(0, -1), 90, 1);
        process(current + new Vector2Int(1, -1), 135, SQRT_2);
    }
    
    private float GetCostMultiplier(Vector2Int location) {
        return detectBarriers.At(location.x, location.y) ? barrierCost : 1;
    }

    // input and output range [0, 360)
    private float AverageAngles(float a, float b) {
        float max = Mathf.Max(a, b);
        float min = Mathf.Min(a, b);
        if (max - min < 180) return (max + min) / 2;
        float result = (max + min + 360) / 2;
        if (result >= 360) return result - 360;
        else return result;
    }
}