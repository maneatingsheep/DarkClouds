using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnlyHelperObject : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

}
