using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MeshFactory {
    public Color[] colorsById = new Color[] { Color.black, Color.red, Color.green, Color.blue, Color.white };

    public Transform armature;
    public Transform body;
    public Transform ikTargets;
    public TwoBoneIKConstraintData defaultIKData;
    public Dictionary<string, List<Renderer>> meshesByName = new Dictionary<string, List<Renderer>>();

    public List<int> tris = new List<int>();
    public List<Vector3> verts = new List<Vector3>();
    public List<Color> colors = new List<Color>();
    public List<BoneWeight> weights = new List<BoneWeight>();
    public List<Transform> bones = new List<Transform>();
    public List<Matrix4x4> bindPoses = new List<Matrix4x4>();
    public List<TwoBoneIKConstraintData> ikData = new List<TwoBoneIKConstraintData>();

    public MeshFactory(Transform armature, Transform ikTargets, TwoBoneIKConstraint ikPrefab) {
        this.armature = armature;
        this.ikTargets = ikTargets;
        this.defaultIKData = ikPrefab.data;
    }

    public void UpdateMeshData(Mesh mesh) {
        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.colors = colors.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.boneWeights = weights.ToArray();
        mesh.bindposes = bindPoses.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }
    public void UpdateMeshComponents(SkinnedMeshRenderer rend) {
        UpdateMeshData(rend.sharedMesh);
        rend.bones = bones.ToArray();
        MeshCollider collider = rend.GetComponent<MeshCollider>();
        if (collider != null) {
            collider.sharedMesh = null;
            collider.sharedMesh = rend.sharedMesh;
        }
    }
    public void UpdateIKComponents(RigBuilder rigBuilder, Rig rig, TwoBoneIKConstraint prefab,
            Action<IEnumerator<YieldInstruction>> runner) {
        foreach (Transform child in rig.transform) GameObject.Destroy(child.gameObject);
        foreach (TwoBoneIKConstraintData data in ikData) {
            TwoBoneIKConstraint ik = GameObject.Instantiate(prefab, rig.transform);
            ik.data = data;
        }
        rigBuilder.enabled = false;
        runner(DoNextFrame(() => rigBuilder.enabled = true));
    }
    private IEnumerator<YieldInstruction> DoNextFrame(Action action) {
        yield return null;
        action();
    }
    public void AddColorId(int id) {
        colors.Add(colorsById[id]);
    }
    public void AddMeshByName(string name, Renderer mesh) {
        if (meshesByName.ContainsKey(name)) {
            meshesByName[name].Add(mesh);
        } else {
            meshesByName[name] = new List<Renderer>() { mesh };
        }
    }
    public List<Renderer> MeshesByName(string name) {
        meshesByName.TryGetValue(name, out List<Renderer> result);
        return result;
    }
}

public class MaterialFactory {
    public Material blueprint;
    public Color[] colors;

    public MaterialFactory(Material blueprint, params Color[] colors) {
        this.blueprint = blueprint;
        this.colors = colors;
    }

    public void Set(List<Renderer> renderers) {
        if (renderers == null || renderers.Count == 0) return;
        Material material = new Material(blueprint);
        if (colors.Length > 1) {
            for (int i = 0; i < Math.Min(4, colors.Length); i++)
                material.SetColor("_Color" + (i + 1), colors[i]);
        } else {
            material.SetColor("_Color", colors[0]);
        }
        foreach (Renderer renderer in renderers) {
            renderer.material = material;
        }
    }
}

public static class MF {
    public static int[] Tri(bool flip, int a, int b, int c) =>
        flip ? new int [] {c, b, a} : new int[] {a, b, c};
    public static int[] Quad(bool flip, int a, int b, int c, int d) =>
        flip ? new int [] {a, d, c, c, b, a} : new int[] {a, b, c, c, d, a};

    public static BoneWeight BoneWeight(int idx0, float weight0) {
        BoneWeight result = new BoneWeight();
        result.boneIndex0 = idx0;
        result.weight0 = weight0;
        return result;
    }
    public static BoneWeight BoneWeight(int idx0, float weight0, int idx1, float weight1) {
        BoneWeight result = BoneWeight(idx0, weight0);
        result.boneIndex1 = idx1;
        result.weight1 = weight1;
        return result;
    }

    public static Transform AddTransform(Transform parent, string name, Vector3 localPosition, Quaternion localRotation) {
        Transform result = new GameObject(name).transform;
        result.parent = parent;
        result.localPosition = localPosition;
        result.localRotation = localRotation;
        return result;
    }

    public static TwoBoneIKConstraintData IK(MeshFactory m, Transform tip, Transform target, Transform hint) {
        TwoBoneIKConstraintData result = m.defaultIKData;
        result.tip = tip;
        result.mid = tip.parent;
        result.root = result.mid.parent;
        result.target = target;
        result.hint = hint;
        return result;
    }

    public static void MarkVertex(List<Vector3> verts, List<int> markedVerts) {
        markedVerts.Add(verts.Count - 1);
    }
}
