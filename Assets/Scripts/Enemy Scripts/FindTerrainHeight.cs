using UnityEngine;

public class FindTerrainHeight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit, 25);
        Debug.DrawRay(hit.point, hit.normal, Color.yellow);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 25 - hit.distance, gameObject.transform.position.z);
    }
}
