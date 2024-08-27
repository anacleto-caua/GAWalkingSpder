using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMaestro : MonoBehaviour
{
    /**
     * Esta classe � respons�vel por controlar o fluxo do algoritmo gen�tico.
     * 
     */

    // Vari�veis de configura��o (s�o definidas no editor, mudar l�)
    public int individualsSize = 10;
    public float timePerRound = 15; // seconds

    // Vari�veis de controle
    public int rounds = 0;
    public float roundTimer;
    public bool pause = false;

    List<IndividualMovement> individuals;
    List<GameObject> IndsOBJ;

    // � o met�do chamado quando se cria a classe, cria os indiv�duos e os coloca no array individuals
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
     * Update � chamado a cada frame, 
     * controla o tempo limite de cada rodada
     * e chama o m�todo de finaliza��o de rodada
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

    // A rodada termina, pausa os indiv�duos, mede a pontua��o, ordena os indiv�duos, cruza os indiv�duos e reseta as posi��es
    public void RoundFinished()
    {
        // Pausa os individuos
        foreach (IndividualMovement ind in individuals)
        {
            ind.Pause();
        }

        // Mede a pontua��o de cada indiv�duo
        foreach (IndividualMovement ind in individuals)
        {
            ind.MeasureScore(transform);
        }

        // Ordena os indiv�duos
        individuals.Sort();

        // Cruza os indiv�duos sobreescrevendo os indiv�duos com pontua��o mais baixa
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

        // Reinicia as posi��es dos indiv�duos
        for (int i = 0; i < individualsSize; i++)
        {
            GameObject IndObj = InstantiateIndividual();
            IndividualMovement indM = IndObj.GetComponent<IndividualMovement>();
            indM.OverrideData(individuals[i]);
            
            Destroy(individuals[i].transform.gameObject);

            individuals[i] = indM;
        }

        // Despausa os indiv�duos e o round vai rodar automaticamente no pr�ximo update
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
