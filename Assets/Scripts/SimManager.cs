﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimManager : MonoBehaviour
{

    static SimManager instance;
    public static SimManager Instance
    {
        get { return instance; }
    }

    public List<PoliceSpawner> PoliceSpawners { get; set; }
    public List<RiotSpawner> RiotSpawners { get; set; }
    public List<CivilianSpawner> CivSpawners { get; set; }

    public List<IAIGuy> PolicePop = new List<IAIGuy>();
    public List<IAIGuy> RiotPop = new List<IAIGuy>();

    // Use this for initialization
    void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        PoliceSpawners = new List<PoliceSpawner>();
        RiotSpawners = new List<RiotSpawner>();
        CivSpawners = new List<CivilianSpawner>();

        //TestGeneticAlgorithm();
    }

    void Start()
    {
        NewRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewRound();
        }
    }

    void TestGeneticAlgorithm()
    {
        IAIGuy one = new Police();
        IAIGuy two = new Police();
        IAIGuy three = new Police();
        IAIGuy four = new Police();
        IAIGuy five = new Police();

        bool[] genes = { true, true, false, false, true, false, false, true, true };
        one.Genes = new BitArray(genes);
        one.fitness = 40;

        genes[0] = false;
        genes[1] = false;
        two.Genes = new BitArray(genes);
        two.fitness = 100;

        genes[2] = true;
        genes[3] = true;
        three.Genes = new BitArray(genes);
        three.fitness = 80;

        genes[4] = false;
        genes[5] = true;
        four.Genes = new BitArray(genes);
        four.fitness = 30;

        five.Genes = new BitArray(genes);
        five.fitness = 50;

        List<IAIGuy> guys = new List<IAIGuy>();
        guys.Add(one);
        guys.Add(two);
        guys.Add(three);
        guys.Add(four);
        guys.Add(five);

        guys = GeneticGeneration.BreedGeneration(guys);

        string output = "";
        foreach (IAIGuy guy in guys)
        {
            foreach (bool b in guy.Genes)
            {
                output += b;
            }
            output += "\n";
        }

        Debug.Log(output);
    }

    void NewRound()
    {
        FallbackPointManager.Instance.NewRound();

        CalculateFitness(PolicePop);
        CalculateFitness(RiotPop);

        PolicePop = GeneticGeneration.BreedGeneration(PolicePop);
        RiotPop = GeneticGeneration.BreedGeneration(RiotPop);

        int pop = PolicePop.Count / PoliceSpawners.Count;
        int spawner = 0;

        List<IAIGuy> population = new List<IAIGuy>();

        for (int i = 0; i < PolicePop.Count; i++)
        {
            population.Add(PolicePop[i]);

            if ((i + 1) % pop == 0)
            {
                PoliceSpawners[spawner].NewRound(population);
                population.Clear();
                spawner++;
            }
        }

        pop = RiotPop.Count / RiotSpawners.Count;
        spawner = 0;

        population = new List<IAIGuy>();

        for (int i = 0; i < RiotPop.Count; i++)
        {
            population.Add(RiotPop[i]);

            if ((i + 1) % pop == 0)
            {
                RiotSpawners[spawner].NewRound(population);
                population.Clear();
                spawner++;
            }
        }
    }

    void CalculateFitness(List<IAIGuy> pop)
    {
        foreach(IAIGuy guy in pop)
        {
            guy.gameObject.SetActive(false);
            guy.CalculateFitness();
        }
    }
}
