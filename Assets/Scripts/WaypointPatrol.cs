using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;
    int m_CurrentWayPointIndex; // 初期値 : 0
    void Start()
    {
        // 初期の目標移動地点
        navMeshAgent.SetDestination (waypoints[0].position);
    }
    void Update()
    {
        /*
            NavMeshAgent.remainingDistace : エージェントの位置および現在の経路での目標地点の間の距離
            NavMeshAgent.stoppingDistace  : 目標地点のどれぐらい手前で停止するかの距離
                                            (目標地点にちょうど到達することはきわめて稀であるため、このプロパティーによりエージェントが停止すべき半径を設定できる)
        */
        // 次の移動目標地点まで、残り距離が 0.2f以下になった時
        if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            // 次の移動地点の更新
            m_CurrentWayPointIndex = (m_CurrentWayPointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[m_CurrentWayPointIndex].position);
        }
    }
}
