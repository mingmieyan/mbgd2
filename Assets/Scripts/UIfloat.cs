using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image 在这里

public class UIfloat : MonoBehaviour
{
    [Header("cuizhishousuo")]
    public float cuizhishousuov = 0;
    public float cuizhishousuoa = 0;
    public float cuizhishousuo = 0;

    [Header("ˮshuipingshousuo")]
    public float shuipingshousuov = 0;
    public float shuipingshousuoa = 0;
    public float shuipingshousuo = 0;

    [Header("cuizhixuanfu")]
    public float cuizhixuanfuv = 0;
    public float cuizhixuanfua = 0;
    public float cuizhixuanfu = 0;

    [Header("zuoyouzhuangdong")]
    public float zuoyouzhuangdongv = 0;
    public float zuoyouzhuangdonga = 0;
    public float zuoyouzhuangdong = 0;

    public float randomPhase = 0;//0�Ǹ����ֵ��1���������£�180�Ƿ�����
    //��Χ0 ��360
    [SerializeField]
    protected bool right = true;

    private SpriteRenderer sr;

    private RectTransform rectTransform;
    private Vector3 baseLocalPosition;//�����Ļ�׼λ��
    private Vector3 originLocalPosition;//��ʼλ��

    private float baseScaleX;
    private float baseScaleY;

   

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        baseLocalPosition = rectTransform.localPosition;
        baseScaleX = Mathf.Abs(rectTransform.localScale.x);
        baseScaleY = rectTransform.localScale.y;

        // 只在Awake时随机一次，不要每次OnEnable都随机
        randomPhase = Random.Range(0f, 1f);
    }

   

   
    private float GetValue(float speed, float phase)
    {
        // 用 Time.time 代替 fixedTime
        float angle = speed * Time.time;
        return -Mathf.Cos((angle + phase + randomPhase) * Mathf.Deg2Rad);
    }

    private void Update()
    {
        float v;
        float vvh = 1;
        float vvs = 1;

        // 收缩
        if (cuizhishousuo != 0 || shuipingshousuo != 0)
        {
            v = (GetValue(cuizhishousuov, cuizhishousuoa) + 1) / 2;
            vvs = v * cuizhishousuo + baseScaleY;

            v = (GetValue(shuipingshousuov, shuipingshousuoa) + 1) / 2;
            vvh = v * shuipingshousuo + baseScaleX;
        }

        vvh = right ? vvh : -vvh;
        rectTransform.localScale = new Vector3(vvh, vvs, 1);

        // 悬浮
        if (cuizhixuanfu != 0)
        {
            v = GetValue(cuizhixuanfuv, cuizhixuanfua);
            float vvf = v * cuizhixuanfu;
            rectTransform.localPosition = new Vector3(baseLocalPosition.x, baseLocalPosition.y + vvf, baseLocalPosition.z);
        }

        // 左右晃动
        if (zuoyouzhuangdong != 0)
        {
            v = GetValue(zuoyouzhuangdongv, zuoyouzhuangdonga);
            float rotz = v * zuoyouzhuangdong;
            rectTransform.localRotation = Quaternion.Euler(0, 0, rotz);
        }
    }
}