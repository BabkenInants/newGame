using UnityEngine;

public class PlayerLink : MonoBehaviour
{
    private Player _player;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    public void RunAnim()
    {
        _player.RunAnim();
    }

    public void StopAnim()
    {
        _player.StopAnim();
    }
}
