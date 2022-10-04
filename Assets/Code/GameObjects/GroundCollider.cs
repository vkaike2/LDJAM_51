using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class GroundCollider : MonoBehaviour
{
    private Player _player;

    private void Start()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _player.CheckIfIsOnGround(collision);
    }
}
