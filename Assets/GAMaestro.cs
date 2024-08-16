using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMaestro : MonoBehaviour
{

    public int individualsSize = 10;
    public float roundTimer;

    public float timePerRound = 15; // seconds
    public int rounds = 0;

    public bool pause = false;

    List<IndividualMovement> individuals;
    List<GameObject> IndsOBJ;

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

    void Update()
    {
        roundTimer -= Time.deltaTime;
        
        if(roundTimer <= 0){

            RoundFinished();

            rounds++;
            roundTimer = timePerRound;
        }
    }

    public void RoundFinished()
    {
        //Pause individuals
        foreach (IndividualMovement ind in individuals)
        {
            ind.Pause();
        }

        //Get the pontuations
        foreach (IndividualMovement ind in individuals)
        {
            ind.MeasureScore(transform);
        }

        //Order the individuals
        individuals.Sort();

        //Breeding lesser ones out
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

        Debug.Log("debug pos breed");
        Debug.Log(individuals[0].score);
        Debug.Log(individuals[1].score);
        Debug.Log(individuals[3].score);

        Debug.Log(individuals[individualsSize - 2].score);
        Debug.Log(individuals[individualsSize - 1].score);
        Debug.Log("debug pos breed");

        //Reset positions
        for (int i = 0; i < individualsSize; i++)
        {
            GameObject IndObj = InstantiateIndividual();
            IndividualMovement indM = IndObj.GetComponent<IndividualMovement>();
            indM.OverrideData(individuals[i]);
            
            Destroy(individuals[i].transform.gameObject);

            individuals[i] = indM;
        }

        // UnPause individuals
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
