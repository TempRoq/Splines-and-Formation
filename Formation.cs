using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Formation : ScriptableObject
{
    public int pathHash;
    [Min(.25f)]
    public float speedMultiplier = 1f;

    public Level.Position[] positions;

    public void SpawnFormation(Vector3 origin, bool hasParent, BPTraverser parent)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject g = Pool.instance.GetInactiveInstance(Level.instance.EnemyHashFloor + positions[i].enemyHash);
            g.transform.position = origin + (Vector3)positions[i].orientation.offset;
            g.SetActive(true);
            BPTraverser bpt = g.GetComponent<BPTraverser>();
            bpt.ClearChildren();
            bpt.StartFollowingPath(Level.instance.pathPool[pathHash], hasParent,
                positions[i].orientation.pathScale, positions[i].orientation.offset, positions[i].initialT,
                speedMultiplier * positions[i].positionalSpeedMultOffset, positions[i].orientation.angleOffset);
            if (hasParent)
            {
                parent.AddChild(bpt);
            }
            
            foreach (Formation f in positions[i].childFormations)
            {
                f.SpawnFormation(origin + (Vector3)positions[i].orientation.offset, true, bpt);
            }
        }
    }
}