using Utils;
using System;
using System.Linq;
using UnityEngine;

using static UnityEngine.Random;

public class Stage : MonoBehaviour {
    #region editor
    [Header("Score Settings")]
    [SerializeField] private RandomizedFloat gunScoreCost = new MinMaxValue(100f, 200f);

    [Space][Header("Guns Settings")]
    [SerializeField] private Transform gunsHolder = default;
    [SerializeField] private bool generateGunsSettings = true;

    [Space][Header("Toy Settings")]
    [SerializeField] private ToyBridge toyBrigde = default; 
    [SerializeField] private ToyPlatform toyPlatform = default;
    #endregion

    #region public events
    public Action failed;
    public Action completed;
    public Action<float, int, int, bool> scoreUpdated; // float because format must be (0f, 1f)
    #endregion

    #region public properties
    public int goalScore => targetScore;
    public int gunsCount => guns.Length;
    public ToyBridge bridge => toyBrigde;
    public ToyPlatform platform => toyPlatform;
    #endregion

    private int targetScore = 0;
    private bool haveGuns => gunsHolder != null;

    private Toy toy;
    private Gun[] guns;

    #region private
    private void Awake() {
        if(!haveGuns) return;

        guns = gunsHolder.GetComponentsInChildren<Gun>();
        if(toyBrigde == null) toyBrigde = GetComponentInChildren<ToyBridge>();
        if(toyPlatform == null) toyPlatform = GetComponentInChildren<ToyPlatform>();

        gunScoreCost = new RandomizedFloat(true, new MinMaxValue(80, 160)); // ...

        // generate gun score cost randomly
        targetScore = Enumerable.Range(0, guns.Length).Select(i => Mathf.FloorToInt(gunScoreCost.randomizedValue)).Sum();
    }

    private void onToyBrokeDown(){
        failed?.Invoke();

        toy.canMove = false; // deactivate user controller
        toy.brokeDown -= onToyBrokeDown;
        toy.scoreUpdated -= onToyScoreUpdated;
    }

    private void onToyScoreUpdated(int score, int value, int ballsInARow, bool fever){
        scoreUpdated?.Invoke(((float)score).map(0f, targetScore, 0f, 1f), value, ballsInARow, fever); // invoke score in % and localValue

        if (score >= targetScore) {
            completed?.Invoke();

            toy.canMove = false; // deactivate user controller
            toy.brokeDown -= onToyBrokeDown;
            toy.scoreUpdated -= onToyScoreUpdated;

            // fixing situation when 1 more bullet riched toy after stage completed
            Array.ForEach(FindObjectsOfType<BulletBase>(), bullet => bullet.returnToPool()); 
        }

        //print($"score: {score}, {((float)score).map(0f, targetScore, 0f, 1f)}");
    }

    private void prepareGuns(){
        if(!haveGuns) return;

        if(generateGunsSettings){
            for(int i = 0; i < guns.Length; i++){
                Gun gun = guns[i];

                bool isFirstGun = i == 0;

                GunCannon.CannonSettings settings = new GunCannon.CannonSettings();

                settings.speedDelta = Range(.2f, .3f);
                settings.speed = new RandomizedFloat(true, new MinMaxValue(70f, 90f), 75f);

                // shootDelay                
                float minShootDelay = 0f;
                float maxShootDelay = .25f;
                if(isFirstGun) minShootDelay = maxShootDelay = 0f;
                settings.shootDelay = new RandomizedFloat(true, new MinMaxValue(minShootDelay, maxShootDelay));

                // bomb spawn chance
                float minBombSpawnChance = 0f;
                float maxBombSpawnChance = 1f;
                if(!isFirstGun) {
                    minBombSpawnChance = Range(0f, .5f);
                    maxBombSpawnChance = Range(.5f, 1f);
                } else maxBombSpawnChance = Range(0f, .2f);
                settings.bombSpawnChance = new RandomizedFloat(true, new MinMaxValue(minBombSpawnChance, maxBombSpawnChance));
                
                // activation delay
                float activationDelta = 5f;
                float minActivationDelay = Range(i * activationDelta, (i + 1) * activationDelta);
                float maxActivationDelay = guns.Length * activationDelta;
                if(isFirstGun) minActivationDelay = maxActivationDelay = 0f;
                RandomizedFloat activationDelay = new RandomizedFloat(true, new MinMaxValue(minActivationDelay, maxActivationDelay));

                gun.prepareCannonSettings(settings, activationDelay);
            }
        }

        Array.ForEach(guns, gun => gun.prepare());
    }
    #endregion

    #region public
    public void activateGuns(bool state = true) {
        if(!haveGuns) return;
        
        Array.ForEach(guns, gun => gun.activate(state));
    }

    public void prepare(Toy toyToPrepare) {
        prepareGuns();

        toy = toyToPrepare;
        toy.score = 0;
        toy.canMove = true;
        toy.platform = toyPlatform;
        toy.brokeDown += onToyBrokeDown;
        toy.scoreUpdated += onToyScoreUpdated;
    }
    #endregion
}
