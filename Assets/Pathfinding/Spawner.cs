using UnityEngine;

public class Spawner : MonoBehaviour {
    public int count;
    public FlowFieldFollower prefab;

    private int i = 0;

    private GameObject player;
    void Start() {
        player = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Update() {
        if (i < count) {
            Spawn(Vector3.zero);
            i++;
        }
    }

    void FixedUpdate() {
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Floor"))) {
                Spawn(hit.point);
            }
        }

    }

    private void Spawn(Vector3 position) {
        FlowFieldFollower fff = GameObject.Instantiate<FlowFieldFollower>(prefab);
        fff.GetComponent<Rigidbody>().position = position;
        fff.target = player;
    }
}
