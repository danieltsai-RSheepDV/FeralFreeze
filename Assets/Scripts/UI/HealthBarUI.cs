using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Sprite fullHealth;
    [SerializeField] private Sprite lowHealth;

    private Image sp;
    
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setImage(bool low)
    {
        sp.sprite = low ? lowHealth : fullHealth;
    }
}
