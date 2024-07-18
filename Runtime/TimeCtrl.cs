using UnityEngine;

/**
 * 可能需要控制 time scale 的对象有：
 * - 运动
 * - 动画
 * - 粒子系统
 * 这个组件能提供一个局部的 timeScale。这个 timeScale 可能是由多个 scale 混合而成，具体而言：
 * - 有些 buff 导致的减速
 * - 有些玩法导致的暂停，比如 hitstop / hitlag
 * - 有些世界状态导致的暂停，比如入场特写
 */
[DefaultExecutionOrder(-1000)]
public class TimeCtrl : MonoBehaviour
{
    /**
     * Time.timeScale 负责系统级别的暂停，WorldTimeScale 负责玩法级别的控制
     * 举几个例子
     * - 系统暂停：Time.timeScale 设置为 0
     * - 调试：Time.timeScale 设置为 0.5
     * - 子弹时间：TimeCtrl.WorldTimeScale 设置为 0.5
     * - 特写：WorldTimeScale 设置为 0，然后打开特写 UI 的 Isolated
     * - 玩家加速：WorldTimeScale 设置为 0.5，然后打开玩家控制角色的 Isolated。此时 LocalTimeScale 留给 buff 用
     * - 敌人减速：仅设置目标敌人的 LocalTimeScale
     */
    public static float WorldTimeScale = 1f;
    public float LocalTimeScale = 1f;
    public bool Isolated;

    public static float GetGameplayTimeScale(Component component)
    {
        if (component && component.TryGetComponent(out TimeCtrl ctrl))
        {
            if (ctrl.Isolated)
            {
                return ctrl.LocalTimeScale;
            }
            else
            {
                return ctrl.LocalTimeScale * WorldTimeScale;
            }
        }
        else
        {
            return WorldTimeScale;
        }
    }
}