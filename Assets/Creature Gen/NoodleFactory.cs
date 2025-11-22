using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public static class NoodleFactory  {
 
    public static Transform AddNoodle(MeshFactory m, string name, int colorId, Vector3 origin, Vector3 relativeDestination, float noodleWidth, int sections = 1) {
        int lastVert = m.verts.Count;
        int lastBone = m.bones.Count;
        Vector3 direction = relativeDestination.normalized * noodleWidth;
        Vector3 sectionRelative = relativeDestination / sections;
        Vector3 location = origin;
        // add (4 * sections + 4) verts
        for (int i = 0; i <= sections; i++) {
            m.verts.Add(location + GetOrthogonal(direction, 0));
            m.verts.Add(location + GetOrthogonal(direction, 90));
            m.verts.Add(location + GetOrthogonal(direction, 180));
            m.verts.Add(location + GetOrthogonal(direction, 270));
            for (int j = 0; j < 4; j++) {
                if (i == 0) m.weights.Add(MF.BoneWeight(lastBone, 1));
                else if (i == sections) m.weights.Add(MF.BoneWeight(lastBone + sections - 1, 1));
                else m.weights.Add(MF.BoneWeight(lastBone + i - 1, .5f, lastBone + i, .5f));
            }
            for (int j = 0; j < 4; j++) m.AddColorId(colorId);
            location += sectionRelative;
        }
        // add (4 * sections) quads (double tris)
        for (int i = 0; i < sections; i++) {
            m.tris.AddRange(MF.Quad(false, lastVert + 1, lastVert + 2, lastVert + 6, lastVert + 5));
            m.tris.AddRange(MF.Quad(false, lastVert + 2, lastVert + 3, lastVert + 7, lastVert + 6));
            m.tris.AddRange(MF.Quad(false, lastVert + 3, lastVert    , lastVert + 4, lastVert + 7));
            m.tris.AddRange(MF.Quad(false, lastVert    , lastVert + 1, lastVert + 5, lastVert + 4));
            lastVert += 4;
        }
        // add (sections + 1) bones (Unity uses one more transform than Blender would show)
        location = origin;
        Transform currentBone = m.armature; // start with this transform then make generations of children
        for (int i = 0; i <= sections; i++) {
            currentBone = MF.AddTransform(currentBone, name + "_" + i, location, Quaternion.identity);
            m.bones.Add(currentBone);
            m.bindPoses.Add(currentBone.worldToLocalMatrix * m.armature.localToWorldMatrix);
            location = sectionRelative;
        }
        // add 2 IK targets
        if (sections == 2) { // IDK why neck is not working, so exclude it for now
            Transform ikTarget = MF.AddTransform(m.ikTargets, name + "_IKTarget",
                origin + relativeDestination, Quaternion.identity);
            Transform ikHint = MF.AddTransform(m.ikTargets, name + "_IKHint",
                origin + relativeDestination - sectionRelative, Quaternion.identity);
            m.ikData.Add(MF.IK(m, currentBone, ikTarget, ikHint));
        }
        // return last bone
        return currentBone;
    }

    private static Vector3 GetOrthogonal(Vector3 vector, float locationOnCircle) {
        // derived from https://stackoverflow.com/a/55465266
        return Quaternion.FromToRotation(Vector3.forward, vector) * (Quaternion.AngleAxis(locationOnCircle, Vector3.forward) * Vector3.right) * vector.magnitude;
    }
}
