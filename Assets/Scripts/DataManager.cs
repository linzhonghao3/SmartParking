using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager 
{
    public enum Status { Busy, Normal, Empty};
    private static DataManager instance;
    private DataManager()
    {
    }

    public static DataManager GetInstance()
    {
        if (instance == null)
        {
            instance = new DataManager();
        }
        return instance;
    }
    public TextAsset parkingLotsData;
    private string[][] Array;
    public void readDataFromTxt() {
        parkingLotsData = Resources.Load("Data", typeof(TextAsset)) as TextAsset;
        string[] lineArray = parkingLotsData.text.Split("\r"[0]);
        Array = new string[lineArray.Length][];
        //把csv中的数据储存在二位数组中  
        for (int i = 0; i < lineArray.Length; i++)
        {
            Array[i] = lineArray[i].Split(',');
        }
        //Debug.Log("共有行数：" + Array.Length);
        
    }
    public float GetStatusByDayAndTime(int parkIndex,int day, int time) {
        int col = (day - 1) * 24 + time;
        
        float emptyRate=float.Parse(GetDataByRowAndCol(parkIndex-1, col));
        //if (emptyRate < 0.8f) return Status.Busy;
        //else if (emptyRate >= 0.8f && emptyRate < 0.85f) return Status.Normal;
        //else return Status.Empty;
        return emptyRate;
    }
    public string GetDataByRowAndCol(int nRow, int nCol)
    {
        if (Array.Length <= 0 || nRow >= Array.Length)
            return "";
        if (nCol >= Array[0].Length)
            return "";
        
        return Array[nRow][nCol];
    }
}
