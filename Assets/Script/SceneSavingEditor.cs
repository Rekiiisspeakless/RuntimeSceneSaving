using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SceneSavingEditor : ScriptableWizard {

    public Material defaultMaterial;
    public string sceneName;
    [MenuItem("Scene/Scene Saving Manager")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Save Scene", typeof(SceneSavingEditor), "Run");
    }

    void OnWizardCreate()
    {
        Load();
    }

    void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/VR_Scene.dat"))
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/VR_Scene.dat", FileMode.Open);
            SceneObject o = (SceneObject)binary.Deserialize(file);
             Scene scene = EditorSceneManager.OpenScene(o.objectPath);
            Queue<SceneObject> queue = new Queue<SceneObject>();
            GameObject[] rootGameObject = scene.GetRootGameObjects();
            int count = 0;
            for(int i = 0; i < o.childObjects.Length; ++i)
            {
                queue.Enqueue(o.childObjects[i]);
                while (queue.Count > 0)
                {
                    SceneObject root = queue.Dequeue();
                    GameObject target;
                    if(root.objectPath != null)
                    {
                        target = Instantiate<GameObject>((GameObject)AssetDatabase.LoadAssetAtPath<Object>(root.objectPath));
                        
                    }else
                    {
                        Debug.Log("Root gameObject " + root.name + " loaded...");
                        target = rootGameObject[count];
                        count++;
                    }
                    target.transform.position = root.position;
                    target.transform.eulerAngles = root.rotation;
                    target.transform.localScale = root.scale;
                    if (target.GetComponent<Renderer>() != null)
                    {
                        if (target.GetComponent<Renderer>().sharedMaterial == null)
                        {
                            target.GetComponent<Renderer>().sharedMaterial = new Material(defaultMaterial);
                        }
                        Texture mainTexture = (Texture)AssetDatabase.LoadMainAssetAtPath(root.mainTexturePath);
                        Texture bumpMap = (Texture)AssetDatabase.LoadMainAssetAtPath(root.bumpMapPath);
                        target.GetComponent<Renderer>().sharedMaterial = new Material(target.GetComponent<Renderer>().sharedMaterial);
                        target.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTexture", mainTexture);
                        target.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTexture", root.mainTextureOffset);
                        target.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTexture", root.mainTextureTiling);
                        target.GetComponent<Renderer>().sharedMaterial.SetTexture("_BumpMap", bumpMap);
                        target.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_BumpMap", root.bumpMapOffset);
                        target.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_BumpMap", root.bumpMapTiling);
                    }
                    if(root.childObjects != null)
                    {
                        Debug.Log(root.childObjects.Length + " child objects detected...");
                        for (int j = 0; j < root.childObjects.Length; ++j)
                        {
                            queue.Enqueue(root.childObjects[j]);
                            Debug.Log("gameObject " + root.childObjects[j].name + " is enqueued...");
                        }
                    }
                    
                }
            }
            string[] splits = o.objectPath.Split('/');
            string savePath = o.objectPath.Substring(0, o.objectPath.Length - splits[splits.Length - 1].Length);
            EditorSceneManager.SaveScene(scene, savePath + sceneName + ".unity", true);
            EditorSceneManager.OpenScene(savePath + sceneName + ".unity");
        }
    }
}
