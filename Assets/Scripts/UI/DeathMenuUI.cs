using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuUI : MonoBehaviour
{
   
    public void LoadScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
