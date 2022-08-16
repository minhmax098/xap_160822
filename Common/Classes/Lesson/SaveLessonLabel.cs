using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLessonLabel : MonoBehaviour
{
    public static int labelId;
    public static void SaveLessonLabelId (int _labelId)
    {
        labelId = _labelId;
    }
}
