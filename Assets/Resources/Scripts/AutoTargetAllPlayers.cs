using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class AutoTargetAllPlayers : MonoBehaviour
{
  CinemachineTargetGroup targetGroup;

  void Awake()
  {
    targetGroup = GetComponent<CinemachineTargetGroup>();
  }

  void Update()
  {
    //   if (targetGroup.m_Targets.Length == 0)
    //   {
    //     GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //     foreach (GameObject player in players)
    //       targetGroup.AddMember(player.transform, 1, 0);
    //   }
  }
}
