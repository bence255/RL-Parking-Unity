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
    public float steering = 0f;
    public float gas = 0f;

    private Vector3 destination = new Vector3(-11f, 0.09f, 7f);
    private float previousDistance = 0f;

    public override void OnEpisodeBegin()
    {
        Debug.Log("episode");
        
        // Visszateszi az autot a kezdo pozicioba
        transform.localPosition = new Vector3(-7f, 0.09f, -5f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        previousDistance = Vector3.Distance(transform.localPosition, destination);
        //Debug.Log(Vector3.Distance(transform.localPosition, destination));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Az agens pozicioja
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        // A target pozicioja
        sensor.AddObservation(destination[0]);
        sensor.AddObservation(destination[2]);

        // Az agens es a target tavolsaga
        sensor.AddObservation(Vector3.Distance(transform.localPosition, destination));

        // Az agens iranya
        sensor.AddObservation(transform.localRotation.eulerAngles.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(Vector3.Distance(transform.localPosition, destination));
        var actionTaken = actions.ContinuousActions;
        steering = actionTaken[0];
        gas = actionTaken[1];

        // Checkpoint rewards
        float distance = Vector3.Distance(transform.localPosition, destination);
        AddReward((previousDistance - distance) / 10);
        previousDistance = distance;


        // Time penalty, comulative reward = -10
        // AddReward(-0.001f);

        // Car controls
        float speed = 4;
        transform.Translate(Vector3.forward * speed * gas * Time.fixedDeltaTime);
        float rotationSpeed = 90f;
        transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + Time.fixedDeltaTime * steering * rotationSpeed, 0);

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
        
        float deltaX = 1f;
        float deltaZ = 1f;
        //float deltaY = 15f;
        if (transform.localPosition.x < destination[0] + deltaX && transform.localPosition.x > destination[0] - deltaX
            && transform.localPosition.z < destination[2] + deltaZ && transform.localPosition.z > destination[2] - deltaZ
            // && ((transform.localRotation.eulerAngles.y < 90 + deltaY && transform.localRotation.eulerAngles.y > 90 - deltaY) || (transform.localRotation.eulerAngles.y < 270 + deltaY && transform.localRotation.eulerAngles.y > 270 - deltaY))
            )
            {
            Debug.Log("Sikeres parkolas");
            float rotationReward = 0;
            int granularity = 5;
            if (transform.rotation.eulerAngles.y < 180)
            {
                rotationReward = granularity / (Math.Abs(90 - transform.localRotation.eulerAngles.y) + granularity);
            }
            else 
            {
                rotationReward = granularity / (Math.Abs(270 - transform.localRotation.eulerAngles.y) + granularity);
            }
            float reward = rotationReward * 10 + 10;
            AddReward(reward);
            EndEpisode();
        }
    }

    // Debug
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        /*
        var action = actionsOut.ContinuousActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        // ALpaertelmezett ertek
        steering = 0;
        gas = 0;


        if (horizontal == -1)
        {
            steering = -1;
        }
        else if (horizontal == 1)
        {
            steering = 1;
        }
        else if (vertical == -1)
        {
            gas = -1;
        }
        else if (vertical == 1)
        {
            gas = 1;
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "ParkingCar") 
        {
            Debug.Log("ParkingCar collision");
            AddReward(-1f);
            EndEpisode();
        }

        if (collision.collider.tag == "Barrier")
        {
            Debug.Log("Barrier collision");
            AddReward(-1f);
            EndEpisode();
        }


    }
}
