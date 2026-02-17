using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System;//必须引入这个命名空间才能使用特性


public class TextDamagePopup : MonoBehaviour
{
    // -----------------------------------------
    // 特性解析 1：[InfoBox]
    // 效果：在面板最上方生成一个提示框。EInfoBoxType.Normal 是普通提示（蓝色），还有 Warning（黄色）和 Error（红色）。
    // 用途：提醒自己或协同开发者这个脚本的作用和注意事项。
    // -----------------------------------------
    [InfoBox("基于 LeanTween 的受击飘字控制器。\n会自动在动画结束后销毁自身。", EInfoBoxType.Normal)]

    [BoxGroup("核心组件引用")]
    [Required("必须绑定子物体的TextMeshPro组件!")]
    public TextMeshPro _textMesh;

    [BoxGroup("动画参数设置")]
    [Range(0.1f, 2.0f)]
    [Tooltip("飘字持续的总时间（秒）")]
    [SerializeField] float _floatDuration = 0.6f;

    [BoxGroup("动画参数设置")]
    [Range(0.5f, 5.0f)]//向上漂移的总距离
    [Tooltip("向上飘动的世界坐标距离")]
    [SerializeField] float _floatUpDistance = 1.5f;

    [BoxGroup("动画参数设置")]
    [Range(0f,2.0f)] //MinValue：特性解析
    [Tooltip("左右随机偏移的范围，避免多个数字完全重叠")]
    [SerializeField] float _randomOffset = 0.5f;

    private Action<TextDamagePopup> returnToPoolAction;

    public void SetUp(int damageAmount,Vector3 spawnPositon,Action<TextDamagePopup> action)
    {
       
        returnToPoolAction = action;
        LeanTween.cancel(gameObject);//防御性编程打断旧动画
        _textMesh.text= damageAmount.ToString();
        transform.position = spawnPositon;
        //重置透明度(防止服用时一开始就是透明的)
        Color  currentcolor = _textMesh.color;
        currentcolor.a = 1f;
        _textMesh.color = currentcolor;

        //计算动画的最终终点位置
        float randomX = UnityEngine.Random.Range(-_randomOffset, _randomOffset);
        //终点位置 = 出生坐标+Vector3(随机X偏移，向上移动的值，Z轴不动)
        Vector3 targetPosition = spawnPositon + new Vector3(randomX, _floatUpDistance, 0);
        /* ==========================================
         动画阶段1:向上移动
        * ========================================== */
        LeanTween.move(gameObject,targetPosition,_floatUpDistance).setEaseOutQuart();
        //动画阶段二:透明度渐隐与回收
        LeanTween.value(gameObject, 1f, 0f, _floatUpDistance).setEaseInExpo().setOnUpdate((float alpha) =>
        {
            Color c = _textMesh.color;
            c.a = alpha;
            _textMesh.color = c;
        }).setOnComplete(() =>
        {
            returnToPoolAction?.Invoke(this);
        });
    }
}
