using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureInputs : MonoBehaviour {
    public static float TAU = 6.28318530718f;
    public static float SQRT2 = 1.41421356237f;

    [Range(1, 5)] public float legLength = 3f;
    [Range(1, 3)] public float torsoHeight = 2f;
    [Range(1, 3)] public float neckLength = 2f;
    [Range(1, 4)] public float armLength = 2.5f;
    [Range(0, 1)] public float noodleWidth = .25f;
    [Range(0, 1)] public float breadthToWidthRatio = .5f;
    [Range(0, 1)] public float chest;
    [Range(0, 1)] public float waist;
    [Range(0, 1)] public float hips;
    public bool skirt;
    [Range(0, 1)] public float skirtLength;
    [Range(0, 1)] public float skirtFlare;
    public Color skinColor;
    [Range(0, 3)] public int hair;
    public Color hairColor;
    public Color shoeColor;
    public Color eyeColor;
    public bool eyeShadow;
    public Color torsoColor1;
    public bool sleeves;
    public Color torsoColor2;
    public bool pants;


    [NonSerialized] public Action torsoChanged;
    private int torsoHash = 0;
    [NonSerialized] public Action colorsChanged;
    private int colorHash = 0;
    [NonSerialized] public Action changed;
    private int generalHash = 0;

    public void PrepareHooks(Action changed, Action torsoChanged, Action colorsChanged) {
        this.changed = changed;
        this.torsoChanged = torsoChanged;
        this.colorsChanged = colorsChanged;
        generalHash = GenGeneralHash();
        torsoHash = GenTorsoHash();
        colorHash = GenColorHash();
    }
    private int GenGeneralHash() => Hash(legLength, torsoHeight, neckLength, armLength, noodleWidth, hair);
    private int GenTorsoHash() => Hash(breadthToWidthRatio, chest, waist, hips, skirt, skirtLength, skirtFlare);
    private int GenColorHash() => Hash(skinColor, hairColor, shoeColor, eyeColor, eyeShadow, torsoColor1, sleeves, torsoColor2, pants);
    private int Hash(params object[] inputs) {
        int result = 0;
        for (int i = 0; i < inputs.Length; i++) {
            result += inputs[i].GetHashCode();
            result *= 10;
        }
        return result;
    }

    void OnValidate() {
        if (generalHash != GenGeneralHash() && changed != null) changed();
        else if (torsoHash != GenTorsoHash() && torsoChanged != null) torsoChanged();
        else if (colorHash != GenColorHash() && colorsChanged != null) colorsChanged();
    }

    public Color ArmColor { get => sleeves ? torsoColor1 : skinColor; }
    public Color LegColor { get => pants ? torsoColor2 : skinColor; }

    public float ChestY { get => legLength + torsoHeight; }

    public Vector3 HeightPosition { get => new Vector3(0, -torsoHeight, 0); }
    public Vector3 SkirtPosition { get => skirt ? new Vector3(0, -skirtLength * (legLength - 1), 0) : Vector3.zero; }
    public Vector3 SkirtScale { get {
        float scale = skirt ? 1 + skirtFlare * 2 : 0;
        return new Vector3(scale, 1, scale);
    }}

    public Vector3 WaistScale { get {
        float biasedBtwr = CenterBias(breadthToWidthRatio);
        biasedBtwr = Mathf.Lerp(.5f, biasedBtwr, waist * 4); // if waist < .25 downplay wtbr
        float breadthFactor = TrigBalance(biasedBtwr);
        float widthFactor = TrigBalance(1 - biasedBtwr);
        float scaledToRange = Mathf.LerpUnclamped(-1, 3, waist);
        return new Vector3(
            Mathf.LerpUnclamped(1, scaledToRange, breadthFactor),
            1,
            Mathf.LerpUnclamped(1, scaledToRange, widthFactor));
    }}

    public Vector3 ChestScale { get {
        float biasedBtwr = EndBias(breadthToWidthRatio);
        float breadthFactor = TrigBalance(biasedBtwr);
        float widthFactor = TrigBalance(1 - biasedBtwr);
        // points in linear transform: (waist, wtbr)-> min/max
        // (.5, .5)-> 1 // (1, .5)-> 1.5 // (0, 0)-> .5 
        float min = waist + .5f;
        // (.5, .5)-> 2 // (0, .5)->1.5 // (.5, 1)->2 // (.5, 0)->2
        float max = 1.5f + waist;
        float scaledToRange = Mathf.LerpUnclamped(min, max, chest);
        return new Vector3(
            Mathf.LerpUnclamped(1, scaledToRange, breadthFactor),
            1.25f,
            Mathf.LerpUnclamped(1, scaledToRange, widthFactor));
    }}
    public Vector3 HipsScale { get {
        float biasedBtwr = CenterBias(breadthToWidthRatio);
        float breadthFactor = TrigBalance(biasedBtwr);
        float widthFactor = TrigBalance(1 - biasedBtwr);
        // points in linear transform: (waist, wtbr)-> min/max
        // (.5, .5)-> 1 // (0, .5)-> 1 // (1, .5)-> 2 // (.5, 0)-> 1
        float min = 1 + 2 * waist * (waist - .5f);
        // (.5, .5)->2.5 // (0, .5)->2 // (.5, 0)->2.5 // (.5, 1)->4.5
        float max = 2f + waist + 4 * biasedBtwr * (biasedBtwr - .5f);
        float scaledToRange = Mathf.LerpUnclamped(min, max, hips);
        return new Vector3(
            Mathf.LerpUnclamped(1, scaledToRange, breadthFactor),
            1,
            Mathf.LerpUnclamped(1, scaledToRange, widthFactor));
    }}

    private static float CenterBias(float t) => t + Mathf.Sin(t * TAU) / TAU;
    private static float EndBias(float t) => t - Mathf.Sin(t * TAU) / TAU;
    private static float TrigBalance(float t) => Mathf.Sin(t * Mathf.PI / 2) * SQRT2;
}
