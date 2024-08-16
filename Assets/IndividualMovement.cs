using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IndividualMovement : MonoBehaviour, IComparable<IndividualMovement>
{
    public class LegController
    {
        //Control
        public GameObject LegObj;
        public TwoBoneIKConstraint LegConstraint;
        public Transform LegIk;
        public int step = 0;
        public bool lastMoveFinished = false;

        //Movs
        public int genes;
        public float mutationChance = 0;
        public List<Vector3> Movements;

        //Status
        public Vector3 target;
        public float tolerance = .03f;
        public float displacement = .7f;
        public float speed = .3f;

        public LegController(GameObject LegObj,
            float tolerance,
            float displacement,
            float speed,
            int genes,
            float mutationChance
            )
        {
            //Control
            this.LegObj = LegObj;
            LegConstraint = LegObj.GetComponent<TwoBoneIKConstraint>();
            LegIk = LegConstraint.data.target;

            //GA
            this.genes = genes;
            this.mutationChance = mutationChance;

            //Status
            this.tolerance = tolerance;
            this.displacement = displacement;
            this.speed = speed;

            GenerateDna();
            NextTarget();
        }

        public void Breed(LegController dad, LegController mom)
        {
            for(int i = 0; i < genes; i++)
            {
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    Movements[i] = dad.Movements[i];
                }
                else
                {
                    Movements[i] = mom.Movements[i];
                }

                if (RandomPercent(mutationChance))
                {
                    Movements[i] = RandomDir();
                }
            }
        }

        public void GenerateDna()
        {
            Movements = new List<Vector3>();

            for(int i = 0; i < genes; i++)
            {
                Movements.Add(RandomDir());
            }
        }

        public Vector3 RandomDir()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        public bool RandomPercent(float chance)
        {
            return UnityEngine.Random.Range(0, 100) <= chance;
        }

        public void Move()
        {
            lastMoveFinished = false;
            if (target != LegIk.position)
            {
                LegIk.position = Vector3.MoveTowards(LegIk.position, target, speed * Time.deltaTime);
            }
            else
            {
                if (Vector3.Distance(LegConstraint.data.tip.position, target) > tolerance)
                {
                    // If the target is unreachable the movement will be unmade
                    LegIk.position -= Movements[step] * displacement;
                }

                step++;
                if (step >= Movements.Count)
                {
                    step = 0;
                }
                NextTarget();

                lastMoveFinished = true;
            }
        }

        public void NextTarget()
        {
            target = LegIk.position + Movements[step] * displacement;
        }

    }

    public class TransformsToReset
    {
        public Transform target;
        public Vector3 position;
        public Quaternion rotation;

        public TransformsToReset(Transform target, Vector3 position, Quaternion rotation)
        {
            this.target = target;
            this.position = position;
            this.rotation = rotation;
        }

        public void ResetPosition()
        {
            this.target.position = position;
            this.target.rotation = rotation;
        }
    }

    public GameObject Leg1Obj;
    public GameObject Leg2Obj;
    public GameObject Leg3Obj;
    public GameObject Leg4Obj;

    public List<LegController> Legs;

    public int legsStep = 0;

    public Transform BodyRoot;

    public bool paused = false;

    public List<TransformsToReset> transformsToReset;

    //LegStatus
    public float tolerance = .3f;
    public float displacement = .7f;
    public float speed = .3f;

    //GA
    public int genes = 5;
    public float mutationChance = 20f;
    public float score = 0;

    void Awake()
    {
        Legs = new List<LegController>
        {
            new(Leg1Obj, tolerance, displacement, speed, genes, mutationChance),
            new(Leg2Obj, tolerance, displacement, speed, genes, mutationChance),
            new(Leg3Obj, tolerance, displacement, speed, genes, mutationChance),
            new(Leg4Obj, tolerance, displacement, speed, genes, mutationChance)
        };
    }

    private void Update()
    {
        if (paused){
            return;
        }
        if (Legs[legsStep].lastMoveFinished)
        {
            legsStep++;
            if (legsStep >= Legs.Count)
            {
                legsStep = 0;
            }
        }

        Legs[legsStep].Move();
    }

    public void SetTransforms()
    {
        transformsToReset = new List<TransformsToReset>
        {
            new TransformsToReset(transform, Vector3.zero, transform.rotation)
        };

        foreach (Transform child in this.transform)
        {
            Debug.Log("Childs: " + child);
            transformsToReset.Add(new TransformsToReset(child.transform, child.position, child.rotation));
        }

    }

    public void Breed(IndividualMovement dad, IndividualMovement mom)
    {
        for(int i = 0; i < Legs.Count; i++)
        {
            Legs[i].Breed(dad.Legs[i], mom.Legs[i]);
        }
    }

    public void MeasureScore(Transform mastro)
    {
        this.score = Vector3.Distance(mastro.position, BodyRoot.position);
    }

    public int CompareTo(IndividualMovement other)
    {
        // Sort by score in ascending order
        return this.score.CompareTo(other.score);
    }

    public void Pause()
    {
        paused = true;
    }

    public void UnPause()
    {
        paused = false;
    }

    public void Reset()
    {
        this.GetComponent<Animator>().enabled = false;
        //this.enabled = false;

        foreach (TransformsToReset tr in transformsToReset)
        {
            tr.ResetPosition();
        }

        this.enabled = true;
        this.GetComponent<Animator>().enabled = true;

    }

    public void OverrideData(IndividualMovement ind)
    {
        Legs[0].Movements = ind.Legs[0].Movements;
        Legs[1].Movements = ind.Legs[1].Movements;
        Legs[2].Movements = ind.Legs[2].Movements;
        Legs[3].Movements = ind.Legs[3].Movements;

        legsStep = 0;

        paused = ind.paused;
    }
}
