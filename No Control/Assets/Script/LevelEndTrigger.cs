using UnityEngine;
using TMPro; // 如果用UGUI的TextMeshPro，没有则换成UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    [Header("过关提示配置")]
    public GameObject winTipPanel; // 过关提示面板（UGUI）
    public TextMeshProUGUI winTipText; // 提示文本（如果用普通Text则换成Text）
    public string winTipContent = "恭喜过关！";
    public float tipShowDelay = 0.5f; // 延迟显示提示（可选）

    // 确保终点的Collider2D是Trigger
    private void Awake()
    {
        Collider2D coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("终点对象缺少Collider2D组件！", this);
            enabled = false;
            return;
        }
        coll.isTrigger = true; // 强制设为触发模式
    }

    // 触发检测（只检测玩家）
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 替换成你的玩家标签（比如Player），确保玩家对象标签正确
        if (other.CompareTag("Player") && GameApp.Instance.State != GameApp.GameState.Win)
        {
            LevelWinLogic();
        }
    }

    // 过关核心逻辑
    private void LevelWinLogic()
    {
        // 1. 更新游戏状态为“过关”，停止怪物生成（适配你现有MobSpawner）
        GameApp.Instance.State = GameApp.GameState.Win;
        
        // 2. 停止时间（可选，冻结游戏画面）
        Time.timeScale = 0f;
        
        // 3. 显示过关提示（延迟显示更丝滑）
        Invoke(nameof(ShowWinTip), tipShowDelay);
        
        // 4. 可选：禁用玩家控制（根据你的玩家脚本调整）
        // 示例：如果玩家有PlayerController脚本，禁用它
        // PlayerController player = FindObjectOfType<PlayerController>();
        // if (player != null) player.enabled = false;
        
        // 5. 可选：停止怪物生成（双重保险）
        if (MobSpawner.Instance != null)
        {
            MobSpawner.Instance.enabled = false;
        }

        Debug.Log("游戏过关！");
    }

    // 显示过关提示
    private void ShowWinTip()
    {
        if (winTipPanel != null)
        {
            winTipPanel.SetActive(true);
        }
        if (winTipText != null)
        {
            winTipText.text = winTipContent;
        }
    }

    // 可选：重置游戏（比如点击按钮重新玩）
    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameApp.Instance.State = GameApp.GameState.Normal;
        if (MobSpawner.Instance != null)
        {
            MobSpawner.Instance.ResetSpawner();
            MobSpawner.Instance.enabled = true;
        }
        if (winTipPanel != null)
        {
            winTipPanel.SetActive(false);
        }
        // 可选：重置玩家状态、怪物等
    }
}