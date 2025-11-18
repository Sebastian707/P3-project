using UnityEngine;

public class LaserBEAM : MonoBehaviour
{
    [SerializeField] private int maxReflectBounce;
    [SerializeField] private float maxLaserDistance;
    private int reflectCount;
    [SerializeField] private LineRenderer laserLine;
    public Transform laserStartPoint;
    [SerializeField] private Vector3 _offSet;
    [SerializeField] private bool mirrorReflect = true;
    [SerializeField] private GameObject currentTarget;
    public string targetName;
    private void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        laserLine.SetPosition(0, laserStartPoint.position);
        
    }
    private void Update()
    {
        
        castLaser(transform.position, transform.forward);
    }
    private void castLaser(Vector3 position, Vector3 direction)
    {
        laserLine.SetPosition(0, laserStartPoint.position);

        for (int i = 0; i < maxReflectBounce; i++)
        {

            Ray ray = new Ray(position, direction);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, maxLaserDistance, 1))
            {

                position = hit.point;
                direction = Vector3.Reflect(direction, hit.normal);
                laserLine.SetPosition(i + 1, hit.point);

                if (hit.transform.tag != "Mirror" && mirrorReflect)
                {
                    if (hit.transform.tag == "LaserTag")
                    {
                        Debug.Log("Hit Laser Target");
                        currentTarget = hit.transform.gameObject;
                        currentTarget.GetComponent<LaserTarget>().lightTouched = true;
                        
                        //Debug.Log("Hit Non Mirror Object");
                        for (int j = (i + 1); j <= maxReflectBounce; j++)
                        {
                        laserLine.SetPosition(j, hit.point);

                        }
                        
                        break;
                    }
                    if(hit.transform.tag != "LaserTag")
                    {
                        Debug.Log("we falsing it");
                        currentTarget.GetComponent<LaserTarget>().lightTouched = false;
                        
                    }



                }
                //Debug.Log("Hit Mirror Object");




            }

        }

    }
}


