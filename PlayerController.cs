using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Vector2 velocity;
    private new Rigidbody2D rigidbody2D;
    private Canvas canvas;


    private GameObject manager;
    private GameSceneManager gameSceneManager;
    [SerializeField] GameObject label_Pre;
    private GameObject label;
    TextMeshProUGUI text;

    public int id;
    private int state;
    public Vector2 buff;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        manager = GameObject.Find("SceneManager");
        gameSceneManager = manager.GetComponent<GameSceneManager>();
    }


    private void OnEnable()
    {
        //ラベル生成
        label = Instantiate(label_Pre, canvas.transform);
        text = label.GetComponent<TextMeshProUGUI>();

        GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        //ステータス(左右)判別
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == gameObject.name)
            {
                count++;
            }
        }
        state = (count + 1) % 2;

        if (state == 0)
        {
            playerInput.actions["Move"].performed += OnMovePerformed;
            text.text = "L";
        }
        else
        {
            playerInput.actions["Move2"].performed += OnMovePerformed;
            text.text = "R";
        }
        //if (gameObject.name == "Chaser_Pre(Clone)") speed = 1.5f - 0.5f * (1 - gameSceneManager.caught_num / gameSceneManager.target_num);
        if (gameObject.name == "Chaser_Pre(Clone)") gameSceneManager.player_speed[id] = 1.2f; 
    }

    private void OnDisable()
    {
        if(state == 0)
        {
            playerInput.actions["Move"].performed -= OnMovePerformed;
        }
        else
        {
            playerInput.actions["Move2"].performed -= OnMovePerformed;
        }
    }



    //キャラ操作
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        velocity = context.ReadValue<Vector2>();
    }


    private void Update()
    {
        //キャラ移動
        if(Math.Abs(velocity.y) == 1 && (Math.Abs(transform.position.x) % 0.5 > 0.45 || Math.Abs(transform.position.x) % 0.5 < 0.05))
        {
            rigidbody2D.linearVelocity = (velocity + buff) * gameSceneManager.player_speed[id];
            transform.rotation = Quaternion.Euler(0, 0,(1-velocity.y) * 90);
        }
        if (Math.Abs(velocity.x) == 1 && (Math.Abs(transform.position.y) % 0.5 > 0.45 || Math.Abs(transform.position.y) % 0.5 < 0.05))
        {
            rigidbody2D.linearVelocity = (velocity + buff) * gameSceneManager.player_speed[id];
            transform.rotation = Quaternion.Euler(0, 0, -velocity.x * 90);
        }
        //ラベル移動
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        text.rectTransform.position = screenPoint;
    }




    //追突時判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == gameObject.name)
        {
            velocity = velocity * -1;
        }
        else if(collision.gameObject.name == "Chaser_Pre(Clone)")
        {
            gameSceneManager.caught_num += 1;
            //speed = 1.5f - 0.5f * (1 - gameSceneManager.caught_num / gameSceneManager.target_num);
            gameSceneManager.player_speed[id] = 1.2f;
            Destroy(text);
            Destroy(gameObject);
        }
    }
}
