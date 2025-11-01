using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class NoodleFactory : MonoBehaviour {
    public float noodleWidth = .25f;
    public Vector3 relativeEnd;
    public int segments = 1;

    private Mesh mesh;
    private SkinnedMeshRenderer rend;
    private MeshFactory meshFactory;

    public static NoodleFactory Build(string name, NoodleFactory prefab, Transform parent, Vector3 localPosition, Vector3 relativeEnd, int segments) {
        NoodleFactory factory = GameObject.Instantiate(prefab, parent);
        factory.gameObject.name = name;
        factory.transform.localPosition = localPosition;
        factory.relativeEnd = relativeEnd;
        factory.segments = segments;
        factory.UpdateMesh();
        return factory;
    }

    public void UpdateMesh() {
        rend = GetComponent<SkinnedMeshRenderer>();
        if (mesh == null) {
            mesh = new Mesh();
            rend.sharedMesh = mesh;
        }
        meshFactory = new MeshFactory();

        AddNoodle(meshFactory, Vector3.zero, relativeEnd, segments);
        UpdateUnityMesh();
    }

    private int[] Tri(bool flip, int a, int b, int c) =>
        flip ? new int [] {c, b, a} : new int[] {a, b, c};
    private int[] Quad(bool flip, int a, int b, int c, int d) =>
        flip ? new int [] {a, d, c, c, b, a} : new int[] {a, b, c, c, d, a};

    private BoneWeight BoneWeight(int idx0, float weight0) {
        BoneWeight result = new BoneWeight();
        result.boneIndex0 = idx0;
        result.weight0 = weight0;
        return result;
    }
    private BoneWeight BoneWeight(int idx0, float weight0, int idx1, float weight1) {
        BoneWeight result = BoneWeight(idx0, weight0);
        result.boneIndex1 = idx1;
        result.weight1 = weight1;
        return result;
    }

    private Transform Bone(string name, Transform parent, Vector3 localPosition, Quaternion localRotation) {
        Transform result = new GameObject(name).transform;
        result.parent = parent;
        result.localPosition = localPosition;
        result.localRotation = localRotation;
        return result;
    }

    private void MarkVertex(List<Vector3> verts, List<int> markedVerts) {
        markedVerts.Add(verts.Count - 1);
    }

    private void AddNoodle(MeshFactory m, Vector3 origin, Vector3 destination, int sections = 1) {
        int last = m.verts.Count;
        Vector3 direction = (destination - origin).normalized * noodleWidth;
        Vector3 location = origin;
        // add (4 * sections + 4) verts
        for (int i = 0; i <= sections; i++) {
            m.verts.Add(location + GetOrthogonal(direction, 0));
            m.verts.Add(location + GetOrthogonal(direction, 90));
            m.verts.Add(location + GetOrthogonal(direction, 180));
            m.verts.Add(location + GetOrthogonal(direction, 270));
            for (int j = 0; j < 4; j++) {
                if (i == 0) m.weights.Add(BoneWeight(0, 1));
                else if (i == sections) m.weights.Add(BoneWeight(sections - 1, 1));
                else m.weights.Add(BoneWeight(i - 1, .5f, i, .5f));
            }
            location += (destination - origin) / sections;
        }
        // add (4 * sections) quads (double tris)
        for (int i = 0; i < sections; i++) {
            m.tris.AddRange(Quad(false, last + 1, last + 2, last + 6, last + 5));
            m.tris.AddRange(Quad(false, last + 2, last + 3, last + 7, last + 6));
            m.tris.AddRange(Quad(false, last + 3, last    , last + 4, last + 7));
            m.tris.AddRange(Quad(false, last    , last + 1, last + 5, last + 4));
            last += 4;
        }
        // add (sections) bones
        location = origin;
        Transform lastBone = transform; // start with this transform then make generations of children
        for (int i = 0; i < sections; i++) {
            lastBone = Bone("" + i, lastBone, location, Quaternion.identity);
            m.bones.Add(lastBone);
            m.bindPoses.Add(lastBone.worldToLocalMatrix * transform.localToWorldMatrix);
            location += (destination - origin) / sections;
        }
    }

    private Vector3 GetOrthogonal(Vector3 vector, float locationOnCircle) {
        // derived from https://stackoverflow.com/a/55465266
        return Quaternion.FromToRotation(Vector3.forward, vector) * (Quaternion.AngleAxis(locationOnCircle, Vector3.forward) * Vector3.right) * vector.magnitude;
    }

    private void UpdateUnityMesh() {
        meshFactory.UpdateMeshData(mesh);
        rend.bones = meshFactory.bones.ToArray();
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider != null) {
            collider.sharedMesh = null;
            collider.sharedMesh = mesh;
        }
    }
}

public class MeshFactory {
    public List<int> tris = new List<int>();
    public List<Vector3> verts = new List<Vector3>();
    public List<BoneWeight> weights = new List<BoneWeight>();
    public List<Transform> bones = new List<Transform>();
    public List<Matrix4x4> bindPoses = new List<Matrix4x4>();

    public void UpdateMeshData(Mesh mesh) {
        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.boneWeights = weights.ToArray();
        mesh.bindposes = bindPoses.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }
}
