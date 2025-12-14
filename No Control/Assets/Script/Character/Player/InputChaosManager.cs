using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputChaosManager : MonoBehaviour
{
    [Header("混乱设置")]
    public bool chaosOnStart = true;
    public bool chaosOnHurt = true;

    [Header("键位映射字典")]
    public Dictionary<string, Key> actionKeyMap = new Dictionary<string, Key>();

    private void Awake()
    {
        // 初始化默认按键
        actionKeyMap["MoveUp"] = Key.W;
        actionKeyMap["MoveDown"] = Key.S;
        actionKeyMap["MoveLeft"] = Key.A;
        actionKeyMap["MoveRight"] = Key.D;
        actionKeyMap["Attack"] = Key.Space;

        if (chaosOnStart)
            ShuffleKeys();
    }

    public Key GetKeyForAction(string action)
    {
        if (actionKeyMap.TryGetValue(action, out var key)) return key;
        return Key.None;
    }

    public void OnPlayerHurt()
    {
        if (!chaosOnHurt || actionKeyMap.Count < 2) return;

        // 随机交换两个键
        List<string> keys = new List<string>(actionKeyMap.Keys);
        int a = Random.Range(0, keys.Count);
        int b;
        do { b = Random.Range(0, keys.Count); } while (a == b);

        string actionA = keys[a];
        string actionB = keys[b];

        Key temp = actionKeyMap[actionA];
        actionKeyMap[actionA] = actionKeyMap[actionB];
        actionKeyMap[actionB] = temp;

        Debug.Log($"[Chaos] 键位交换：{actionA} ({actionKeyMap[actionA]}) ⇄ {actionB} ({actionKeyMap[actionB]})");
    }

    private void ShuffleKeys()
    {
        List<string> keys = new List<string>(actionKeyMap.Keys);
        for (int i = keys.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string a = keys[i], b = keys[j];
            Key temp = actionKeyMap[a];
            actionKeyMap[a] = actionKeyMap[b];
            actionKeyMap[b] = temp;
            Debug.Log($"[Chaos] 初始洗牌：{a} ⇄ {b}");
        }
    }
}
