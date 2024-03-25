using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LogicManagerScript : MonoBehaviour
{

    public List<Dictionary<string, float>> beatMap;
    public float beatMapStartTime;

    private float registeredTapTimestamp;
    private bool newRegisteredTap = false;

    private int currentNote = 0; 
    private float bufferWindow = 0.2f;
    private float expectedInputWindow = 0.4f;

    public Queue<GameObject> noteObjectsQueue = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If current note exists
        if (currentNote < beatMap.Count)
        {
            // If the current note has been missed, go to the next one
            if (Time.time - beatMapStartTime > beatMap[currentNote]["timeStamp"] + bufferWindow)
            {
                Debug.Log("Missed Timing");
                currentNote++;
            }

            // If a tap has been registered, check if it is at the correct timing
            if (newRegisteredTap)
            {
                if (Math.Abs(beatMap[currentNote]["timeStamp"] - registeredTapTimestamp) <= bufferWindow)
                {
                    Debug.Log("Correct Timing");
                    DeleteNoteObject();
                    currentNote++;
                }
                else
                {
                    Debug.Log("Wrong Timing");
                    // Delete note if the wrong input was expected
                    if (Math.Abs(beatMap[currentNote]["timeStamp"] - registeredTapTimestamp) <= expectedInputWindow)
                    { 
                        DeleteNoteObject();
                        currentNote++;
                    }        
                }
                newRegisteredTap = false;
            }
        }
        // Level Complete
        else
        {
            
        }
    }

    public void Tap(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            registeredTapTimestamp = Time.time - beatMapStartTime;
            newRegisteredTap = true;
        }
    }

    public void DeleteNoteObject()
    {
        GameObject oldestNote = noteObjectsQueue.Dequeue();
        Destroy(oldestNote);
    }

}
