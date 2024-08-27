using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMaestro : MonoBehaviour
{
    /**
     * Esta classe é responsável por controlar o fluxo do algoritmo genético.
     * 
     */

    // Variáveis de configuração (são definidas no editor, mudar lá)
    public int individualsSize = 10;
    public float timePerRound = 15; // seconds

    // Variáveis de controle
    public int rounds = 0;
    public float roundTimer;
    public bool pause = false;

    List<IndividualMovement> individuals;
    List<GameObject> IndsOBJ;

    // É o metódo chamado quando se cria a classe, cria os indivíduos e os coloca no array individuals
    void Start()
    {
        individuals = new List<IndividualMovement>();
        IndsOBJ = new List<GameObject>();

        for (int i = 0; i < individualsSize; i++)
        {
            IndsOBJ.Add(InstantiateIndividual());
            individuals.Add(IndsOBJ[i].GetComponent<IndividualMovement>());
            individuals[i].SetTransforms();
        }
    }

    /**
     * Update é chamado a cada frame, 
     * controla o tempo limite de cada rodada
     * e chama o método de finalização de rodada
     * 
     */
    void Update()
    {
        roundTimer -= Time.deltaTime;
        
        if(roundTimer <= 0){

            RoundFinished();

            rounds++;
            roundTimer = timePerRound;
        }
    }

    // A rodada termina, pausa os indivíduos, mede a pontuação, ordena os indivíduos, cruza os indivíduos e reseta as posições
    public void RoundFinished()
    {
        // Pausa os individuos
        foreach (IndividualMovement ind in individuals)
        {
            ind.Pause();
        }

        // Mede a pontuação de cada indivíduo
        foreach (IndividualMovement ind in individuals)
        {
            ind.MeasureScore(transform);
        }

        // Ordena os indivíduos
        individuals.Sort();

        // Cruza os indivíduos sobreescrevendo os indivíduos com pontuação mais baixa
        int idDad = 0;
        int idMom = 1;
        for (int i = 0; i < individualsSize/2; i++)
        {
            individuals[i + individualsSize/2].Breed(individuals[idDad], individuals[idMom]);

            idDad++;
            idMom++;
            if(idMom >= individualsSize)
            {
                idDad = 0;
                idMom = 0;
            }
        }

        /*
        Debug.Log("debug pos breed");
        Debug.Log(individuals[0].score);
        Debug.Log(individuals[1].score);
        Debug.Log(individuals[3].score);

        Debug.Log(individuals[individualsSize - 2].score);
        Debug.Log(individuals[individualsSize - 1].score);
        Debug.Log("debug pos breed");
        */

        // Reinicia as posições dos indivíduos
        for (int i = 0; i < individualsSize; i++)
        {
            GameObject IndObj = InstantiateIndividual();
            IndividualMovement indM = IndObj.GetComponent<IndividualMovement>();
            indM.OverrideData(individuals[i]);
            
            Destroy(individuals[i].transform.gameObject);

            individuals[i] = indM;
        }

        // Despausa os indivíduos e o round vai rodar automaticamente no próximo update
        foreach (IndividualMovement ind in individuals)
        {
            ind.UnPause();
        }
    }

    public GameObject InstantiateIndividual()
    {
        return Instantiate(Resources.Load<GameObject>("SpiderGA"));
    }

}
