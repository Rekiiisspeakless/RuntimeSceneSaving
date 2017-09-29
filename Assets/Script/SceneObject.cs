using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneObject{

    public string name;
    public string objectPath;
    public string materialPath;
    public string mainTexturePath;
    public string bumpMapPath;

    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public SerializableVector3 scale;

    public SerializableVector2 mainTextureOffset;
    public SerializableVector2 mainTextureTiling;
    public SerializableVector2 bumpMapOffset;
    public SerializableVector2 bumpMapTiling;

    public SceneObject[] childObjects;
    public bool isScene;
    public bool isRootGameObject;
}
