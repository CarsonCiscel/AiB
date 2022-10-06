using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    // Look at build settings for scene ID 
    public void MoveToScene(int sceneID){
        SceneManager.LoadScene(sceneID);
    }
}
