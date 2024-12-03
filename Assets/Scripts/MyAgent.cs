using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System;


public class MyAgent : Agent
{
    //Azert publicok mert a car controller igy tudja majd olvasni
    public float steering = 0f;
    public float brakes = 0f;
    public float gas = 0f;

    public GameObject parkingCar;

    public Vector3[] positions;
    private GameObject[] parkedCars = new GameObject[16];

    private int destination = 0;
    private int counter = 0;

    GameObject go;
    PrometeoCarController carController;


    public override void OnEpisodeBegin()
    {
        
        go = GameObject.Find("Car");
        carController = go.GetComponent<PrometeoCarController>();
        //Debug.Log("speed is: " + carController.localVelocityZ);

        Debug.Log("episode");
        
        //Visszateszi az autot a kezdo pozicioba
        transform.localPosition = new Vector3(-2f, -5.88f, -10f);
        transform.localRotation = Quaternion.Euler(0, 180, 0);


        //destination = Random.Range(0, 16);
        //Debug.Log(positions[destination]);
        destination = 6;

            for (int i = 0; i < 16; i++)
            {
                bool carSpawn = false;
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f && i!=destination)
                {
                    carSpawn = true;
                    //Debug.Log(carSpawn);
                }
                // Kitorli a kocsit
                if (!carSpawn && parkedCars[i])
                {
                    //Debug.Log("Spawn a car");
                    Destroy(parkedCars[i]);
                }
                // Hozzadja a kocsit
                if (carSpawn && !parkedCars[i])
                {
                    //Debug.Log("Despawn a car");
                    GameObject car = Instantiate(parkingCar, positions[i], Quaternion.Euler(0, 90, 0));
                    parkedCars[i] = car;
                }
            }
        counter++;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Az agens pozicioja
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        // A target pozicioja
        sensor.AddObservation(positions[destination][0]);
        sensor.AddObservation(positions[destination][2]);

        //Az agens es a target tavolsaga
        float distance = Vector3.Distance(transform.localPosition, positions[destination]);
        sensor.AddObservation(distance);

        // Az agens iranya
        sensor.AddObservation(transform.localRotation.eulerAngles.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log("Paraszt debug");
        var actionTaken = actions.ContinuousActions;
        steering = actionTaken[0];
        //brakes = actionTaken[1];
        gas = actionTaken[1];

        //Debug.Log("steering: "+actionTaken[0]);
        //Debug.Log("gas: "+actionTaken[1]);

        float distance = Vector3.Distance(transform.localPosition, positions[destination]);
        float reward = 1/distance;
        reward = reward / 10;
        AddReward(reward);
        

        CheckForOutOfBounds();
        CheckForSuccessfulParking();

    }
    private void CheckForOutOfBounds()
    {
        if (transform.localPosition.y < -7)
        {
            Debug.Log("y out of bounds");
            AddReward(-1);
            EndEpisode();
        }
    }
    private void CheckForSuccessfulParking()
    {

        //&& ((transform.localRotation.eulerAngles.y < 90 + marginOfErrorRotation && transform.localRotation.eulerAngles.y > 90 - marginOfErrorRotation) || (transform.localRotation.eulerAngles.y < 270 + marginOfErrorRotation && transform.localRotation.eulerAngles.y > 270 - marginOfErrorRotation))
        float deltaX = 3f;
        float deltaZ = 3f;
        //float marginOfErrorRotation = 15f;
        if (transform.localPosition.x < positions[destination][0] + deltaX && transform.localPosition.x > positions[destination][0] - deltaX
            && transform.localPosition.z < positions[destination][2] + deltaZ && transform.localPosition.z > positions[destination][2] - deltaZ

            )
        {
            float slownessReward = 1 / (carController.localVelocityZ);
            AddReward(slownessReward);
            AddReward(100);
            EndEpisode();
        }
    }


    
    




    // Debug
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "ParkingCar") 
        {
            Debug.Log("ParkingCar collision");
            AddReward(-0.1f);
            //EndEpisode();
        }

        if (collision.collider.tag == "Barrier")
        {
            Debug.Log("Barrier collision");
            AddReward(-1f);
            EndEpisode();
        }


    }
}
