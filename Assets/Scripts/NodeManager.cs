using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager
{
    //单例
    private static NodeManager instance;
    private NodeManager()
    {
    }

    public static NodeManager GetInstance()
    {
        if (instance == null)
        {
            instance = new NodeManager();
        }
        return instance;
    }

    public AStarNode[,] nodes = new AStarNode[21, 21];
    private List<AStarNode> openList = new List<AStarNode>();
    private List<AStarNode> closeList = new List<AStarNode>();
    private int mapHeight = 600;
    private int mapWidth = 600;
    private int[] direction = { -1, 0, 1 };

    //初始化地图
    public void Initialization(int mapHeight=600, int mapWidth=600)
    {
        //初始化一个mapHeight*mapWidth大小的地图
        this.mapHeight = mapHeight/30;
        this.mapWidth = mapWidth/30;
        
        //把全部方块设置成障碍物
        for (int i = 0; i <= this.mapHeight; i++)
        {
            for (int j = 0; j <= this.mapWidth; j++)
            {
                AStarNode node = new AStarNode(i, j, true);
                nodes[i, j] = node;
            }
        }

        
    }

    public List<AStarNode> findPath(Vector2 startPos, Vector2 endPos)
    {
        //判断起点/终点是否超出区域范围，是否是障碍物 
        if (startPos.x < 0 || startPos.y < 0 || startPos.x > mapHeight || startPos.y > mapWidth ||
            endPos.x < 0 || endPos.y < 0 || endPos.x > mapHeight || endPos.y > mapWidth)
        {
            Debug.Log("起/终点在范围外");
            return null;
        }
        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];
        if (startNode.block == true || endNode.block == true)
        {
            Debug.Log("起/终点是障碍物");
            return null;
        }
        //清空开启/关闭列表
        closeList.Clear();
        openList.Clear();
        //把起点放入关闭列表中
        startNode.fatherNode = null;
        startNode.f = 0;
        startNode.g = 0;
        startNode.h = 0;
        closeList.Add(startNode);
        while (true)
        {
            //将起点周围的八个点放入开启列表中
            foreach (int i in direction)
            {
                foreach (int j in direction)
                {
                    if (!(i == 0 && j == 0))
                    {
                        //上下左右格子离父格距离为1
                        if (i == 0 || j == 0)
                        {
                            FindAdjacentNodes(startNode.x + i, startNode.y + j, 1f, startNode, endNode);
                        }
                    }
                }
            }

            //把周围的点都加入openlist后，若openlist为空，则为死路
            if (openList.Count == 0)
            {
                //Debug.Log("死路，"+"起点："+startNode.x+"  "+startNode.y+" 终点:"+endNode.x+"  "+endNode.y);
                return null;
            }

            //选出openList中f最小的点，放入关闭列表中
            openList.Sort(SortOpenList);
            closeList.Add(openList[0]);
            //找到的最优点作为起点再次寻路
            startNode = openList[0];
            openList.RemoveAt(0);

            //若找到终点，从尾到头回述获得路径
            if (startNode == endNode)
            {
                List<AStarNode> path = new List<AStarNode>();
                while (endNode != null)
                {
                    path.Add(endNode);
                    endNode = endNode.fatherNode;

                }
                path.Reverse();
                return path;
            }
        }
    }

    private int SortOpenList(AStarNode _a, AStarNode _b)
    {
        if (_a.f > _b.f)
        {
            return 1;
        }
        else if (_a.f == _b.f)
        {
            return 1;
        }
        else return -1;
    }
    private void FindAdjacentNodes(int x, int y, float g, AStarNode _fatherNode, AStarNode _endNode)
    {
        if (x < 0 || y < 0 || x >= mapHeight || y >= mapWidth)
        {
            return;
        }
        AStarNode adjacentNode = nodes[x, y];
        if (adjacentNode == null || adjacentNode.block == true || openList.Contains(adjacentNode) || closeList.Contains(adjacentNode))
        {
            //空点/障碍物/已存在开启或关闭列表中
            return;
        }
        //计算f值
        adjacentNode.fatherNode = _fatherNode;
        adjacentNode.g = _fatherNode.g + g;
        adjacentNode.h = Mathf.Abs(_endNode.x - adjacentNode.x) + Mathf.Abs(_endNode.y - adjacentNode.y);
        adjacentNode.f = adjacentNode.g + adjacentNode.h;

        //把符合规范的邻点加入openlist
        openList.Add(adjacentNode);
    }
}
