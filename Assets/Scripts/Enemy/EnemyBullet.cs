using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 1f;
    private Vector3 dir;
    
    // Start is called before the first frame update
    public void Instantiate(Vector3 d, float s)
    {
        dir = d;
        speed = s;
    }
    
    void Start()
    {
        StartCoroutine(delete());
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Paused) return;
        
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerController>())
        {
            GameManager.Player.Damage();
            Destroy(gameObject);
        }
    }

    IEnumerator delete()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
