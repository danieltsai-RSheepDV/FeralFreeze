using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    enum States {
        IDLE,
        FREEZING,
        DASHING
    }

    private States curState = States.IDLE;
    
    #region Input
    
    private Inpactions inputs;

    private void OnEnable()
    {
        inputs = new Inpactions();
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
    
    #endregion

    #region Movement
    
    private Rigidbody2D rb;
    [Space]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] private float jumpDistance = 1f;
    
    private Vector3 dir;
    
    private Animator animator;

    void Move()
    {
        if (curState == States.IDLE || curState == States.FREEZING)
        {
            curState = inputs.Player.Freeze.inProgress ? States.FREEZING : States.IDLE;
            
            Vector2 inputVector = inputs.Player.Move.ReadValue<Vector2>();
            dir = new Vector3(inputVector.x, inputVector.y);
            footsteps.setPaused(curState == States.FREEZING || !inputs.Player.Move.inProgress);
            
            if (inputs.Player.Move.triggered)
            {
                animator.Play("Walk");

                if (dir.x != 0) animator.SetFloat("VX", dir.x > 0 ? 1 : -1);
            }
            else if (inputs.Player.Move.WasReleasedThisFrame())
            {
                dir = new Vector3(0, 0);

                animator.Play("Idle");
            }

            rb.velocity = dir * moveSpeed * (inputs.Player.Freeze.IsInProgress() ? 0.2f : 1);

            if (inputs.Player.Dash.triggered && curState == States.IDLE)
            {
                dash.start();
                curState = States.DASHING;
                Physics2D.IgnoreLayerCollision(6, 7, true);
                GenerateIceRadius(0);
                StartCoroutine(JumpTimer());
            }
        }else if (curState == States.DASHING)
        {
            rb.velocity = dir * moveSpeed * 2;
            
            animator.Play("Dash");
        }
    }

    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(jumpDistance / (moveSpeed * 2));
        curState = States.IDLE;
        animator.Play("Idle");
        GenerateIceRadius(1);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreLayerCollision(6, 7, false);
    }

    #endregion

    #region Icing
    
    private List<Vector3Int> iceOffsets = new();

    void GenerateIceRadius(int iceRadius)
    {
        iceOffsets = new();
        for (int i = -iceRadius; i <= iceRadius; i++)
        {
            for (int j = -iceRadius; j <= iceRadius; j++)
            {
                if (Mathf.Sqrt(i * i + j * j) <= iceRadius)
                {
                    iceOffsets.Add(new Vector3Int(i, j));
                }
            }
        }
    }

    void FreezeWater()
    {
        GameManager.SetIceDanger(inputs.Player.Freeze.inProgress);
        foreach (var offset in iceOffsets)
        {
            Vector3Int pos = GameManager.Tilemap.layoutGrid.WorldToCell(transform.position) + offset;
            TileBase tile = GameManager.Tilemap.GetTile(pos);
            
            if (tile == GameManager.WaterTile || tile == GameManager.IceTile)
            {
                GameManager.IceTracker[pos] = GameManager.TimeToMelt + Random.Range(-1f, 0f);
                GameManager.Tilemap.SetTile(pos, GameManager.IceTile);
            }
        }
    }

    #endregion

    #region Health

    [Space]
    [SerializeField] private SpriteRenderer sp;
    private bool vulnurable;
    private bool invincible;
    [SerializeField] private float iFrameTime = 3f;
    [SerializeField] private float recoveryTime = 10f;
    
    public void Damage()
    {
        if (vulnurable && !invincible)
        {
            SceneManager.LoadScene("DeathScene");
        }
        else if(!invincible)
        {
            vulnurable = true;
            invincible = true;
            GameManager.HealthBar.setImage(true);
            StartCoroutine(invincibilityTimer());
            StartCoroutine(recoveryTimer());
        }
    }

    IEnumerator invincibilityTimer()
    {
        for (int i = 0; i < iFrameTime / 0.25f; i++)
        {
            sp.color = new Color(0, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.125f);
            sp.color = new Color(0, 0, 0, 0.7f);
            yield return new WaitForSeconds(0.125f);
        }
        sp.color = Color.black;
        invincible = false;
    }

    IEnumerator recoveryTimer()
    {
        yield return new WaitForSeconds(recoveryTime);
        vulnurable = false;
        sp.color = Color.white;
        GameManager.HealthBar.setImage(false);
    }

    #endregion
    
    private EventInstance footsteps;
    private EventInstance dash;

    // Lifecycle
    void Start()
    {
        GenerateIceRadius(1);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/footstep");
        footsteps.setPaused(true);
        footsteps.start();

        dash = FMODUnity.RuntimeManager.CreateInstance("event:/loot");
    }

    void Update()
    {
        if(GameManager.Paused) return;
        
        Move();
        
        FreezeWater();
    }

    private void OnDestroy()
    {
        footsteps.stop(STOP_MODE.IMMEDIATE);
        footsteps.release();
    }
    
    
}
