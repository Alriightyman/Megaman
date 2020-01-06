using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class REmovemen : MonoBehaviour
{
    public static REmovemen i;
    // Start is called before the first frame update
    void Start()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        if (Input.GetButtonDown("Debug"))
            GetComponent<Health>().AddHealth(0f);
    }
}
