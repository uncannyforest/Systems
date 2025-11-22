using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CreatureFactory : MonoBehaviour {
    public CreatureInputs inputs;
    public SkinnedMeshRenderer mesh;
    public Transform armature;
    public Transform ikTargets;
    public RigBuilder rigBuilder;
    public Rig rig;
    public TwoBoneIKConstraint ikConstraintPrefab;
    public GameObject headPrefab;
    public Material headMaterial;
    public Material headMaterialEyeShadow;
    public GameObject bodyPrefab;
    public Material bodyMaterial;
    public GameObject handPrefab;
    public GameObject footPrefab;
    public GameObject hairMeshes;
    public Material noodleMaterial;
    public Material oneColor;

    private MeshFactory meshFactory;

    void Start() {
        Generate();
    }

    public void Generate() {
        if (mesh.sharedMesh == null) mesh.sharedMesh = new Mesh();

        MakeHumanoid();

        meshFactory.UpdateMeshComponents(mesh);
        meshFactory.UpdateIKComponents(rigBuilder, rig, ikConstraintPrefab, RunCoroutine);

        inputs.PrepareHooks(Generate, ShapeTorso, SetColors);
    }

    private void MakeHumanoid() {
        CreatureInputs I = inputs;
        foreach (Transform child in armature) Destroy(child.gameObject);
        foreach (Transform child in ikTargets) Destroy(child.gameObject);

        Transform chest = MF.AddTransform(armature, "Chest", new Vector3(0, I.ChestY, 0), Quaternion.identity);
        meshFactory = new MeshFactory(chest, ikTargets, ikConstraintPrefab);
        meshFactory.AddMeshByName("Skin", mesh);
        Transform leftFoot = NoodleFactory.AddNoodle(meshFactory, "LeftLeg", 3,
            new Vector3(.5f, -I.torsoHeight, 0), new Vector3(0, -I.legLength, 0), I.noodleWidth, 2);
        PlaceMesh(leftFoot, footPrefab, "Shoe");
        Transform rightFoot = NoodleFactory.AddNoodle(meshFactory, "RightLeg", 3,
            new Vector3(-.5f, -I.torsoHeight, 0), new Vector3(0, -I.legLength, 0), I.noodleWidth, 2);
        PlaceMesh(rightFoot, footPrefab, "Shoe");
        Transform leftHand = NoodleFactory.AddNoodle(meshFactory, "LeftArm", 2,
            new Vector3(.5f, 0, 0), new Vector3(I.armLength, 0, 0), I.noodleWidth, 2);
        PlaceMesh(leftHand, handPrefab, "Hand");
        Transform rightHand = NoodleFactory.AddNoodle(meshFactory, "RightArm", 2,
            new Vector3(-.5f, 0, 0), new Vector3(-I.armLength, 0, 0), I.noodleWidth, 2);
        PlaceMesh(rightHand, handPrefab, "Hand");
        Transform head = NoodleFactory.AddNoodle(meshFactory, "Neck", 1,
            new Vector3(0, 0, 0), new Vector3(0, I.neckLength, 0), I.noodleWidth, 1);
        PlaceMesh(head, headPrefab, "Head").localRotation = Quaternion.Euler(0, -90, 0);
        SelectMesh(head, hairMeshes, I.hair);
        meshFactory.body = PlaceMesh(chest, bodyPrefab, "Body");
        ShapeTorso();
        SetColors();
    }

    private void ShapeTorso() {
        meshFactory.body.Find("Armature").localPosition = Vector3.zero;
        Transform chest = meshFactory.body.Find("Armature/Base/Chest");
        Transform waist = meshFactory.body.Find("Armature/Base/Waist");
        Transform height = meshFactory.body.Find("Armature/Base/Height");
        Transform hips = height.Find("Hips");
        Transform skirt = height.Find("Skirt");
        chest.localScale = inputs.ChestScale;
        waist.localScale = inputs.WaistScale;
        hips.localScale = inputs.HipsScale;
        height.localPosition = inputs.HeightPosition;
        skirt.localPosition = inputs.SkirtPosition;
        skirt.localScale = inputs.SkirtScale;
    }
    private void SetColors() {
        CreatureInputs I = inputs;
        new MaterialFactory(noodleMaterial, I.skinColor, I.ArmColor, I.LegColor)
            .Set(meshFactory.MeshesByName("Skin"));
        new MaterialFactory(bodyMaterial, I.skinColor, I.torsoColor1, I.torsoColor2)
            .Set(meshFactory.MeshesByName("Body"));
        new MaterialFactory(I.eyeShadow ? headMaterialEyeShadow : headMaterial,
                I.skinColor, I.eyeColor, Color.black, Color.white)
            .Set(meshFactory.MeshesByName("Head"));
        new MaterialFactory(oneColor, I.hairColor)
            .Set(meshFactory.MeshesByName("Hair"));
        new MaterialFactory(oneColor, I.skinColor)
            .Set(meshFactory.MeshesByName("Hand"));
        new MaterialFactory(oneColor, I.shoeColor)
            .Set(meshFactory.MeshesByName("Shoe"));
    }

    private Transform PlaceMesh(Transform bone, GameObject mesh, string name) {
        Transform child = GameObject.Instantiate(mesh, bone).transform;
        child.localPosition = Vector3.zero;
        child.rotation = Quaternion.identity;
        child.gameObject.name = name;
        meshFactory.AddMeshByName(name, child.GetComponentInChildren<Renderer>());
        return child;
    }

    private void SelectMesh(Transform parent, GameObject prefabParent, int index) {
        if (index == 0) return;
        PlaceMesh(parent, prefabParent.transform.GetChild(index - 1).gameObject, "Hair");
    }

    private void RunCoroutine(IEnumerator<YieldInstruction> enumerator) {
        StartCoroutine(enumerator);
    }
}
