using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour 
{
    public int currentStep = 1;
    [SerializeField] private List<GameObject> stepsObjects = new List<GameObject>();
    [SerializeField] GameObject tutorialObj;
    [SerializeField] GameObject box1;
    [SerializeField] GameObject box2;
    private void Awake()
    {
        if(PlayerPrefs.GetInt("tutorialCompleted") == 1)
        {
            foreach(BoxCollider2D box in box1.GetComponents<BoxCollider2D>())
            {
                box.enabled = true;
            }
            foreach (BoxCollider2D box in box2.GetComponents<BoxCollider2D>())
            {
                box.enabled = true;
            }
            tutorialObj.SetActive(false);
            currentStep = 6;
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
    }
    public void NextStep()
    {
       
        stepsObjects[currentStep-1].SetActive(false);  
        stepsObjects[currentStep].SetActive(true);
        currentStep++;
      
    }
    public void TutorialComplete()
    {
        stepsObjects[currentStep-1].SetActive(false);
        tutorialObj.SetActive(false);
    }
}
