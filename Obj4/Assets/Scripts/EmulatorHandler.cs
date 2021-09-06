using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmulatorHandler : MonoBehaviour
{
    struct Object
    {
        public Vector3 pos;
        public float velocity;
        public float mass;
        public Color color;
    }


    public TMP_InputField objectCountSimulation;
    public TMP_InputField minMassInput;
    public TMP_InputField maxMassInput;
    public TextMeshProUGUI CPUTEXT, GPUTEXT;


    public int objectCount;
    [SerializeField] ComputeShader computeShader;
    [SerializeField] float minMassValue, maxMassValue, minVelocity, maxVelocity;
    [SerializeField] GameObject floor;
    int count;
    [SerializeField] GameObject sample;
    [SerializeField] Material baseMaterial;

    Object[] data;
    GameObject[] objects;

    bool startGPU, startCPU = false;

    bool canSpawn = true;

    float lastTime;

    float startTime, endTime, timeToArrive;


    float time;


    void Update()
    {
        if (startGPU)
        {
            time += Time.deltaTime;
            int totalSize;
            int colorSize = sizeof(float) * 4;
            int positionSize = sizeof(float) * 3;

            int velocitySize = sizeof(float) * 1;
            int massSize = sizeof(float) * 1;

            totalSize = colorSize + positionSize + velocitySize + massSize;

            ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);
            computeBuffer.SetData(data);
            computeShader.SetBuffer(1, "objects", computeBuffer);
            computeShader.SetFloat("time", time);

            computeShader.Dispatch(1, data.Length, 1, 1);

            lastTime = time;

            computeBuffer.GetData(data);

            computeBuffer.Dispose();

            int a = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].velocity == 0)
                {
                    a++;
                }
                if (objects[i].transform.position.y <= 0.5f)
                {
                    Vector3 pos = objects[i].transform.position;
                    data[i].pos = new Vector3(pos.x, 1, pos.z);
                    data[i].velocity = 0;
                    objects[i].GetComponent<MeshRenderer>().material = new Material(baseMaterial);
                    objects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                    
                }
                objects[i].transform.position = data[i].pos;

            }
            if (a == data.Length && endTime == 0)
            {
                //StopTimer();
                startGPU = false;
                GPUTEXT.text = lastTime.ToString("00.000" + "ms");
            }
        }
        else if (startCPU)
        {
            PhysicsCPU();
        }
    }





    public void ParseButtonValues()
    {
        objectCount = int.Parse(objectCountSimulation.text);
        minMassValue = int.Parse(minMassInput.text);
        maxMassValue = int.Parse(maxMassInput.text);

        CreateObjects(0);
    }




    public void PhysicsGPU()
    {
        ZeroTimer();
        startCPU = false;
        int count = 0;
        foreach (Object o in data)
        {
            if (o.pos.y <= floor.transform.position.y + 1)
                count++;
        }
        if (count == data.Length)
        {
            CreateObjects(2);
        }
        else
        {
            int totalSize;
            int colorSize = sizeof(float) * 4;
            int positionSize = sizeof(float) * 3;

            int velocitySize = sizeof(float) * 1;
            int massSize = sizeof(float) * 1;

            totalSize = colorSize + positionSize + velocitySize + massSize;

            ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);
            computeBuffer.SetData(data);
            computeShader.SetBuffer(0, "objects", computeBuffer);
            computeShader.Dispatch(0, data.Length, 1, 1);
            computeBuffer.GetData(data);

            computeBuffer.Dispose();

            startGPU = true;
        }
    }

    public void PhysicsCPU()
    {
        ZeroTimer();
        startGPU = false;
        time += Time.deltaTime;
        int c = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].velocity == 0)
            {
                c++;
            }
            else
            {
                if (objects[i].transform.position.y <= 0.5f)
                {
                    Vector3 pos = objects[i].transform.position;
                    data[i].pos.y = floor.transform.position.y + 1;
                    data[i].velocity = 0;
                    objects[i].GetComponent<MeshRenderer>().material = new Material(baseMaterial);
                    objects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());


                }
                else
                {
                    float delta = time - lastTime;
                    data[i].velocity += (9.8f / data[i].mass) * delta;
                    data[i].pos += new Vector3(0, -data[i].velocity, 0) * delta;
                }
            }
            objects[i].transform.position = data[i].pos;

        }
        lastTime = time;
        endTime = 0;
        if (c == data.Length && endTime == 0)
        {

            startCPU = false;
            CPUTEXT.text = lastTime.ToString("00.000" + "ms");

        }
        
        startCPU = true;


    }


    public void ZeroTimer()
    {
        time = 0;
        lastTime = 0;

    }
    void CreateObjects(int zOffset)
    {
        objects = new GameObject[objectCount];
        data = new Object[objectCount];
        for (int i = 0; i < objectCount; i++)
        {

            float offsetX = -objectCount / 2 + i;
            //float posY = -objectCount / 2 + i;
            GameObject go = Instantiate(sample, new Vector3(offsetX * 2.5f, 80, zOffset), Quaternion.identity);
            objects[i] = go;
            data[i] = new Object();
            data[i].pos = go.transform.position;
            data[i].velocity = Random.Range(minVelocity, maxVelocity);
            data[i].mass = Random.Range(minMassValue, maxMassValue);

        }
    }

    public void StopTimer()
    {

       // endTime = Time.realtimeSinceStartup;

        timeToArrive = endTime - startTime;


        Debug.Log(timeToArrive);


    }
    public void ClearObj()
    {
        SceneManager.LoadScene(0);
    }


}

