using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite frozenSprite;
    [SerializeField] private Sprite corruptSprite;

    [SerializeField] private float timeToFreeze;
    [SerializeField] private float speed = 1f;

    #region Attack

    [SerializeField] private GameObject bullet;
    [SerializeField] private float attackRange;
    private float AttackRange;
    [SerializeField] private float attackTime;
    [SerializeField] private float attackSpeed;
    private float attackCountdown = 0f;
    private bool attackCardinal = false;
    
    void Shoot()
    {
        attackCountdown += Time.deltaTime;
        if (attackCountdown > attackTime)
        {
            if (attackCardinal)
            {
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>()
                    .Instantiate(Vector3.up, attackSpeed);
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>()
                    .Instantiate(Vector3.down, attackSpeed);
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>()
                    .Instantiate(Vector3.left, attackSpeed);
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>()
                    .Instantiate(Vector3.right, attackSpeed);
            }
            else
            {
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>().Instantiate(Vector3.up + Vector3.left, attackSpeed);
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>().Instantiate(Vector3.down + Vector3.left, attackSpeed);
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>().Instantiate(Vector3.up + Vector3.right, attackSpeed);
                Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>().Instantiate(Vector3.down + Vector3.right, attackSpeed);
            }

            attackCountdown = 0f;
            attackCardinal = !attackCardinal;
        }
    }

    #endregion
    
    private float freezeAmount = 0f;
    [NonSerialized] public bool corrupt = true;
    private bool frozen = false;

    private SpriteRenderer sp;
    
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        AttackRange = attackRange;
        StartCoroutine(nameof(Shoot));
    }

    // Update is called once per frame
    void Update()
    {
        
        if(GameManager.Paused) return;

        if (corrupt)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameManager.Player.transform.position);
            Vector3Int pos = GameManager.Tilemap.layoutGrid.WorldToCell(transform.position);
            float walkSpeed = speed * (GameManager.Tilemap.GetTile<MixableRuleTile>(pos) ? (GameManager.iceDangerous ? 0.5f : 1.5f) : 1f);

            if (distanceToPlayer > AttackRange)
            {
                transform.position += (GameManager.Player.transform.position - transform.position).normalized *
                                      Time.deltaTime * walkSpeed;
            }else if (Mathf.Abs(distanceToPlayer - AttackRange) < 0.01f)
            {
                AttackRange = attackRange - Random.Range(0f, 0.5f);
            }else
            {
                transform.position -= (GameManager.Player.transform.position - transform.position).normalized *
                                      Time.deltaTime * walkSpeed;
            }
            Shoot();
        }
        CheckFreeze();
    }

    void CheckFreeze()
    {
        Vector3Int pos = GameManager.Tilemap.layoutGrid.WorldToCell(transform.position);
        if (GameManager.Tilemap.GetTile<MixableRuleTile>(pos) && !frozen && GameManager.iceDangerous)
        {
            freezeAmount += Time.deltaTime;
        }
        else if(!GameManager.Tilemap.GetTile<MixableRuleTile>(pos) && frozen)
        {
            freezeAmount -= Time.deltaTime;
        }
        
        if (!frozen && freezeAmount > timeToFreeze)
        {
            sp.sprite = frozenSprite;
            frozen = true;
            corrupt = false;
        }
        else if(frozen && freezeAmount < 0)
        {
            sp.sprite = normalSprite;
            frozen = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(!corrupt) return;

        if (col.gameObject.GetComponent<PlayerController>())
        {
            GameManager.Player.Damage();
        }
    }
}
