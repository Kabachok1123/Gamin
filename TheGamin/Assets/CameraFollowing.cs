using UnityEngine; 

public class CameraFollow : MonoBehaviour 
{ 
        [SerializeField] float y_offset = 4.0f;

    public Transform player; // ссылка на объект игрока 

    private void LateUpdate() 
    { 
        // Перемещаем камеру к позиции игрока 
        transform.position = new Vector3(player.position.x, player.position.y+y_offset, transform.position.z);  
    } 
}