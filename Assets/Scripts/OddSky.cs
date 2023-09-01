using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OddSky : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(0.1f, 0, 0);
        if(gameObject.transform.position.x <= -130)
        {
            Destroy(gameObject);
        }
    }
}
