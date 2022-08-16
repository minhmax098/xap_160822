using UnityEngine;
using static UnityEngine.ParticleSystem;
public class LoadingEffectManager : MonoBehaviour
{
    private static LoadingEffectManager instance;
    public static LoadingEffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LoadingEffectManager>();
            }
            return instance;
        }
    }

    public GameObject objectLoadingEffect;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide loading effect
    /// </summary>
    /// <param name="isActiveEffect"></param>
    public void ShowLoadingEffect(bool isActiveEffect)
    {
        objectLoadingEffect.SetActive(isActiveEffect);
    }
}
