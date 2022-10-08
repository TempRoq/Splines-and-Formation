using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Singleton<Level>
{

    /*
     * TODO:
     * + Make Loop
     * + Hash in EmptyBezierTransforms to be enemy -1
     */

    #region Data Structures

    public enum LevelState
    {
        NOT_BEGAN,
        CONTINUING,
        CLEAR_FLAGGED,
        CLEARED
    }
    [System.Serializable]
    public enum NextCondition
    {
        NONE,
        WAIT_FOR_WAVE_SPAWN,
        FRAMES,
        FRAMES_AFTER_WAVE_SPAWN,
        ENEMIES_REMAINING,
        ENEMIES_REMAINING_AFTER_WAVE_SPAWN
    }

    [System.Serializable]
    public class FormationNode
    {
        public Formation formation;
        public Vector3 origin;
        public float frameToSpawn, numberOfLoops, loopDelay;
        public int trackable;
        public NextCondition nextWaveCondition;

        public IEnumerator SpawnFormationNode()
        {
            instance.waiting = true;
            instance.numThingsSpawning += 1;
            int currLoop = 0;
            int frames = 0;
            while (currLoop < numberOfLoops)
            {
                if (frames == frameToSpawn)
                {
                    formation.SpawnFormation(origin, false, null);
                }


                frames += 1;

                if (frames == frameToSpawn + loopDelay)
                {
                    frames = 0;
                    currLoop++;
                }
                yield return new WaitForFixedUpdate();
            }
            instance.numThingsSpawning -= 1;
        }
    }

    


    [System.Serializable]
    public struct Position
    {
        public int enemyHash;
        public Orientation orientation;
        public float positionalSpeedMultOffset, initialT; //GAS GAS GAS  
        public Formation[] childFormations;
    }

    [System.Serializable]
    public struct Orientation
    {
        public Vector2 offset;
        public Vector2 pathScale;
        public float angleOffset;
    }
    #endregion

    #region Tracking
    protected NextCondition currCondition = NextCondition.NONE;
    protected int currentFrame = 0;
    protected int trackable = 0;
    public int currentWave = 0;

    private int numThingsSpawning = 0;

    protected bool begin = false;
    protected bool waiting = false;
    public bool clearFlag = false;
    #endregion
    public Vector3 worldPosition;
    public Formation mainFormation;
    public BezierPath[] pathPool;
    public FormationNode[] waves;

    protected int enemiesSpawned = 0, currNumEnemies = 0, numEnemiesShotDown = 0;
    public LevelState ls;
    


    [SerializeField]
    protected GameObject[] enemies;
    [SerializeField]
    protected int[] enemyAmounts;
    protected int enemyHashFloor;

    public int EnemyHashFloor { get { return enemyHashFloor; } }

    public void BeginLevel()
    {
        currentFrame = 0;
        currentWave = 0;
        currCondition = waves[0].nextWaveCondition;
        ls = LevelState.CONTINUING;
        clearFlag = false;
        begin = true;
    }

    public void NextWave()
    {
        currentFrame = 0;
        currentWave += 1;
        if (currentWave == waves.Length)
        {
            clearFlag = true;
            return;
        }
        currCondition = waves[currentWave].nextWaveCondition;
        trackable = waves[currentWave].trackable;
    }

    protected override void Awake()
    {
        base.Awake();
        enemyHashFloor = Pool.instance.HashSpawnables(enemies, enemyAmounts);
    }

    //Current 
    protected virtual void FixedUpdate()
    {
        if (currentWave >= waves.Length)
        {
            return;
        }
        if (!begin)
        {
            return;
        }
        if (ls == LevelState.CLEARED)
        {
            return;
        }
        if (waiting)
        {
            if (clearFlag)
            {
                if (currNumEnemies == 0)
                {
                    Debug.Log("You Did IT!");
                    ls = LevelState.CLEARED;
                }
            }
            else {
                switch (currCondition)
                {
                    case NextCondition.NONE:
                        waiting = false;
                        currentFrame = 0;
                        break;
                    case NextCondition.WAIT_FOR_WAVE_SPAWN:
                        if (numThingsSpawning == 0)
                        {
                            waiting = false;
                            currentFrame = 0;
                        }
                        break;
                    case NextCondition.FRAMES:
                        currentFrame += 1;
                        if (currentFrame == trackable)
                        {
                            waiting = false;
                            currentFrame = 0;
                        }
                        break;
                    case NextCondition.FRAMES_AFTER_WAVE_SPAWN:
                        if (numThingsSpawning == 0)
                        {
                            currentFrame += 1;
                            if (currentFrame == trackable)
                            {
                                waiting = false;
                                currentFrame = 0;
                            }
                        }
                        break;
                    case NextCondition.ENEMIES_REMAINING:
                        if (currNumEnemies < trackable)
                        {
                            waiting = false;
                            currentFrame = 0;
                        }
                        break;
                    case NextCondition.ENEMIES_REMAINING_AFTER_WAVE_SPAWN:
                        if (numThingsSpawning == 0 && currNumEnemies < trackable)
                        {
                            waiting = false;
                            currentFrame = 0;
                        }
                        break;
                }
            }
        }

        if (!waiting)
        {
            StartCoroutine(waves[currentWave].SpawnFormationNode());
            currCondition = waves[currentWave].nextWaveCondition;
            NextWave();
        }


        /*

        while (currentPattern < waves[currentWave].pattern.Length && waves[currentWave].frames[currentPattern] <= currentFrame) { //while the current pattern
            Debug.Log(waves[currentWave].comment);
            waves[currentWave].pattern[currentPattern].SpawnFormation(transform.position + (Vector3)waves[currentWave].offsets[currentPattern]);
            currentPattern += 1;
            if (currentPattern == waves[currentWave].frames.Length)
            {
                NextWave();
                if (currentWave == waves.Length)
                {
                    clearFlag = true;
                    break;
                }
                if (waves[currentWave].nextWaveCondition == NextCondition.NONE || (waves[currentWave].nextWaveCondition == NextCondition.ENEMIESREMAINING && currNumEnemies <= waves[currentWave].amount))
                {
                    waiting = false;
                }
                else
                {
                    waiting = true;
                }
                
            }
        }
        */
        currentFrame++;
    }


    #region Tracking
    public void AddEnemy()
    {
        currNumEnemies += 1;
    }
    public void RemoveEnemy()
    {
        currNumEnemies -= 1;
    }
    #endregion



    private static Color[] pathColors = new Color[] { Color.red, new Color { r = 1, g = .64f, b = 0f, a = 1 }, Color.yellow, Color.green, Color.blue, new Color { r = .42f, g = .05f, b = .68f, a = 1 } };
    private void OnDrawGizmos()
    {
        float radius = .1f;
        for (int i = 0; i < pathPool.Length; i++)
        {
            for (int j = 0; j < pathPool[i].Nodes.Length; j++)
            {
                Gizmos.color = pathColors[j % pathColors.Length];
                Gizmos.DrawCube(worldPosition + pathPool[i].startPoint + pathPool[i].Nodes[j].curve.AnchorA, Vector3.one * radius * 3);
                Gizmos.DrawCube(worldPosition + pathPool[i].startPoint + pathPool[i].Nodes[j].curve.AnchorB, Vector3.one * radius * 3);
                
                for (float k = 0; k < 1f; k += (.016666f * Mathf.Max(1, pathPool[i].Nodes[j].speedMultiplier)))
                {
                    Gizmos.color = Color.Lerp(pathColors[j % pathColors.Length], pathColors[(j + 1)% pathColors.Length], k);
                    Gizmos.DrawSphere(worldPosition +pathPool[i].startPoint+ pathPool[i].LocationAtTime(j + k, pathPool[i].Nodes[j].tween), radius);
                }
            }
        }
    }



}
