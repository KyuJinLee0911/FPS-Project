using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bastion : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // HitBox Collider가 아니라면 무시
        if(other.gameObject.layer != 7) return;

        // 투사체가 아닐 경우
        // - 보스의 공격은 투사체가 아님
        Projectile _projectile = other.GetComponent<Projectile>();
        if(_projectile == null)
        {

            return;
        }
        
        // 투사체가 맞을 경우
        // 투사체를 발사한 공격자가 플레이어일 경우 무시
        Player _player = _projectile.Instigator.GetComponent<Player>();
        if(_player != null)
            return;
        
        // 발사한 주체가 적일 경우 무효화(오브젝트 풀에 다시 돌려놓음)
        GameManager.Instance._pool.ReturnObj(_projectile.Instigator.name, _projectile);

    }
}
