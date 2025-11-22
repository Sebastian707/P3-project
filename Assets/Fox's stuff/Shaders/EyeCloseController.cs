using UnityEngine;

public class EyeCloseController : MonoBehaviour
{
    public Material eyeMat;   // assign the SAME material used in the renderer feature

    void Update()
    {
        float t = Mathf.PingPong(Time.time * 0.5f, 1f);
        eyeMat.SetFloat("_Eyes_closed", t);    // property name EXACTLY as shown in Shader Graph
    }
}
