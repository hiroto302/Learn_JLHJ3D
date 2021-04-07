using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;
    bool m_IsPlayerInRange;

    public GameEnding gameEnding;
    void OnTriggerEnter (Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
    }
    void OnTriggerExit (Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
    }

    void Update()
    {
        if (m_IsPlayerInRange)
        {
            // 対象への方向座標 =  対象の座標 - 自分の座標 (+ 対象の大体の重心座標で補正)
            Vector3 direction = player.position - transform.position + Vector3.up;
            // 自分の座標(第一引数 : origin)から求めた座標方向(第二引数 : direction)に ray を作成
            Ray ray = new Ray(transform.position, direction);
            // レイキャストによる情報を得るための構造体
            RaycastHit raycastHit;
            /* 最初に衝突を検知したものが raycatHit に格納される。 そのため、壁越しなどでPlayerが発見されることがなくなる。
               最適化のために、Rayの長さを制限したり、MaskLayerを加えることも念頭におく
            */
            if(Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    gameEnding.CaughtPlayer();
                }
            }

        }

    }
}
