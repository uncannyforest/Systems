using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureFactory : MonoBehaviour {
    public NoodleFactory appendage;

    void Start() {
        MakeHumanoid();
    }

    private void MakeHumanoid() {
        NoodleFactory.Build("LeftLeg", appendage, transform,
            new Vector3(-.5f, 2.5f, 0), new Vector3(0, -2, 0), 2);
        NoodleFactory.Build("RightLeg", appendage, transform,
            new Vector3(.5f, 2.5f, 0), new Vector3(0, -2, 0), 2);
        NoodleFactory.Build("LeftArm", appendage, transform,
            new Vector3(-.5f, 5, 0), new Vector3(-2.5f, 0, 0), 2);
        NoodleFactory.Build("RightArm", appendage, transform,
            new Vector3(.5f, 5, 0), new Vector3(2.5f, 0, 0), 2);
        NoodleFactory.Build("Neck", appendage, transform,
            new Vector3(0, 5, 0), new Vector3(0, 2, 0), 1);
    }
}
