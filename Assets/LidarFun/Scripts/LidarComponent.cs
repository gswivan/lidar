
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class LidarComponent : MonoBehaviour
{

    public string port;
    public float scale = 1.0f;
    public Vector2 offset;
    public Vector2 size;
    
    private void OnEnable()
    {
        LidarThread.Start(port);
    }

    private void OnDisable()
    {
        LidarThread.Stop();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 offset3 = new Vector3(offset.x, offset.y, 0);
        Gizmos.DrawWireCube(transform.position + offset3, size);
        if (EditorApplication.isPlaying)
        {
            LidarPoints p = LidarThread.GetPoints();
            for (int i = 0; i < p.Length(); ++i)
            {
                Vector3 point = p.Get(i) * scale - offset3;
                if (point.x < -size.x / 2) continue;
                if (point.x > size.x / 2) continue;
                if (point.y < -size.y / 2) continue;
                if (point.y > size.y / 2) continue;
                Gizmos.DrawWireSphere(transform.position + point + offset3, 0.1f * scale / 2.0f);
            }
        }
    }
}
