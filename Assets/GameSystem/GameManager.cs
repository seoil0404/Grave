using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int stack = 0;

    private float time = 0;

    private void Start()
    {

        PlayerController.Instance.transform.position = MapManager.CurrentSquare.Position + Vector3.up * 1.8f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stack++;
            MapManager.CurrentSquare = MapManager.CurrentSquare.NodeList[0].Square;
            Debug.Log(stack);
            time = 1f;
        }

        time -= Time.deltaTime;
        
        if(time > 0)
            PlayerController.Instance.transform.position = Vector3.Lerp(PlayerController.Instance.transform.position, MapManager.CurrentSquare.Position + Vector3.up * 3f, 0.05f);
    }
}
