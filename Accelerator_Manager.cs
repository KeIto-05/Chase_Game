using UnityEngine;

public class Accelerator_Manager : MonoBehaviour
{
    public int state;
    public Vector2 direction;
    private Color[] colors = new Color[] {Color.green, new Color(1f, 0.8627f, 0f) }; //カラー




    void Start()
    {
        //カラー変更
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = colors[state];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Child_Pre(Clone)" && state == 0)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.buff = direction * 0.3f;
        }
        else if(collision.gameObject.name == "Chaser_Pre(Clone)" && state == 1)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.buff = direction * 0.3f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Child_Pre(Clone)" || collision.gameObject.name == "Chaser_Pre(Clone)")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.buff = Vector2.zero;
        }
    }
}
