using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floking : MonoBehaviour
{

    float speed; 
    bool turnig = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(FlockingManager.FM.minSpeed, FlockingManager.FM.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockingManager.FM.transform.position, FlockingManager.FM.flyLimit);

        if (!b.Contains(transform.position))
        {
            turnig = true;
        }
        else 
            turnig = false;

        if (turnig)
        {
            Vector3 direction = FlockingManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                FlockingManager.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if(Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockingManager.FM.minSpeed, FlockingManager.FM.maxSpeed);
            }
            applayRules();
            if(Random.Range(0, 100) < 100)
            {
               
            }
        }
        
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void applayRules()
    {
        GameObject[] gos;
        gos = FlockingManager.FM.allElement; 

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f; 
        float nDistance;
        int groupSize = 0; 

        foreach(GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if(nDistance <= FlockingManager.FM.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Floking anotherFloking = go.GetComponent<Floking>();
                    gSpeed = gSpeed + anotherFloking.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (FlockingManager.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            if(speed > FlockingManager.FM.maxSpeed)
            {
                speed = FlockingManager.FM.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        FlockingManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
