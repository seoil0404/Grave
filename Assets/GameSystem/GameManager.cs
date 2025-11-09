using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            MapManager.CurrentSquare = MapManager.CurrentSquare.TargetNode.Square;

            PlayerController.PlayerContext.MovementController.MoveCurve(
                PlayerController.Instance.transform.position, 
                MapManager.CurrentSquare.Position + Vector3.up * 2, 
                1f
                );

            GrassManager.Instance.DrawGrass(MapManager.CurrentSquare);
        }
    }
}
