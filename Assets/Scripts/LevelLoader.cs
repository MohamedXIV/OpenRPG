using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingBar;
    public Image loadingSlider;
    private TextMeshPro loadingText;

    private void Start()
    {
        //loadingText = GetComponent<TextMeshPro>();
    }

    public void LoadLevel(int sI)
    {
        StartCoroutine(LoadAsync(sI));
    }

    IEnumerator LoadAsync(int sI)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sI);
        
        loadingBar.SetActive(true);
        
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //loadingText.text = progress.ToString();
            loadingSlider.fillAmount = progress;
            Debug.Log(progress);
            
            yield return null;
        }
    }
    
}
