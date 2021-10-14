using UnityEngine;
public class CubeGenerator : MonoBehaviour
{
    public Transform SampleCube;
    public Transform PointGenerate;
    public Vector3 cubeOffset = new Vector3(.2f, 0, .2f);

    private bool _isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStart == false)
        {
            _isStart = true;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Vector3 newCube = PointGenerate.transform.position;
                    newCube.x = +i;
                    newCube.z = +j;

                    Vector3 localCubeOffset = new Vector3(cubeOffset.x, 0, cubeOffset.z);
                    localCubeOffset.x *= i;
                    localCubeOffset.z *= j;
                    localCubeOffset.y += UnityEngine.Random.value > .9f ? .5f : 0;

//                    Debug.Log(localCubeOffset);
                    newCube = newCube + localCubeOffset;

                    Transform tr = Instantiate(SampleCube, newCube, Quaternion.identity);
                    tr.parent = transform;

                }
            }
        }
    }
}
