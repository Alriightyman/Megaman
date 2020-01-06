using Prime31;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    public enum ItemType
    {
        None = -1,
        LargeHealth = 0,
        SmallHealth = 1,
        LargeWeapon = 2,
        SmallWeapon = 3,
        OneUp = 4,
        Energy = 5
    }

    public Color Color { get; set; } = new Color(1f, 1f, 1f);

    private CharacterController2D charController;
    private Animator animator = null;
    private SpriteRenderer spriteRenderer;
    public ItemType item = ItemType.None;
    private bool blink = false;

    [SerializeField] float gravity = 118f;
    [SerializeField] float smallAmout = 2f;
    [SerializeField] float LargeAmout = 10f;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // if this isn't a hardcoded item then generate a random item
        if(item == ItemType.None)
        {
            int value = UnityEngine.Random.Range(0, 128);
            
            if(value == 1) // one chance
            {
                animator.Play("OneUp");
                item = ItemType.OneUp;
            }
            else if(value >= 2 && value <=5) // 4 chances
            {
                animator.Play("LargeHealth");
                item = ItemType.LargeHealth;
            }
            else if(value >= 6 && value <= 10)
            {
                animator.Play("LargeWeapon");
                item = ItemType.LargeWeapon;
            }
            else if(value >= 11 && value <= 25)
            {
                animator.Play("SmallHealth");
                item = ItemType.SmallHealth;
            }
            else if(value >= 26 && value <= 50)
            {
                animator.Play("SmallWeapon");
                item = ItemType.SmallWeapon;
            }
            else
            {
                item = ItemType.None;
            }

            StartCoroutine("LifeSpan");
        }
        else if(item == ItemType.Energy)
        {
            animator.Play("Energy");
        }
        else if(item == ItemType.OneUp)
        {
            animator.Play("OneUp");
        }

        spriteRenderer.enabled = true;

        Debug.Log(String.Format("Item: {0}", item));

        spriteRenderer.color = Color;


    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

        SetAnimation();
    }

    /// <summary>
    /// Make Object fall
    /// </summary>
    private void ApplyGravity()
    {
        float speed = -gravity;
        if(charController.isGrounded)
        {
            speed = 0f;
        }

        var movement = new Vector3(0f, speed * Time.deltaTime, 0f);

        charController.move(movement);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log(String.Format("Picked up {0}", item));

            switch (item)
            {
                case ItemType.Energy:

                    break;
                case ItemType.LargeHealth:
                    GameEngine.Player.AddHealth(LargeAmout);
                    break;
                case ItemType.LargeWeapon:

                    break;
                case ItemType.OneUp:

                    break;
                case ItemType.SmallHealth:
                    GameEngine.Player.AddHealth(smallAmout);
                    break;
                case ItemType.SmallWeapon:

                    break;
            }

            Destroy(gameObject);
        }
    }



    private void SetAnimation()
    {
        switch (item)
        {
            // if no item, destory it
            case ItemType.None:
                Destroy(gameObject);
                break;
            case ItemType.Energy:
                animator.Play("Energy");
                break;
            case ItemType.LargeHealth:
                animator.Play("LargeHealth");
                break;
            case ItemType.LargeWeapon:
                animator.Play("LargeWeapon");
                break;
            case ItemType.OneUp:
                animator.Play("OneUp");
                break;
            case ItemType.SmallHealth:
                animator.Play("SmallHealth");
                break;
            case ItemType.SmallWeapon:
                animator.Play("SmallWeapon");
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }

    private IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine("Flash");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private IEnumerator Flash()
    {
        while(true)
        {
           spriteRenderer.color = new Color(Color.r, Color.g, Color.b, Color.a == 0f ? 1f : 0f);
           yield return new WaitForSeconds(0.1f);
        }
    }
}
