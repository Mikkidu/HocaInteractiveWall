using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform spawnPoint1, spawnPoint2, spawnPoint3, endPoint4, endPoint5, endPoint6;
    public double timer;
    public GameObject sky1, sky2, sky3, sky4, sky5, sky6;

    // Start is called before the first frame update
    void Start()
    {
        timer = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            var randomNum = Random.Range(1, 4);
            print(randomNum);
            if (randomNum == 1)
            {
                Instantiate(sky1, spawnPoint1.position, sky1.transform.localRotation);
            }
            if (randomNum == 2)
            {
                Instantiate(sky2, spawnPoint2.position, sky2.transform.localRotation);
            }
            if (randomNum == 3)
            {
                Instantiate(sky3, spawnPoint3.position, sky3.transform.localRotation);
            }
            timer = 3;
        }
    }

    //public void startTimer()
    //{
    //    timer = 3;
    //    timer -= Time.deltaTime;

    //    if(timer <= 0.0f)
    //    {

    //        startTimer();
    //    }
    //}
}
