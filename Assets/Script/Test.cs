using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Test: MonoBehaviour {
    public GameObject target;
    public GameObject[] child;

    public GameObject outputTarget;
    //public Texture outputMainTexture;
    //public Texture outputBumpMap;
    public Material outputMaterial;
    public GameObject[] outputChild;
    public TestObject testObject;
    public string outputName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save()
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Test.dat");
        TestObject o = new TestObject();
        TestObject o2 = new TestObject();

        //o.o = target;
        o.name = target.name;
        o2.name = "test";
        o.o = o2;
        //o.mainTexture = target.GetComponent<Renderer>().material.mainTexture;
        //o.material = target.GetComponent<Renderer>().material;
        //o.bumpMap = target.GetComponent<Renderer>().material.GetTexture("_BumpMap");
        //o.child = child;

        binary.Serialize(file, o);
        file.Close();
    }

    public void Load()
    {
        print(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/Test.dat"))
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Test.dat", FileMode.Open);
            TestObject o = (TestObject)binary.Deserialize(file);
            file.Close();
            //outputTarget = o.o;
            outputName = o.name;
            testObject = o.o;
            //outputChild = o.child;
            //outputMainTexture = o.mainTexture;
            //outputMaterial = o.material;
            //outputBumpMap = o.bumpMap;
        }
    }
}
