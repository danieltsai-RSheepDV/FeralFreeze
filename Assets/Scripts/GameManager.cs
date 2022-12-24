using System;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    
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
    
    [Space]
    [SerializeField] private PlayerController player;
        public static PlayerController Player;
    [SerializeField] private Tilemap tilemap;
        public static Tilemap Tilemap;
        
    [Space]
    [SerializeField] private HealthBarUI healthBar;
        public static HealthBarUI HealthBar;
    [SerializeField] private PauseMenuUI pauseMenu;
        public static PauseMenuUI PauseMenu;
    [SerializeField] private TextMeshProUGUI scoreUI;
        
    
    [Space]
    [SerializeField] private Material normalIce;
        public static Material NormalIce;
    [SerializeField] private Material spikedIce;
        public static Material SpikedIce;
    [SerializeField] private TileBase waterTile;
        public static TileBase WaterTile;
    [SerializeField] private TileBase iceTile;
        public static TileBase IceTile;
    [SerializeField] private float timeToMelt;
        public static float TimeToMelt;
        public static Dictionary<Vector3Int, float> IceTracker = new();
    public static bool iceDangerous = false;

    [NonSerialized] public static bool Paused = false;

    [SerializeField] private GameObject enemy;
    private List<GameObject> enemies = new();
    
    private EventInstance music;

    public static int score = 0;

    void Awake()
    {
        Player = player;
        Tilemap = tilemap;
        
        HealthBar = healthBar;
        PauseMenu = pauseMenu;
        
        TimeToMelt = timeToMelt;
        IceTile = iceTile;
        SpikedIce = spikedIce;
        NormalIce = normalIce;
        WaterTile = waterTile;
        
        music = FMODUnity.RuntimeManager.CreateInstance("event:/inGame");
        music.start();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputs.Player.Pause.triggered)
        {
            Paused = true;
            PauseMenu.gameObject.SetActive(true);
        }
        
        if(Paused) return;

        scoreUI.text = score.ToString("000000.##");

        foreach (var key in new List<Vector3Int>(IceTracker.Keys))
        {
            if (IceTracker[key] < 0)
            {
                Tilemap.SetTile(key, waterTile);
                IceTracker.Remove(key);
            }
            else
            {
                IceTracker[key] -= Time.deltaTime;
            }
        }

        enemies.RemoveAll(v => v.GetComponent<SlimeController>().corrupt == false);
        if (enemies.Count < 3)
        {
            for (int i = enemies.Count; i < 3; i++)
            {
                enemies.Add(Instantiate(enemy, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 15, Quaternion.identity));
            }
        }
    }

    public static void SetIceDanger(bool b)
    {
        iceDangerous = b;
        Tilemap.GetComponent<TilemapRenderer>().material = b ? SpikedIce : NormalIce;
    }

    private void OnDestroy()
    {
        music.stop(STOP_MODE.IMMEDIATE);
        music.release();
    }
}
