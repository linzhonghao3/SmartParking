
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public Vector2 Margin, Smoothing;
    private Vector2 first = Vector2.zero;//鼠标第一次落下点
    private Vector2 second = Vector2.zero;//鼠标第二次位置（拖拽位置）
    private Vector3 vecPos = Vector3.zero;
    private bool IsNeedMove = false;//是否需要移动
    private void Awake()
    {
    }
    public void Start()
    {
        first.x = transform.position.x;//初始化
        first.y = transform.position.y;
    }

    public void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            //记录鼠标按下的位置 　　
            first = Event.current.mousePosition;
        }
        if (Event.current.type == EventType.MouseDrag)
        {
            //记录鼠标拖动的位置 　　
            second = Event.current.mousePosition;
            Vector3 fir = Camera.main.ScreenToWorldPoint(new Vector3(first.x, first.y, 0));//转换至世界坐标
            Vector3 sec = Camera.main.ScreenToWorldPoint(new Vector3(second.x, second.y, 0));
            vecPos = sec - fir;//需要移动的 向量 
            first = second;
            IsNeedMove = true;

        }
        else
        {
            IsNeedMove = false;
        }

    }
    public void Update()
    {
        if (IsNeedMove == false)
        {
            return;
        }
        var x = transform.position.x;
        x = x - vecPos.x;//向量偏移
        var z = transform.position.z;
        z = z-vecPos.z;
        //限制摄像机移动
        //x = Mathf.Clamp(x, -49, 50);
        transform.position = new Vector3(x, transform.position.y, z);

        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
           
            this.gameObject.GetComponent<Camera>().orthographicSize+=10;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
          
            this.gameObject.GetComponent<Camera>().orthographicSize+=10;
        }
    }
}

