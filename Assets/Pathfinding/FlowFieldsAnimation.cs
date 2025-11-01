using UnityEngine;

public class FlowFieldsAnimation : MonoBehaviour {
    public GameObject prefab;

    private GameObject player;
    private Transform[,] arrow;

    void Start() {
        player = GameObject.Find("Player");

        arrow = new Transform[FlowFieldRegistry.I.bounds.max.x - FlowFieldRegistry.I.bounds.min.x + 1,
            FlowFieldRegistry.I.bounds.max.z - FlowFieldRegistry.I.bounds.min.z + 1];
        for (int x = 0; x < arrow.GetLength(0); x++)
            for (int y = 0; y < arrow.GetLength(1); y++)
                arrow[x, y] = GameObject.Instantiate(prefab, FlowFieldRegistry.I.bounds.min + new Vector3Int(x, 0, y),
                    Quaternion.identity, transform).transform;
    }

    public void Animate(FlowField ff) {
        for (int x = ff.boundsMin.x; x <= ff.boundsMax.x; x++)
            for (int y = ff.boundsMin.y; y <= ff.boundsMax.y; y++)
                ff.Get(arrow, new Vector2Int(x, y)).transform.rotation =
                    Quaternion.Euler(0, -ff.GetDirection(x, y, player.transform.position), 0);
    }
}
