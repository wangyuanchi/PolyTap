using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementLinesScript : MonoBehaviour
{
    [SerializeField] private GameObject judgementLineCircle;
    [SerializeField] private GameObject judgementLineSquare;
    [SerializeField] private GameObject judgementLineTriangle;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeInJudgementLine(judgementLineCircle));
        StartCoroutine(FadeInJudgementLine(judgementLineSquare));
        StartCoroutine(FadeInJudgementLine(judgementLineTriangle));
    }

    private IEnumerator FadeInJudgementLine(GameObject judgementLine)
    {
        float time = 0f;
        float fadeDuration = 0.5f;
        SpriteRenderer spriteRenderer = judgementLine.GetComponent<SpriteRenderer>();

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            Color color = spriteRenderer.color;
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            color.a = alpha;
            spriteRenderer.color = color; 
            yield return null;
        }    
    }

    // Make the judgement lines drop with different gravity
    public void GameOver()
    {
        Rigidbody2D rigidBodyCircle = judgementLineCircle.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidBodySquare = judgementLineSquare.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidBodyTriangle = judgementLineTriangle.GetComponent<Rigidbody2D>();

        rigidBodyCircle.gravityScale = Random.Range(2f, 4f);
        rigidBodySquare.gravityScale = Random.Range(2f, 4f);
        rigidBodyTriangle.gravityScale = Random.Range(2f, 4f);
        rigidBodyCircle.simulated = true;
        rigidBodySquare.simulated = true;
        rigidBodyTriangle.simulated = true;
    }
}
