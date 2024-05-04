using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LogicManagerScript : MonoBehaviour
{
    public Queue<GameObject> circleObjectsQueue = new Queue<GameObject>();
    public Queue<GameObject> squareObjectsQueue = new Queue<GameObject>();
    public Queue<GameObject> triangleObjectsQueue = new Queue<GameObject>();
    public Queue<Dictionary<string, float>> circleTimingsQueue = new Queue<Dictionary<string, float>>();
    public Queue<Dictionary<string, float>> squareTimingsQueue = new Queue<Dictionary<string, float>>();
    public Queue<Dictionary<string, float>> triangleTimingsQueue = new Queue<Dictionary<string, float>>();

    [Header("Managers")]
    [SerializeField] private GameObject UIManager;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference circleActionReference;
    [SerializeField] private InputActionReference squareActionReference;
    [SerializeField] private InputActionReference triangleActionReference;

    [Header("Note Square")]
    private bool initialSquareInput = false;

    [Header("Timings")]
    public float beatMapStartTime;
    [SerializeField] private float bufferWindow; // The buffer window is a subset of the expected window
    [SerializeField] private float expectedWindow; // The expected window is where the user is expected to provide an input before the note is missed

    [Header("Particles")]
    [SerializeField] private GameObject inputParticles;

    private void OnEnable()
    {
        circleActionReference.action.Enable();
        squareActionReference.action.Enable();
        triangleActionReference.action.Enable();

        circleActionReference.action.performed += onCircle;
        squareActionReference.action.started += onSquareHold;
        squareActionReference.action.canceled += onSquareRelease;
        triangleActionReference.action.performed += onTriangle;
    }

    private void OnDisable()
    {
        circleActionReference.action.performed -= onCircle;
        squareActionReference.action.started -= onSquareHold;
        squareActionReference.action.canceled -= onSquareRelease;
        triangleActionReference.action.performed -= onTriangle;

        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
    }

    public void DisableInputs()
    {
        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
    }

    // Circle -> Single Tap
    private void onCircle(InputAction.CallbackContext context)
    {
        if (circleTimingsQueue.Count > 0) 
        { checkInputCircle(Time.time - beatMapStartTime); }
    }

    // Square -> Hold and Release
    private void onSquareHold(InputAction.CallbackContext context)
    {
        if (squareTimingsQueue.Count > 0)
        { checkInputSquareInitial(Time.time - beatMapStartTime); }
    }
    private void onSquareRelease(InputAction.CallbackContext context)
    {
        // initialSquareInput is only true if the first input is correct after checkInputSquareInitial is performed
        if (initialSquareInput && squareTimingsQueue.Count > 0) 
        { 
            checkInputSquareFinal(Time.time - beatMapStartTime);
            initialSquareInput = false;
        }
    }

    // Triangle -> Double Tap
    private void onTriangle(InputAction.CallbackContext context)
    {
        if (triangleTimingsQueue.Count > 0) 
        { checkInputTriangle(Time.time - beatMapStartTime); }
    }

    // For missed note, inputCorrect is false even though there was no input
    private void ProcessInput(bool inputCorrect, string inputDetails)
    {
        if (!inputCorrect)
        { UIManager.GetComponent<UIManagerScript>().TakeDamage(); }
        Debug.Log(inputDetails);
    }

    private void DequeueNote(Queue<GameObject> noteObjectsQueue, Queue<Dictionary<string, float>> noteTimingsQueue, bool destroyNote)
    {
        GameObject note = noteObjectsQueue.Dequeue();
        noteTimingsQueue.Dequeue();
        if (destroyNote)
        { Destroy(note); }
    }

    private void checkInputCircle(float inputTimeStamp)
    {
        float requiredTimeStamp = circleTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        if (timeFromPerfect <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Circle]");
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
    }

    private void checkInputSquareInitial(float inputTimeStamp)
    {
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        // If the first input is correct, destroy noteSquareStart only and wait for second input
        if (timeFromPerfect <= bufferWindow)
        {
            squareObjectsQueue.Peek().GetComponent<NoteSquareScript>().DestroyNoteSquareStart();
            initialSquareInput = true; // Give a bool flag for the second input such that
                                       // the second input is checked only if the first input is correct
        }
        // If the first input is wrong, process the input and destroy the whole note 
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Square (Start)]");
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
    }

    // This ignores accuracy of the first input
    private void checkInputSquareFinal(float inputTimeStamp)
    {
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        if (timeFromPerfect <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Square (End)]");
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
    }
    private void checkInputTriangle(float inputTimeStamp)
    {
        float requiredTimeStamp = triangleTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        if (timeFromPerfect <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Triangle]");
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentTimeStamp = Time.time - beatMapStartTime;

        // Missed note checks is based on the fact that a non-missed note would have already been dequeued,
        // and since it is not, it must be missed
        if (circleTimingsQueue.Count > 0 && currentTimeStamp > circleTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            DequeueNote(circleObjectsQueue, circleTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Circle]");
        }
        // Only a missed note if the first input has not yet be detected, because the note is not dequeued upon first input check
        if (squareTimingsQueue.Count > 0 && !initialSquareInput && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            // Prevent 2 notes being shown and causing confusion
            // Only the first note will pass the judgement line and signify the loss of 1 health
            squareObjectsQueue.Peek().GetComponent<NoteSquareScript>().DestroyNoteSquareEnd(); 
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Square (Start)]");
        }
        if (squareTimingsQueue.Count > 0 && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"] + expectedWindow)
        {
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Square (End)]");
        }
        if (triangleTimingsQueue.Count > 0 && currentTimeStamp > triangleTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Triangle]");
        }
    }
}
