// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-09
// Description :
// **********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryEditor;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Test start");
        AddressableExample abe = GetComponent<AddressableExample>();
        abe.enabled = true;
        abe.cancel();
        //GameObject.DestroyImmediate(gameObject);
        Debug.Log($"Test load AddressableExample");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
