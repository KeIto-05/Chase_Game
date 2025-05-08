using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyCntroller : MonoBehaviour
{
    private PlayerInput playerInput;
    private LobbySceneManager lobbySceneManager;
    private TextMeshProUGUI tmp;
    private Canvas canvas;
    private GameObject text;
    [SerializeField] GameObject lobby_text_Pre; 


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        GameObject manager = GameObject.Find("SceneManager");
        lobbySceneManager = manager.GetComponent<LobbySceneManager>();
        canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        Destroy(text);
        //text = Instantiate(lobby_text_Pre, new Vector3(390, 438 - (438 * (playerInput.playerIndex + 1) / 5), 0), Quaternion.identity, canvas.transform);
        text = Instantiate(lobby_text_Pre, Camera.main.WorldToScreenPoint(new Vector3(0, 3f - playerInput.playerIndex * 1.9f, 0)), Quaternion.identity, canvas.transform);
        tmp = text.GetComponent<TextMeshProUGUI>();
        tmp.text = $"Player {playerInput.playerIndex + 1} : {LobbySceneManager.joinedDevices[playerInput.playerIndex].name}";
    }


    private void OnEnable()
    {
        playerInput.actions["Start"].performed += OnStartPerformed;
        playerInput.actions["Leave"].performed += OnLeavePerformed;
        playerInput.actions["Select"].started += OnSelectStarted;
    }

    private void OnDisable()
    {
        playerInput.actions["Start"].performed -= OnStartPerformed;
        playerInput.actions["Leave"].performed -= OnLeavePerformed;
        playerInput.actions["Select"].started -= OnSelectStarted;
    }

    private void OnStartPerformed(InputAction.CallbackContext context)
    {
        if(lobbySceneManager.player_num > 1)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private void OnLeavePerformed(InputAction.CallbackContext context)
    {
        LobbySceneManager.chaser_index = 0;
        lobbySceneManager.player_num -= 1;
        LobbySceneManager.joinedDevices[playerInput.playerIndex] = null;
        lobbySceneManager.player_id.Add(playerInput.playerIndex);
        lobbySceneManager.Marking();
        Destroy(text);
        Destroy(gameObject);
    }

    private void OnSelectStarted(InputAction.CallbackContext context)
    {
        LobbySceneManager.chaser_index = MOD((LobbySceneManager.chaser_index + -1 * (int)context.ReadValue<Vector2>().y), lobbySceneManager.player_num);
        lobbySceneManager.Marking();

        int MOD(int a, int n)
        {
            return ((a % n) + n) % n; 
        }
    }
}
