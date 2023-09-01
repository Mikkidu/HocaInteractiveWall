using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySky : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(-0.1f, 0, 0);
        if(gameObject.transform.position.x <= -200)
        {
            Destroy(gameObject);
        }
    }
}
