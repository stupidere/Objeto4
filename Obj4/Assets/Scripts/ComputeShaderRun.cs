using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComputeShaderRun : MonoBehaviour
{
    public struct Object
    {
        public Vector3 position;
        public Color color;
    };

    ComputeBuffer computeBuffer;
    ComputeBuffer colorBuffer;
    [SerializeField] ComputeShader objController;
    [SerializeField] ComputeShader objColor;
    public TMP_InputField objectCountSimulation;
    public TMP_InputField minMassInput;
    public TMP_InputField maxMassInput;

    public int cycles;
    public int objectCount;
    public float minMassValue;
    public float maxMassValue;
    public float initialTimeRecorded, oldTimeRecorded, newTimeRecorded;
    public GameObject objSample;
    public GameObject[] objHolder;
    public Material material;
    Object[] objInfo;
    bool isCPUStarted, isGPUStarted = false;



    public void CPURunSimulation()
    {
        objectCount = int.Parse(objectCountSimulation.text);
        minMassValue = int.Parse(minMassInput.text);
        maxMassValue = int.Parse(maxMassInput.text);
        objInfo = new Object[objectCount];
        objHolder = new GameObject[objectCount];

    }



}


