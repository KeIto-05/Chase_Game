using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class LobbySceneManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    [SerializeField] private InputAction playerJoinInputAction;
    public static int chaser_index = 0; //誰が鬼か
    public int player_num; //人数管理
    [SerializeField] GameObject player_Pre;
    [SerializeField] private GameObject[] marker_Pre = new GameObject[2];
    public static InputDevice[] joinedDevices = new InputDevice[4]; //プレイヤー：デバイス紐づけ
    public List<int> player_id = new List<int> {0, 1, 2, 3}; //空き番管理
    public static Color[] colors = new Color[] {Color.red , Color.cyan, Color.magenta, new Color(1f, 0.5f, 0f)}; //プレイヤーカラー



    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerJoinInputAction.Enable();
    }



    void OnEnable()
    {
        if (playerInputManager != null)
        {
            playerJoinInputAction.performed += OnJoin;
        }
    }




    void OnDisable()
    {
        if (playerInputManager != null)
        {
            playerJoinInputAction.performed -= OnJoin;
            playerJoinInputAction.Dispose();
        }
    }


    //プレイヤー入室
    private void OnJoin(InputAction.CallbackContext context)
    {
        if(player_num >= 4)
        {
            return;
        }

        foreach (var device in joinedDevices)
        {
            if(context.control.device == device)
            {
                return;
            }
        }

        int index = player_id.Min();
        player_id.Remove(index);
        joinedDevices[index] = context.control.device;
        PlayerInput.Instantiate(prefab: player_Pre.gameObject, playerIndex: index, pairWithDevice: context.control.device);

        player_num++;
        Marking();
    }



    //マーカー付与
    public void Marking()
    {
        int cnt = 0;

        GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "marker0_Pre(Clone)" || obj.name == "marker1_Pre(Clone)")
            {
                Destroy(obj);
            }
        }


        for (int i = 0; i < 4; i++)
        {
            var device = joinedDevices[i];
            if (device != null)
            {
                if (cnt == chaser_index)
                {
                    var C_marker = Instantiate(marker_Pre[0]);
                    C_marker.transform.position = new Vector3(7.5f, 3f - i * 1.9f, 0);
                }
                else
                {
                    var marker = Instantiate(marker_Pre[1]);
                    marker.transform.position = new Vector3(7.5f, 3f - i * 1.9f, 0);
                    Renderer renderer = marker.GetComponent<Renderer>();
                    renderer.material.color = colors[i];
                }
                cnt++;
            }
        }
    }
}
