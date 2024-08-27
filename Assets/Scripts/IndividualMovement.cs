using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Esta classe � respons�vel por controlar o movimento de um indiv�duo.
 */
public class IndividualMovement : MonoBehaviour, IComparable<IndividualMovement>
{
    // Vari�veis de controle
    public GameObject Leg1Obj;
    public GameObject Leg2Obj;
    public GameObject Leg3Obj;
    public GameObject Leg4Obj;

    public List<LegController> Legs;

    public int legsStep = 0;

    public Transform BodyRoot;

    public bool paused = false;
    public float score = 0;

    // Vari�veis de status das pernas
    public float tolerance = .3f; // Dist�ncia m�xima do alvo para considerar que chegou
    public float displacement = .7f; // Dist�ncia movida por target
    public float speed = .3f; // Dist�ncia movida por frame
    public float mutationChance = 20f; // Chance de muta��o por gene
    public int genes = 5; // Quantidade de genes por indiv�duo

    // Primeiro m�todo chamado, cria as pernas
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
     * Controla o movimento do indiv�duo a cada frame.
     * 
     * Se est� pausado ignora a execu��o
     * 
     * Se o �ltimo movimento foi terminado, passa para o pr�ximo movimento da pr�xima perna
     * 
     * Se n�o houver mais pernas, volta para a primeira
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

    // Fun��o de ponte para chamar o movimento de breed das pernas
    public void Breed(IndividualMovement dad, IndividualMovement mom)
    {
        for(int i = 0; i < Legs.Count; i++)
        {
            Legs[i].Breed(dad.Legs[i], mom.Legs[i]);
        }
    }

    // Calcula a dist�ncia do ind�viduo para o mastro de alvo
    public void MeasureScore(Transform mastro)
    {
        this.score = Vector3.Distance(mastro.position, BodyRoot.position);
    }

    // Fun��o de compara��o para ordena��o de indiv�duos
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

    // Gambiarra pra poder resetar a posi��o do ind�viduo
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
