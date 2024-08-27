using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/**
* Esta classe é responsável por controlar o movimento de uma perna do índividuo.
*/
public class LegController
{
    //Variáveis de controle
    public GameObject LegObj;
    public TwoBoneIKConstraint LegConstraint;
    public Transform LegIk;

    //Variáveis de controle do movimento
    public List<Vector3> Movements;
    public Vector3 target;
    public int step = 0;
    public bool lastMoveFinished = false;

    //Variáveis de status da perna
    public int genes;
    public float mutationChance = 0;
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

    // Cruza os genes de dois indivíduos sobreescrevendo este
    public void Breed(LegController dad, LegController mom)
    {
        for (int i = 0; i < genes; i++)
        {
            if (Random.Range(0, 2) == 0)
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

    // Cria um dna de genes aleratórios
    public void GenerateDna()
    {
        Movements = new List<Vector3>();

        for (int i = 0; i < genes; i++)
        {
            Movements.Add(RandomDir());
        }
    }

    // Gera uma direção aleatória (ignorando o eixo y)
    public Vector3 RandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    // Retorna verdadeiro com uma chance de chance %
    public bool RandomPercent(float chance)
    {
        return Random.Range(0, 100) <= chance;
    }

    /**
     * Move a perna em relação ao target
     * se o target for atingido, o próximo target é definido
     * se não for possível atingir o target, o movimento é desfeito e 
     * segue para o próximo target
     */
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

    // Define o próximo target
    public void NextTarget()
    {
        target = LegIk.position + Movements[step] * displacement;
    }

}