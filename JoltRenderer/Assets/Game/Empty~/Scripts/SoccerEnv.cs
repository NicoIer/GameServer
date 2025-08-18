// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class SoccerEnv : MonoBehaviour
// {
//     public List<SoccerAgent> agentList = new List<SoccerAgent>();
//     public Rigidbody ball;
//     
//     public void GoalTouched(SoccerAgent.Team scoredTeam)
//     {
//         foreach(SoccerAgent temp_agent in agentList)
//         {
//             if(temp_agent.team != scoredTeam)
//             {
//                 temp_agent.AddReward(1.0f);
//             }
//             temp_agent.OnEpisodeBegin();
//
//         }
//         RestBall();
//     }
//
//
//     public void FixedUpdate()
//     {
//         if(Mathf.Abs(ball.transform.localPosition.x)>14.0f || Mathf.Abs(ball.transform.localPosition.z) > 5.5f)
//         {
//             foreach (SoccerAgent temp_agent in agentList)
//             {
//                 temp_agent.OnEpisodeBegin();
//             }
//             RestBall();
//         }
//     }
//
//     public void RestBall()
//     {
//         ball.transform.localPosition = Vector3.up * 0.5f;
//         ball.transform.rotation = Quaternion.Euler(Vector3.zero);
//         ball.velocity = Vector3.zero;
//         ball.angularVelocity = Vector3.zero;
//     }
// }
