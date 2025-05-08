using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    public PlayerInputManager playerInputManager;
    [SerializeField] private InputAction playerAction;




    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerAction.Enable();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int cnt = 0;
        for (int i = 0; i < 4; i++)
        {
            if (LobbySceneManager.joinedDevices[i] != null)
            {
                if (LobbySceneManager.chaser_index == cnt && GameSceneManager.result == 0)
                {
                    text.text = $"Player {i}\n";
                }
                else if (LobbySceneManager.chaser_index != cnt && GameSceneManager.result == 1)
                {
                    text.text += $"Player {i}\n";
                }
                cnt++;
            }
        }
        text.text += "Win !!!";
    }



    void OnEnable()
    {
        if (playerInputManager != null)
        {
            playerAction.performed += OnAny;
        }
    }




    void OnDisable()
    {
        if (playerInputManager != null)
        {
            playerAction.performed -= OnAny;
            playerAction.Dispose();
        }
    }



    private void OnAny(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
