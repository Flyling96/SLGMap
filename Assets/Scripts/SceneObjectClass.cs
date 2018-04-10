using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectClass : MonoBehaviour {

	
    public SceneObjectInfo sceneObjectInfo;

    public Vector3 position;

    public Quaternion rotation;


    public void SetPosition(Vector3 position)
    {
        position.y += transform.localScale.y * 0.5f;
        transform.position = HexMetrics.instance.Perturb(position);
        transform.rotation = Quaternion.Euler(0f, 360f * Random.value, 0f);
    }
}
