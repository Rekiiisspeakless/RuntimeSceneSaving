using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SceneSavingManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        

    }

    public void SaveScene(int buildIndex)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/VR_Scene.dat");
        Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
        SceneObject originalScene = new SceneObject();
        GameObject[] objects = scene.GetRootGameObjects();
        Queue<GameObject> queue = new Queue<GameObject>();
        originalScene.isScene = true;
        originalScene.childObjects = new SceneObject[objects.Length];
        originalScene.objectPath = scene.path;
        Debug.Log(objects.Length + " root GameObject detected...");
        int count = 0;
        foreach (GameObject o in objects)
        {
            Debug.Log("gameObject " + o.name + " is enqueued...");
            queue.Enqueue(o);
            while (queue.Count > 0)
            {
                GameObject root = queue.Dequeue();
                Debug.Log("gameObject " + root.name + " is dequeued...");
                SceneObject sceneObject = new SceneObject();
                sceneObject.name = root.name;
                sceneObject.position = root.transform.position;
                sceneObject.rotation = root.transform.eulerAngles;
                sceneObject.scale = root.transform.localScale;
                sceneObject.isScene = false;
                if(root.GetComponent<Renderer>() != null)
                {
                    sceneObject.mainTexturePath = AssetDatabase.GetAssetPath(
                    root.GetComponent<Renderer>().material.mainTexture);
                    sceneObject.mainTextureOffset = root.GetComponent<Renderer>().material.mainTextureOffset;
                    sceneObject.mainTextureTiling = root.GetComponent<Renderer>().material.mainTextureScale;
                    sceneObject.bumpMapPath = AssetDatabase.GetAssetPath(
                        root.GetComponent<Renderer>().material.GetTexture("_BumpMap"));
                    sceneObject.bumpMapOffset = root.GetComponent<Renderer>().material.GetTextureOffset("_BumpMap");
                    sceneObject.bumpMapTiling = root.GetComponent<Renderer>().material.GetTextureScale("_BumpMap");
                }
                
                if(PrefabUtility.GetPrefabParent(root) != null && PrefabUtility.GetPrefabObject(root) != null)
                {
                    string prefabPath = AssetDatabase.GetAssetPath(
                        PrefabUtility.GetPrefabParent(root));
                    Debug.Log(root.name + "'s prefab path: " + prefabPath);
                    sceneObject.objectPath = AssetDatabase.GetAssetPath(
                        PrefabUtility.GetPrefabParent(root));
                    sceneObject.isRootGameObject = false;
                }else
                {
                    Debug.Log("object scene name: " + root.scene.name);
                    sceneObject.isRootGameObject = true;
                }
                if(root.transform.parent == null)
                {
                    Debug.Log(count + "th root gameObject saved...");
                    Debug.Log("Root gameObject name: " + root.name);
                    originalScene.childObjects[count] = sceneObject;
                    count++;
                }
                Transform[] childs = root.GetComponentsInChildren<Transform>();
                for (int i = 0; i < childs.Length; ++ i)
                {
                    if(childs[i].gameObject.GetInstanceID() != root.GetInstanceID())
                    {
                        queue.Enqueue(childs[i].gameObject);
                        Debug.Log("gameObject " + childs[i].gameObject.name + " is enqueued...");
                    }
                }
            }
        }
        binary.Serialize(file, originalScene);
        file.Close();

    }
}
