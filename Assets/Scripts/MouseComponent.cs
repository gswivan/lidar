using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseComponent : MonoBehaviour
{
    
    public GameObject FireWork;
    
    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
               GameObject obj = Instantiate(FireWork, hit.point, Quaternion.identity);
               GameObject.Destroy(obj, 4.0f);
            }
        }
    }
}

