using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeftPanelHandler : MonoBehaviour
{
    [SerializeField]
    Animator statusAnimator;
    void Start()
    {
        InitUI();
    }
  
    void InitUI() 
    {
    }

    public void ShowLeftPanel() 
    {
        statusAnimator.SetBool(AnimatorConfig.showLeftPanel, true);
    }

    public void HideLeftPanel() 
    {
        statusAnimator.SetBool(AnimatorConfig.showLeftPanel, false);
    }
    
    public void JoinMeeting()
    {
        BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.meetingJoining);
        SceneManager.LoadScene(SceneConfig.meetingJoining);
    } 
}
