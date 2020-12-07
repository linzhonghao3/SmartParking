using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    //坐标
    public int x;
    public int y;
    //是否是障碍物
    public bool block = false;
    //距离公式 f=g（离起点距离）+h（离终点距离）
    public float g;
    public float h;
    public float f;
    //父对象
    public AStarNode fatherNode;
    public AStarNode(int x, int y, bool block)
    {
        this.x = x;
        this.y = y;
        this.block = block;
    }
}
