using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelTypeManager : MonoBehaviour
{
    public List<Button> modelTypeBtns;
    public List<GameObject> modelPanelObject;
    public int currentModelTypeIndex { get; set; } = 0;
    private int[] modelTypes = new int[] { -1, 1, 2};

    private static ModelTypeManager instance;
    public static ModelTypeManager Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ModelTypeManager>();
            }
            return instance;
        }
    }

    public int GetCurrentModeType ()
    {
        return modelTypes[currentModelTypeIndex];
    }

    void Start()
    {
        InitEvents();
        modelTypeBtns[currentModelTypeIndex].onClick.Invoke();
    }

    void InitEvents()
    {
        foreach (Button modelTypeBtn in modelTypeBtns)
        {
            modelTypeBtn.onClick.AddListener(delegate { SelectModelPanel(modelTypeBtn); });
        }
    }

    void SelectModelPanel(Button selectedBtn)
    {   
        currentModelTypeIndex = modelTypeBtns.IndexOf(selectedBtn);
        ChangeActiveBtn();
        ChangeActiveModelPanel();
    }

    void ChangeActiveBtn()
    {
        for (int i = 0; i < modelTypeBtns.Count; i++)
        {
            ShowActiveButton(modelTypeBtns[i], i == currentModelTypeIndex);
        }
    }

    void ChangeActiveModelPanel()
    {
        for (int i = 0; i < modelPanelObject.Count; i++)
        {
            modelPanelObject[i].SetActive(i == currentModelTypeIndex);
        }
    }

    void ShowActiveButton(Button button, bool isActive)
    {
        if (isActive)
        {
            button.gameObject.GetComponent<Image>().color = Color.red;
            button.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        }
        else
        {
            button.gameObject.GetComponent<Image>().color = Color.white;
            button.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
    }
}
