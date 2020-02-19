using Utils;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameSettings {
    #region public properties
    public int savedLevel => level;
    public int levelsPassed => levelsPassedCount;
    public int savedProgress => localProgress;
    public bool gameStarted => localGameIsStarted;
    public int diamondsCollected => diamonds;
    public bool sound => soundOn.toBool();
    public bool vibration => vibrationOn.toBool();
    #endregion

    private int level = 0;
    private int levelsPassedCount = 0;
    private int diamonds = 0;
    private int soundOn = 0;
    private int vibrationOn = 0;
    private int localProgress = 0; // this is local score in % (refresh on new level loaded)
    private bool localGameIsStarted = false; // local state of the game (refresh on new level loaded)
    private bool globalSettingsInitialized => PlayerPrefs.GetInt("settingsInitialized").toBool();

    private bool debug = false;

    private List<int> notPassedLevels = new List<int>();

    #region private
    private void initializeSavedSettings() {
        DOTween.SetTweensCapacity(500, 250);
        
        level = PlayerPrefs.GetInt("savedLevel");
        diamonds = PlayerPrefs.GetInt("diamondsCollected");
        soundOn = PlayerPrefs.GetInt("soundOn");
        vibrationOn = PlayerPrefs.GetInt("vibrationOn");
        levelsPassedCount = PlayerPrefs.GetInt("levelsPassed");

        localProgress = 0;
        localGameIsStarted = false;

        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
            if(PlayerPrefs.GetInt($"Level_{i}").toBool() == false) {
                notPassedLevels.Add(i);
            }
        }

        if (debug) {
            diamonds = 0;
        }
    }
    #endregion

    #region public
    public GameSettings(bool debugMode, bool firstLaunch) {
        debug = debugMode;

        if (!globalSettingsInitialized || firstLaunch) initializeGlobalSettings();

        initializeSavedSettings();
    }

    public void initializeGlobalSettings() {
        PlayerPrefs.SetInt("savedLevel", 1);
        PlayerPrefs.SetInt("levelsPassed", 0);
        PlayerPrefs.SetInt("diamondsCollected", 0);
        PlayerPrefs.SetInt("soundOn", true.toInt());
        PlayerPrefs.SetInt("vibrationOn", true.toInt());
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) PlayerPrefs.SetInt($"Level_{i}", false.toInt());

        PlayerPrefs.SetInt("settingsInitialized", true.toInt());        
    }

    public void saveLevel(int value) {
        level = value;

        PlayerPrefs.SetInt("savedLevel", level);

        localProgress = 0; // refresh local progress
        localGameIsStarted = false; // stop local game when new scene loaded
    }

    public int addDiamond() {
        diamonds++;

        PlayerPrefs.SetInt("diamondsCollected", diamonds);

        return diamonds;
    }

    public void setSound(bool state){
        soundOn = state.toInt();

        PlayerPrefs.SetInt("soundOn", soundOn);
    }

    public void setVibration(bool state){
        vibrationOn = state.toInt();

        PlayerPrefs.SetInt("vibrationOn", vibrationOn);
    }

    public void saveProgress(int progress) => localProgress = progress;

    public void startLocalGame() => localGameIsStarted = true;

    public void levelPassed() {
        levelsPassedCount++;
        PlayerPrefs.SetInt("levelsPassed", levelsPassedCount);

        PlayerPrefs.SetInt($"Level_{savedLevel}", true.toInt());
        notPassedLevels.Remove(savedLevel);
    }

    public int randomLevel() {
        int randomLevel = Random.Range(1, SceneManager.sceneCountInBuildSettings);

        if (notPassedLevels.Count != 0) return notPassedLevels[Random.Range(0, notPassedLevels.Count)];

        return randomLevel;
    }
    #endregion
}
