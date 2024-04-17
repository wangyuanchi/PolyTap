using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTriangleScript : MonoBehaviour
{

    public float timeSpawnToJudgement;
    public float noteSpeedTiming;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, 6f));
    }

    // Scale the note to go to (6, 6, 6), where it passes JudgementLineTriangle at (1, 1, 1) by timeSpawnToJudgement
    private IEnumerator ScaleOverTime(float timeSpawnToJudgement, float finalScale)
    {
        float defaultTimeSpawnToDestroy = noteSpeedTiming * finalScale;
        float elapsedTime = noteSpeedTiming - timeSpawnToJudgement;

        // Move preSpawned note to starting position if required
        if (elapsedTime > 0)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), elapsedTime / defaultTimeSpawnToDestroy);
        }

        while (elapsedTime < defaultTimeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), elapsedTime / defaultTimeSpawnToDestroy);
            yield return null;
        }

        Destroy(gameObject);
    }
}
