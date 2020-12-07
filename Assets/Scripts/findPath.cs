using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class findPath : MonoBehaviour
{
    enum Direction{North,East,South,West};
    public int beginX = 0;
    public int beginY = 0;
    public int widthForNode = 30;
    public int heightForNode = 30;
    public int mapHeight = 600;
    public int mapWidth = 600;
    public float moveSpeed = 60f;
    public int roadMask;
    public GameObject car;
    private bool isMoving = false;
    private int moveIndex = 0;
    public int chooseRange = 10;
    private int day=1;
    private int time = 1;
    public Dropdown dayDropdown;
    public Dropdown timeDropdown;

    private Vector2 beginPos=Vector2.right*-1;
    public Vector2 endPos;
    //private List<Vector2> parkingLots = new List<Vector2>();
    private Dictionary<int,Vector2> parkingLots = new Dictionary<int,Vector2>();
    private List<AStarNode> path;
    private Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();
    void Start()
    {
        DataManager.GetInstance().readDataFromTxt();
        NodeManager.GetInstance().Initialization(mapHeight, mapWidth);
        //扫描一遍把道路设置成非障碍物
        Vector3 pointCheck = new Vector3(0f, 0f, 0f);
        int h_Node_Number = mapHeight / heightForNode;
        int w_Node_Number = mapWidth / widthForNode;
        for (int i = 0; i <= h_Node_Number; i++)
        {
            for (int j = 0; j <= w_Node_Number; j++)
            {
                pointCheck.x = i * heightForNode;
                pointCheck.z = j * widthForNode;
                Ray ray = new Ray(pointCheck, Vector3.down);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 100f);
                if (hit.transform.gameObject.tag == "Road")
                {
                    NodeManager.GetInstance().nodes[i, j].block = false;
                }
                else if (hit.transform.gameObject.tag == "Parking")
                {
                    int name = int.Parse(hit.transform.gameObject.name);
                    NodeManager.GetInstance().nodes[i, j].block = false;
                    Vector2 parkingPos = new Vector3(NodeManager.GetInstance().nodes[i, j].x, 
                        NodeManager.GetInstance().nodes[i, j].y);
                    parkingLots.Add(name,parkingPos);
                    
                }
            }
        }

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1)&& EventSystem.current.IsPointerOverGameObject() == false)
        {

            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //判断鼠标点击到哪个立方体
            if (Physics.Raycast(ray, out info, 1000))
            {
                beginPos = new Vector2((int)car.transform.position.x / 30, (int)car.transform.position.z / 30);
                
                endPos = new Vector2((int)info.point.x / 30, (int)info.point.z / 30);
                endPos = chooseParkingLot(endPos);
                path = NodeManager.GetInstance().findPath(beginPos, endPos);
                    //如果是死路，恢复起点初始值，把起点和终点颜色变回白色
                    if (path == null)
                    {
                        
                    }
                    else
                    {
                        car.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);
                        moveIndex = 0;
                        isMoving = true;
                    }
                }
            }
       
        if (isMoving) {
            if (moveIndex == path.Count - 1)
            {
                isMoving = false;
                car.transform.position = new Vector3(path[moveIndex].x * widthForNode, 0f, path[moveIndex].y * heightForNode);
            }
            else {
                Vector3 curPos = new Vector3(path[moveIndex].x * widthForNode, 0f, path[moveIndex].y * heightForNode);
                Vector3 nextPos = new Vector3(path[moveIndex + 1].x * widthForNode, 0f, path[moveIndex + 1].y * heightForNode);
                
                changeDir((int)car.transform.localEulerAngles.y, curPos, nextPos);
                car.transform.position = Vector3.MoveTowards(car.transform.position, nextPos, Time.deltaTime * moveSpeed);
                if (Vector3.Distance(car.transform.position, nextPos) <= 2f)
                {
                    moveIndex++;
                }
            }
            
            
        }
    }
    void changeDir(int nowRotation, Vector3 curPos, Vector3 nextPos) {
        Direction curDir=Direction.North;
        Direction nextDir=Direction.North;
        switch (nowRotation / 90) {
            case 0:curDir = Direction.North;
                break;
            case 1:
                curDir = Direction.East;
                break;
            case 2:
                curDir = Direction.South;
                break;
            case 3:
                curDir = Direction.West;
                break;
        }
        Vector3 dir = nextPos - curPos;
        if (dir.x >= 1) nextDir = Direction.East;
        else if (dir.x <= -1) nextDir = Direction.West;
        else if (dir.z >= 1) nextDir = Direction.North;
        else if (dir.z <= -1) nextDir = Direction.South;
        int dif = (int)nextDir - (int)curDir;
        if (dif == 1||dif==-3)
        {
            //需要右转
            
            car.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);
        }
        else if (dif==2) car.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        else if (dif == 0) return;
        else if (dif==-1||dif==3)
        {
            //需要左转
            
            car.transform.Rotate(new Vector3(0f, -90f, 0f), Space.Self);
        }



    }

    Vector2 chooseParkingLot (Vector2 destination) {
        double maxRecommendRate=double.MinValue;
        Vector2 choosedLot=Vector2.zero;
        List<Vector2> sortedLots= new List<Vector2>();
        
        foreach (var dic in  parkingLots) {
            //范围限制，不考虑选择范围以外的停车场
            if (Vector2.Distance(dic.Value, destination) >= chooseRange) continue;

            float emptyRate = DataManager.GetInstance().GetStatusByDayAndTime(dic.Key, 
                dayDropdown.value+1, timeDropdown.value+1);
            
            //加权公式：30m换1%的空闲率
            double recommendRate = (double)emptyRate - 0.01*Vector2.Distance(dic.Value, destination)/30;
            if (recommendRate > maxRecommendRate && emptyRate>0.8f)
            {
                maxRecommendRate = recommendRate;
                choosedLot = dic.Value;
            }
            
        }
        

        return choosedLot;
    }
}
