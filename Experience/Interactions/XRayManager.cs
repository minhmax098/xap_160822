using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class XRayManager : MonoBehaviour
{
    private static XRayManager instance;
    public static XRayManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<XRayManager>();
            }
            return instance;
        }
    }
    public Button btnXray;
    public Dictionary<string, Material[]> DictionaryMaterialOriginal { get; set; } = new Dictionary<string, Material[]>();
    public Material transparentMeterial;
    Material[] temp;
    Material[] newMaterials;
    private bool isMakingXRay;

    public bool IsMakingXRay
    {
        get
        {
            return isMakingXRay;
        }
        set
        {
            isMakingXRay = value;
            btnXray.GetComponent<Image>().sprite = isMakingXRay ? Resources.Load<Sprite>(PathConfig.XRAY_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.XRAY_UNCLICK_IMAGE);
        }
    }
    void Start()
    {
        newMaterials = new Material[] { transparentMeterial };
    }

    public void HandleXRayView(bool currentXRayStatus)
    {
        IsMakingXRay = currentXRayStatus;
        if (IsMakingXRay)
        {
            btnXray.interactable = false;
            ChangeMaterial(ObjectManager.Instance.OriginObject);
            btnXray.interactable = true;
        }
        else
        {
            btnXray.interactable = false;
            BackToOriginMaterial(ObjectManager.Instance.OriginObject);
            btnXray.interactable = true;
        }
    }
    public void GetOriginalMaterial(GameObject objectInstance)
    {
        var renderers = objectInstance.GetComponentsInChildren<MeshRenderer>(true);
        if (renderers.Length <= 0)
            return;

        foreach (var item in renderers)
            DictionaryMaterialOriginal.Add(item.gameObject.name, item.materials);
    }

    public void ChangeMaterial(GameObject objectInstance)
    {
        var renderers = objectInstance.GetComponentsInChildren<MeshRenderer>(true);

        if (renderers.Length <= 0)
            return;

        foreach (var item in renderers)
        {
            item.materials = newMaterials;
        }
    }
    public void BackToOriginMaterial(GameObject objectInstance)
    {
        var renderers = objectInstance.GetComponentsInChildren<MeshRenderer>(true);
        if (renderers.Length <= 0)
            return;

        foreach (var item in renderers)
        {
            if (DictionaryMaterialOriginal.TryGetValue(item.gameObject.name, out temp))
                item.materials = temp;
        }
    }
}
