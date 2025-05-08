using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour
{
    [SerializeField] GameObject block_Pre;
    [SerializeField] GameObject accelerator_Pre;
    [SerializeField] GameObject chaser_Pre;
    [SerializeField] GameObject child_Pre;
    [SerializeField] TextMeshProUGUI text;


    const int size = 21;
    const int Wall = 1;
    const int Path = 0;
    private int[,] maze = new int[size, size];

    public int target_num;
    public int caught_num = 0;
    public float[] player_speed = new float[] {1.5f, 1.5f, 1.5f, 1.5f} ;
    private float time = 190f;
    private bool flag = false;
    public static int result = 0;

    void Start()
    {
        Generate_field();
        Set_Accelerator();
        target_num = Spawn_player();
    }



    //鬼のスポーン，終了判定
    void Update()
    {
        //UI表示
        time -= Time.deltaTime;
        text.text = $"Time\n{((int)(time / 60)).ToString("00")} : {((int)(time % 60)).ToString("00")}\nTarget\n{caught_num} / {target_num}";
        //鬼の生成
        if(!flag && time <= 180f)
        {
            flag = true ;
            Spawn_player();
        }
        //終了判定
        if(target_num <= caught_num)
        {
            SceneManager.LoadScene("ResultScene");
        }
        else if(time <= 0)
        {
            result = 1;
            SceneManager.LoadScene("ResultScene");
        }
    }


    
    //プレイヤー生成   
    int Spawn_player()
    {
        int cnt = 0;
        for (int i = 0; i < 4; i++)
        {
            var device = LobbySceneManager.joinedDevices[i];
            if (device != null)
            {
                if (cnt == LobbySceneManager.chaser_index && flag)
                {
                    var player = PlayerInput.Instantiate(prefab: chaser_Pre.gameObject, pairWithDevice: device);
                    player.transform.position = new Vector3(-4.5f, 4.5f, 0);
                    player = PlayerInput.Instantiate(prefab: chaser_Pre.gameObject, pairWithDevice: device);
                    player.transform.position = new Vector3(4.5f, -4.5f, 0);
                }
                else if(cnt != LobbySceneManager.chaser_index && !flag)
                {
                    int x1, x2, y1, y2;
                    while(true)
                    {
                        x1 = UnityEngine.Random.Range(0, 10);
                        y1 = UnityEngine.Random.Range(0, 21);
                        x2 = UnityEngine.Random.Range(10, 21);
                        y2 = UnityEngine.Random.Range(0, 21);
                        if (maze[x1, y1] == Path && maze[x2, y2] == Path)
                        {
                            maze[x1, y1] = Wall;
                            maze[x2, y2] = Wall;
                            break;
                        }
                    }
                    //生成
                    var player = PlayerInput.Instantiate(prefab: child_Pre.gameObject, pairWithDevice: device);
                    player.transform.position = new Vector3(x1 * 0.5f - 5, y1 * 0.5f - 5, 0);
                    //カラー変更
                    Renderer renderer = player.GetComponent<Renderer>();
                    renderer.material.color = LobbySceneManager.colors[i];
                    //ID割り振り
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    playerController.id = i;
                    //生成
                    player = PlayerInput.Instantiate(prefab: child_Pre.gameObject, pairWithDevice: device);
                    player.transform.position = new Vector3(x2 * 0.5f - 5, y2 * 0.5f - 5, 0);
                    //カラー変更
                    renderer = player.GetComponent<Renderer>();
                    renderer.material.color = LobbySceneManager.colors[i];
                    //ID割り振り
                    playerController = player.GetComponent<PlayerController>();
                    playerController.id = i;
                }
                cnt++;
            }
        }
        return cnt;
    }



    void Generate_field()
    {
        int rnd_select = UnityEngine.Random.Range(0, 3);
        maze = new int[size, size];

        Maze_extend();

        //迷路生成
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (maze[i, j] == Wall)
                {
                    GameObject wallBrock = Instantiate(block_Pre) as GameObject;
                    wallBrock.transform.position = new Vector3(i*0.5f - 5, j*0.5f - 5, 0);
                }
            }
        }
    }



    //壁伸ばし法
    void Maze_extend()
    {
        var list_wall = new List<Vector2Int>();
        var list_now = new List<Vector2Int>();

        //壁で囲う
        for (int i = 0; i < size; i++)
        {
            maze[i, 0] = 1;
            maze[i, size - 1] = Wall;
            maze[0, i] = 1;
            maze[size - 1, i] = Wall;
        }

        //リストアップ
        for (int i = 2; i < size - 1; i += 2)
        {
            for (int j = 2; j < size - 1; j += 2)
            {
                list_wall.Add(new Vector2Int(i, j));
            }
        }

        int start = UnityEngine.Random.Range(0, list_wall.Count);
        int start_x = list_wall[start].x;
        int start_y = list_wall[start].y;

        //壁を伸ばす
        Extend(start_x, start_y);

        //ループを作る
        Make_loop();


        void Extend(int x, int y)
        {
            var list_extend = new List<int>();
            if (maze[x, y] == Path)
            {
                list_now.Add(new Vector2Int(x, y));
                list_wall.Remove(new Vector2Int(x, y));
                maze[x, y] = Wall;
                //伸ばせる方向を探す
                if (!list_now.Contains(new Vector2Int(x + 2, y))) list_extend.Add(0);
                if (!list_now.Contains(new Vector2Int(x - 2, y))) list_extend.Add(1);
                if (!list_now.Contains(new Vector2Int(x, y + 2))) list_extend.Add(2);
                if (!list_now.Contains(new Vector2Int(x, y - 2))) list_extend.Add(3);

                //壁を伸ばす
                if (list_extend.Count > 0)
                {
                    int rnd_extend = UnityEngine.Random.Range(0, list_extend.Count);
                    switch (list_extend[rnd_extend])
                    {
                        case 0:
                            maze[x + 1, y] = Wall;
                            Extend(x + 2, y);
                            break;
                        case 1:
                            maze[x - 1, y] = Wall;
                            Extend(x - 2, y);
                            break;
                        case 2:
                            maze[x, y + 1] = Wall;
                            Extend(x, y + 2);
                            break;
                        case 3:
                            maze[x, y - 1] = Wall;
                            Extend(x, y - 2);
                            break;
                    }
                }
                else
                {
                    list_now.Clear();
                    if (list_wall.Count > 0)
                    {
                        int rnd_next = UnityEngine.Random.Range(0, list_wall.Count);
                        Extend(list_wall[rnd_next].x, list_wall[rnd_next].y);
                    }
                }
            }
            else
            {
                list_now.Clear();
                if (list_wall.Count > 0)
                {
                    int rnd_next = UnityEngine.Random.Range(0, list_wall.Count);
                    Extend(list_wall[rnd_next].x, list_wall[rnd_next].y);
                }
            }

        }
    }




    private void Make_loop()
    {
        for(int i = 1; i < size - 1; i++)
        {
            for(int j = 1; j < size - 1; j++)
            {
                if (maze[i, j] == Path)
                {
                    List<Vector2Int> list_wall = new List<Vector2Int>();
                    if (maze[i, j + 1] == Wall) list_wall.Add(new Vector2Int(i, j + 1));
                    if (maze[i, j - 1] == Wall) list_wall.Add(new Vector2Int(i, j - 1));
                    if (maze[i + 1, j] == Wall) list_wall.Add(new Vector2Int(i + 1, j));
                    if (maze[i - 1, j] == Wall) list_wall.Add(new Vector2Int(i - 1, j));
                    if(list_wall.Count == 3)
                    {
                        while (true)
                        {
                            int rnd = UnityEngine.Random.Range(0, 3);
                            if (list_wall[rnd].x != 0 && list_wall[rnd].x != size - 1 && list_wall[rnd].y != 0 && list_wall[rnd].y != size - 1)
                            {
                                maze[list_wall[rnd].x, list_wall[rnd].y] = Path;
                                break;
                            }
                        }
                    } 
                }
            }
        }
    }



    //ダッシュ板生成
    private void Set_Accelerator()
    {
        int length;
        List<Vector3Int> load_y = new List<Vector3Int>(), load_x = new List<Vector3Int>();
        //縦方向
        for(int i = 1; i < size - 1; i++)
        {
            length = 0;
            for (int j = 2; j < size; j++)
            {
                if (maze[i, j] == Path && maze[i-1, j] * maze[i+1, j] != 0)
                {
                    length += 1;
                }
                else
                {
                    if(length > 2) load_y.Add(new Vector3Int(i, j - 1, length));
                    length = 0;
                }
            }
        }


        //横方向
        for (int j = 1; j < size - 1; j++)
        {
            length = 0;
            for (int i = 2; i < size - 2; i++)
            {
                if (maze[i, j] == Path && maze[i, j + 1] * maze[i, j - 1] != 0)
                {
                    length += 1;
                }
                else
                {
                    if (length > 2) load_x.Add(new Vector3Int(i - 1, j, length));
                    length = 0;
                }
            }
        }


        //縦方向
        int max = load_y.Count / 2;
        for (int i = 0; i < max; i++)
        {
            int rnd = UnityEngine.Random.Range(0, load_y.Count);
            var v = load_y[rnd];
            load_y.RemoveAt(rnd);
            for (int j = 0; j < 3; j++)
            {
                GameObject ac_Pre = Instantiate(accelerator_Pre);
                ac_Pre.transform.position = new Vector3(v.x * 0.5f - 5, (v.y - j) * 0.5f - 5);
                Accelerator_Manager accelerator_Manager = ac_Pre.GetComponent<Accelerator_Manager>();
                accelerator_Manager.state = i % 2;
                accelerator_Manager.direction = new Vector2(0, 1);
                maze[v.x, v.y - j] = Wall;
            }
        }


        //横方向
        max = load_x.Count / 2;
        for (int i = 0; i < max; i++)
        {
            int rnd = UnityEngine.Random.Range(0, load_x.Count);
            var v = load_x[rnd];
            load_x.RemoveAt(rnd);
            for (int j = 0; j < 3; j++)
            {
                GameObject ac_Pre = Instantiate(accelerator_Pre);
                ac_Pre.transform.position = new Vector3((v.x - j) * 0.5f - 5, v.y * 0.5f - 5);
                ac_Pre.transform.rotation = Quaternion.Euler(0, 0, -90);
                Accelerator_Manager accelerator_Manager = ac_Pre.GetComponent<Accelerator_Manager>();
                accelerator_Manager.state = i % 2;
                accelerator_Manager.direction = new Vector2(1, 0);
                maze[v.x - j, v.y] = Wall;
            }
        }
    }
}
