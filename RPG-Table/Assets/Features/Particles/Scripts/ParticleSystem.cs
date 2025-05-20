using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ParticleSys : MonoBehaviour
{
    public bool Adventage = false;
    public bool Disadvantage = false;
    public bool Crawl = false;
    public bool Overwatch = false;
    public bool Block = false;
    public bool Dead = false;
    public bool Stunned = false;

    //add
    public bool Stealth = false;
    public bool Cover = false;


    // ===========================================

    private GameObject ParticleInstance, advantage, disadvantage, crawl, overwatch, block, dead, stunned;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.gameObject.name);
        Transform obj = transform.Find("ParticleInstance");
        ParticleInstance = obj.gameObject;
        advantage = ParticleInstance.transform.Find("Adv").gameObject;
        disadvantage = ParticleInstance.transform.Find("Disadv").gameObject;
        crawl = ParticleInstance.transform.Find("Crawl").gameObject;
        overwatch = ParticleInstance.transform.Find("Overwatch").gameObject;
        block = ParticleInstance.transform.Find("Block").gameObject;
        dead = ParticleInstance.transform.Find("Dead").gameObject;
        stunned = ParticleInstance.transform.Find("Stunned").gameObject;
    }

    // Update is called once per frame
    void Update()
    {   
        advantage.SetActive(Adventage);
        disadvantage.SetActive(Disadvantage);
        crawl.SetActive(Crawl);
        overwatch.SetActive(Overwatch);
        block.SetActive(Block);
        dead.SetActive(Dead);
        stunned.SetActive(Stunned);
    }
}
