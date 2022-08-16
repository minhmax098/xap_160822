using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrganManager : MonoBehaviour
{
   public static int organId; 
   public static string organName;
   public static void InitOrgan(int _organId, string _organName)
   {
       organId = _organId; 
       organName = _organName;
   }
}
