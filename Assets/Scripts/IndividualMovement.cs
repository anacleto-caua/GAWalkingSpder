using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Esta classe é responsável por controlar o movimento de um indivíduo.
 */
public class IndividualMovement : MonoBehaviour, IComparable<IndividualMovement>
{
    // Variáveis de controle
    public GameObject Leg1Obj;
    public GameObject Leg2Obj;
    public GameObject Leg3Obj;
    public GameObject Leg4Obj;

    public List<LegController> Legs;

    public int legsStep = 0;

    public Transform BodyRoot;

    public bool paused = false;
    public float score = 0;

    // Variáveis de status das pernas
    public float tolerance = .3f; // Distância máxima do alvo para considerar que chegou
    public float displacement = .7f; // Distância movida por target
    public float speed = .3f; // Distância movida por frame
    public float mutationChance = 20f; // Chance de mutação por gene
    public int genes = 5; // Quantidade de genes por indivíduo

    // Primeiro método chamado, cria as pernas
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

    /**
     * Controla o movimento do indivíduo a cada frame.
     * 
     * Se está pausado ignora a execução
     * 
     * Se o último movimento foi terminado, passa para o próximo movimento da próxima perna
     * 
     * Se não houver mais pernas, volta para a primeira
     * 
     * Ao fim executa o movimento da perna atual
     */
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

    // Função de ponte para chamar o movimento de breed das pernas
    public void Breed(IndividualMovement dad, IndividualMovement mom)
    {
        for(int i = 0; i < Legs.Count; i++)
        {
            Legs[i].Breed(dad.Legs[i], mom.Legs[i]);
        }
    }

    // Calcula a distância do indíviduo para o mastro de alvo
    public void MeasureScore(Transform mastro)
    {
        this.score = Vector3.Distance(mastro.position, BodyRoot.position);
    }

    // Função de comparação para ordenação de indivíduos
    public int CompareTo(IndividualMovement other)
    {
        // Sort by score in ascending order
        return this.score.CompareTo(other.score);
    }

    #region handyFunctionsToPauseUnpause
    public void Pause()
    {
        paused = true;
    }

    public void UnPause()
    {
        paused = false;
    }
    #endregion handyFunctionsToPauseUnpause

    // Gambiarra pra poder resetar a posição do indíviduo
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
