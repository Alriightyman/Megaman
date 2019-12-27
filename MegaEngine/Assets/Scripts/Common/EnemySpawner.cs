using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;

    private bool isSpawned = false;
    private LevelCamera cam;

    // Start is called before the first frame update
    void Start()
    {

        Assert.IsNotNull(enemyPrefab);

        cam = Camera.main.GetComponent<LevelCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        bool onScreen = Camera.main.IsObjectOnScreen(transform);

        if(onScreen && isSpawned == false)
        {
            Instantiate(enemyPrefab, transform);
            isSpawned = true;
        }
        else if(!onScreen && isSpawned)
        {
            isSpawned = false;
        }
    }
}
