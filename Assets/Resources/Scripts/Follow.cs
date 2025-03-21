using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject Player;

    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.position = Player.transform.position + offset;
    }
}
