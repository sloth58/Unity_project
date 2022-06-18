using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// automatically assigns components "Rigidbody2D" , "SpriteRenderer" and "Animator" to the object this script is attached to.
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]

public class CharacterController : MonoBehaviour
{
    //Event
    [SerializeField]
    UnityEvent OnDeathEvent;

    [SerializeField]
    UnityEvent OnHitEvent;

    [SerializeField]
    UnityEvent OnCollectedEvent;

    AudioSource source;
    Rigidbody2D rigid; // variable rigid of type Rigidbody2D used to store value
    SpriteRenderer render;// variable render of type SpriteRenderer used to store value
    Animator anim; // variable anim of type Animator used to store value

    float horizantal = 0; // horizontal is initialized to 0 therefore object is still

    bool grounded = false; // grounded is initialized to false until object makes contact with ground

    bool attacking = false;

    [SerializeField]
    private List<AudioClip> jump_clip = new List<AudioClip>();

    [SerializeField]
    private AudioClip attack_clip;

    [SerializeField]
    private AudioClip land_clip;

    [SerializeField]
    private List<AudioClip> hit_clip = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> walk_clip = new List<AudioClip>();

    [SerializeField] // allows "speed_factor" , "jump_factor" and "attack_key" values to be manipulated and changed
    private float speed_factor = 3; // multiply the velocity's speed on x axis

    [SerializeField]
    private float jump_factor = 10; // multiply the velocitys speed on y axis

    [SerializeField]
    private KeyCode attack_key = KeyCode.F; // attack key

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.TryGetComponent<AudioSource>(out AudioSource audio_source))
        {
            source = audio_source;
        }
        else
        {
           source = gameObject.AddComponent<AudioSource>();
        }


        rigid = gameObject.GetComponent<Rigidbody2D>(); // assigns "rigid" the component "Rigidbody2D" in object
        render = gameObject.GetComponent<SpriteRenderer>(); // assigns "render" the component "SpriteRenderer" in object
        anim = gameObject.GetComponent<Animator>(); // assigns "anim" the component "Animator" in object
    }

    private void Update()
    {
        horizantal = Input.GetAxisRaw("Horizontal"); // assign horizontal the horizontal value when moving horizontally

        if (horizantal != 0 && grounded) // check to see if horizontal = 0 and if the object is grounded ie touches the ground
        {
            render.flipX = horizantal <= 0;

            anim.SetBool("is_walking", true);

            source.volume = 0.25f;
            int range = Random.Range(0, 4);
            
            if (!source.isPlaying)
            {
                source.clip = walk_clip[range];
                source.pitch = 0.6f;
                source.Play();
            }
        }
        else
        {
            anim.SetBool("is_walking", false);
            source.pitch = 1.0f;
        }

        if (Input.GetKeyDown(attack_key)) // check to see if attack key is pressed and executes code if so
        {
            attacking = true;
            anim.SetBool("is_attacking", attacking);

        }



        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && grounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jump_factor);

            anim.SetBool("is_jumping", true);

            int range = Random.Range(0, 3);
            source.clip = jump_clip[range];
            source.volume = 0.5f;
            source.Play();

        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
      
        if(grounded)
        rigid.velocity = new Vector2(horizantal * speed_factor, rigid.velocity.y);

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("ground")) // if object collides with the ground than ground becomes true
        {
            grounded = true;

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("ground")) // if object collides with the ground than ground becomes true
        {
            transform.rotation = collision.transform.rotation;


            anim.SetBool("is_jumping", false);

            source.clip = land_clip;
            source.volume = 0.25f;
            source.Play();
        }


        if (collision.transform.CompareTag("enemy"))
        {
            Vector2 diff = transform.position - collision.transform.position;

            float direction = 1;

            if (render.flipX)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }


            if (Vector2.Dot(transform.right.normalized * direction, diff.normalized) < -0.8)
            {

                if (attacking)
                {
                    source.clip = attack_clip;
                    source.volume = 0.25f;
                    source.Play();

                    Destroy(collision.gameObject);
                    //damage enemy
                }
                else
                {
                    int range = Random.Range(0, 4);
                    source.clip = hit_clip[range];
                    source.volume = 0.5f;
                    source.pitch = 1.0f;
                    source.Play();
                    OnHitEvent.Invoke();
                    //damage player
                }
            }
            else
            {
                int range = Random.Range(0, 4);
                source.clip = hit_clip[range];
                source.volume = 0.5f;
                source.pitch = 1.0f;
                source.Play();
                OnHitEvent.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("death_zone"))
        {
            OnDeathEvent.Invoke();
        }


        if (collision.transform.CompareTag("coin"))
        {
            Destroy(collision.gameObject);
            OnCollectedEvent.Invoke();
        }
    }

    private void OnCollisionExit2D(Collision2D collision) // if object moves away from the ground ie jumps then grounded = false and therefore 
        // cant move in the air until they touch the ground again
    {
        if (collision.transform.CompareTag("ground"))
        {
            grounded = false;
        }
    }

    public void end_attack()
    {
        attacking = false;
        anim.SetBool("is_attacking", attacking);
    }
}
