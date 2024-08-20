using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaves : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            print("SAVES DELETED");
            PlayerPrefs.DeleteAll();
        }
    }
}
