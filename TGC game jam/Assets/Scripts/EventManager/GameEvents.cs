// GameEvents.cs
using UnityEngine; 

/// <summary>
/// 存放游戏中所有事件的名称常量。
/// 使用这些常量来注册、触发和移除事件
/// </summary>
public static class GameEvents
{
    #region 玩家相关事件 (Player Events)
    /// <summary> 玩家死亡事件 </summary>
    public const string PlayerDied = "PlayerDied";
    /// <summary> 玩家重生/生成事件 </summary>
    public const string PlayerSpawned = "PlayerSpawned";
    /// <summary> 玩家生命值改变事件。参数: (int currentHealth, int maxHealth) </summary>
    public const string PlayerHealthChanged = "PlayerHealthChanged";
    /// <summary> 玩家魔法值改变事件。参数: (int currentMana, int maxMana) </summary>
    public const string PlayerManaChanged = "PlayerManaChanged";
    /// <summary> 玩家升级事件。参数: (int newLevel) </summary>
    public const string PlayerLevelUp = "PlayerLevelUp";
    /// <summary> 玩家获得经验事件。参数: (int experienceGained) </summary>
    public const string PlayerGainedExperience = "PlayerGainedExperience";
    #endregion

    #region 分数与货币事件 (Score & Currency Events)
    /// <summary> 分数更新事件。参数: (int newTotalScore) </summary>
    public const string ScoreUpdated = "ScoreUpdated";
    /// <summary> 货币数量改变事件。参数: (string currencyID, int newAmount, int changeAmount) </summary>
    public const string CurrencyChanged = "CurrencyChanged";
    #endregion

    #region 敌人相关事件 (Enemy Events)
    /// <summary> 敌人生成事件。参数: (string enemyType, Vector3 spawnPosition) </summary>
    public const string EnemySpawned = "EnemySpawned";
    /// <summary> 敌人被击败事件。参数: (string enemyType, int scoreValue, Vector3 defeatPosition) </summary>
    public const string EnemyDefeated = "EnemyDefeated";
    /// <summary> Boss被击败事件。参数: (string bossName) </summary>
    public const string BossDefeated = "BossDefeated";
    #endregion

    #region 物品与库存事件 (Item & Inventory Events)
    /// <summary> 物品被收集事件。参数: (string itemID, int quantity) 或 (ItemData collectedItem) </summary>
    public const string ItemCollected = "ItemCollected";
    /// <summary> 物品被使用事件。参数: (string itemID) 或 (ItemData usedItem) </summary>
    public const string ItemUsed = "ItemUsed";
    /// <summary> 库存更新事件 (通常在物品增减后触发) </summary>
    public const string InventoryUpdated = "InventoryUpdated";
    #endregion

    #region UI 相关事件 (UI Events)
    /// <summary> 请求打开某个UI窗口事件。参数: (string windowName, object optionalData) </summary>
    public const string OpenWindowRequest = "OpenWindowRequest";
    /// <summary> 请求关闭某个UI窗口事件。参数: (string windowName) </summary>
    public const string CloseWindowRequest = "CloseWindowRequest";
    /// <summary> UI按钮点击事件。参数: (string buttonIdentifier) </summary>
    public const string ButtonClicked = "ButtonClicked";
    /// <summary> 提示信息显示请求。参数: (string message, float duration) </summary>
    public const string ShowNotification = "ShowNotification";
    #endregion

    #region 游戏状态与流程事件 (Game State & Flow Events)
    /// <summary> 游戏暂停事件 </summary>
    public const string GamePaused = "GamePaused";
    /// <summary> 游戏恢复事件 </summary>
    public const string GameResumed = "GameResumed";
    /// <summary> 游戏结束事件 </summary>
    public const string GameOver = "GameOver";
    /// <summary> 游戏胜利事件 </summary>
    public const string GameWon = "GameWon";
    /// <summary> 场景加载开始事件。参数: (string sceneName) </summary>
    public const string SceneLoadStart = "SceneLoadStart";
    /// <summary> 场景加载完成事件。参数: (string sceneName) </summary>
    public const string SceneLoadComplete = "SceneLoadComplete";
    /// <summary> 游戏初始化完成事件 </summary>
    public const string GameInitialized = "GameInitialized";
    #endregion

    #region 对话系统事件 (Dialogue Events)
    public const string ShowDialogue = "StartDialogue"; // 进行对话

    #endregion
    
    
    #region 摄像机系统事件 (Camera Events)
    /// <summary> 移动摄像机到目的地。参数: (Vector aimPos) </summary>
    public const string MoveCameraToPos = "MoveCameraToPos";
    
    /// <summary> 开始传送。</summary>
    public const string SwitchScene = "SwitchScene";
    
    #endregion
    
    #region 游戏剧情事件 (GamePlot Events)
    /// <summary> 游戏开始，玩家醒来。</summary>
    public const string GameStartsPlayerWakesUp = "GameStartsPlayerWakesUp";
    
    /// <summary> 正常玩家醒来。</summary>
    public const string PlayerWakesUp = "PlayerWakesUp";
    
    /// <summary> 玩家休眠。</summary>
    public const string PlayerSleep = "PlayerSleep";
    
    
    /// <summary> 一天过去。</summary>
    public const string PlayerSleepAndDayChange = "PlayerSleepAndDayChange";
    
    /// <summary> 一天到来。</summary>
    public const string DayChangeAndPlayerWakeUp = "DayChangeAndPlayerWakeUp";

    #endregion
    
}