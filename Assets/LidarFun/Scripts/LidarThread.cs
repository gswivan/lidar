

using System.Threading;
using System.Collections.Generic;
using RPLidar;
using UnityEngine;

public class LidarPoints
{

    private List<Vector3> points = new List<Vector3>();
    
    public LidarPoints()
    {
    }

    public LidarPoints(Scan s)
    {
        foreach (Measurement measurement in s.Measurements)
        {
            if (measurement.Distance <= float.Epsilon) continue;
            float angle = measurement.Angle * Mathf.Deg2Rad;
            float distance = measurement.Distance;
            points.Add(new Vector3(-distance * Mathf.Cos(angle), distance * Mathf.Sin(angle), 0));
        }
    }
    
    
    public Vector3 Get(int index_)
    {
        return points[index_];
    }

    public int Length()
    {
        return points.Count;
    }
    
}


public class LidarThread
{
    private string m_port;
    private LidarPoints m_points;
    private CancellationTokenSource m_cancel;
    
    public LidarThread(string port_)
    {
        m_port = port_;
        m_points = new LidarPoints();
        m_cancel = new CancellationTokenSource();
    }
    
    void Run()
    {
        RPLidar.Lidar lidar = new RPLidar.Lidar();
        lidar.PortName = m_port;
        lidar.ReceiveTimeout = 3000;
        lidar.IsFlipped = false;
        lidar.AngleOffset = 0;
        
        lidar.Reset();
        if (lidar.Open())
        {
            lidar.ControlMotorDtr(false);
            if (lidar.StartScan(ScanMode.ExpressLegacy))
            {
                while (!m_cancel.IsCancellationRequested)
                {
                    Scan s = lidar.GetScan(m_cancel.Token);
                    if (s == null)
                    {
                        continue;
                    }
                    lock (this)
                    {
                        m_points = new LidarPoints(s);
                    }
                }
                lidar.StopScan();
                lidar.ControlMotorDtr(true);
            }
            lidar.Close();
        }
    }

    private LidarPoints GetLidarPoints()
    {
        lock (this)
        {
            return m_points;
        }
    }
    
    private static Thread ThreadInstance;
    private static LidarThread Instance = null;
    
    public static void Start(string port_)
    {
        Instance = new LidarThread(port_);
        ThreadInstance = new Thread(Instance.Run);
        ThreadInstance.Start();
    }

    public static void Stop()
    {
        Instance.m_cancel.Cancel();
        ThreadInstance.Join();
        Instance = null;
    }

    public static LidarPoints GetPoints()
    {
        if (Instance != null)
        {
            return Instance.GetLidarPoints();
        }
        return new LidarPoints();
    }
    
}
