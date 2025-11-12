using UnityEngine;

public class LightBeam : MonoBehaviour
{
    public GameObject beamPos;
    public GameObject lightBeamPrefab;
    public bool startBeamOnStart = true;
    [SerializeField] private float beamMaxHeightY;
    [SerializeField] private float beamMaxWidthZ;
    [SerializeField] private float beamMaxLengthX;
    private GameObject currentBeam;
    public void Start()
    {
        if (startBeamOnStart == true)
        {
            LightBeamClass();
        }
    }
    public void LightBeamClass()
    {
        currentBeam = Instantiate(lightBeamPrefab,beamPos.transform.position,beamPos.transform.rotation);
        currentBeam.transform.localScale = new Vector3(beamMaxLengthX, beamMaxHeightY, beamMaxWidthZ);
        currentBeam.transform.position = new Vector3(beamPos.transform.position.x + beamMaxLengthX/2, beamPos.transform.position.y, beamPos.transform.position.z);
        currentBeam.transform.SetParent(beamPos.transform);
    }


}
