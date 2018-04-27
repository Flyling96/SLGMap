using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneObjectItem : DownSelectWndItem
{
    SceneObjectInfo sceneObjectInfo = new SceneObjectInfo();
    public GameObject sceneObjectModel;

    public override void OnItemClick(bool isOn)
    {
        base.OnItemClick(isOn);
        if (isOn)
        {
            HexMetrics.instance.IsEditorSceneObject = true;
            if (sceneObjectModel == null)
            {
                sceneObjectModel = GameObject.Instantiate(Resources.Load(sceneObjectInfo.modelPath) as GameObject);
                sceneObjectModel.AddComponent<SceneObjectClass>();
                sceneObjectModel.GetComponent<SceneObjectClass>().sceneObjectInfo = sceneObjectInfo;
                sceneObjectModel.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                sceneObjectModel.SetActive(false);
            }
            HexMetrics.instance.editorSceneObject = sceneObjectModel;
            HexMetrics.instance.editorSceneObjectInfo = sceneObjectInfo;
            //GameObject item = GameObjectPool.instance.GetPoolChild(selectItem.name, selectItem);
        }
    }

    public override void Init()
    {
        base.Init();
        if (transform.Find("IconItem/Image") != null)
        {
            transform.Find("IconItem/Image").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public override void SetInfo(BaseInfo baseInfo)
    {
        sceneObjectInfo = (SceneObjectInfo)baseInfo;
        base.SetInfo(sceneObjectInfo);
        iconPath = sceneObjectInfo.iconPath;
        iconName = sceneObjectInfo.iconName;
    }

}
