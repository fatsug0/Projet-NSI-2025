using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class XpBehaviour : MonoBehaviour
{
    public int xpValue;
    [SerializeField] private float xpSizeCoef;
    [SerializeField] private float xpExplosionForce;
    [SerializeField] private float stopTime = 1.5f; // Time before stopping completely
    [SerializeField] private float dragIncreaseRate = 2f; // Rate at which drag increases
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 randomExplosionVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.AddForce(randomExplosionVector * Random.Range(1, xpExplosionForce), ForceMode2D.Impulse);
        
        xpValue = Random.Range(1, xpValue);
        transform.localScale = new Vector3(
            transform.localScale.x * xpValue * xpSizeCoef,
            transform.localScale.y * xpValue * xpSizeCoef, 
            transform.localScale.z);
        
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = (transform.localScale.x * xpValue * xpSizeCoef + 0.1f) / 2;
        
        StartCoroutine(SlowDownAndStop());
    }
    
    private IEnumerator SlowDownAndStop()
    {
        float timer = 0f;
        while (timer < stopTime)
        {
            rb.linearDamping += dragIncreaseRate * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}