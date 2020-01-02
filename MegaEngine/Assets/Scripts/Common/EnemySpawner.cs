using UnityEngine;
using UnityEngine.Assertions;
using Extensions;

/// <summary>
/// Spawns a GameObject when it becomes on screen
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // enemy to spawn
    public GameObject enemyPrefab;

    private bool isSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(enemyPrefab);
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
