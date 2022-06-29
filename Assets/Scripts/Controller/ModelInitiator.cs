using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInitiator : BaseIntroducer {

    //monobehave refs
    [SerializeField] private Ticker _ticker;
    [SerializeField] private SettingsConfig _settingsConfigRef;


    //internal models
    private MainStateModel _mainStateModelRef;
    private PersistentDataModel _persistentDataModelRef;

    //static ref
    public static ModelInitiator Instance;

    void Awake() {
        Instance = this;
    }

    public void Init(MainStateModel mainStateModel, PersistentDataModel persistentDataModel) {
        _mainStateModelRef = mainStateModel;
        _persistentDataModelRef = persistentDataModel;
    }

    override internal void CheckDependencies() {
        bool allHere = true;
        allHere &= _settingsConfigRef != null;
        allHere &= _ticker != null;

        if (!allHere) {
            throw new Exception("missing model refs");
        }
    }


    public static MainStateModel GetMainStateModel() {
        Instance.CheckAccess();
        return Instance._mainStateModelRef;
    }


    public static PersistentDataModel GetPersistentDataModel() {
        Instance.CheckAccess();
        return Instance._persistentDataModelRef;
    }

    public static SettingsConfig GetSettingsConfig() {
        Instance.CheckAccess();
        return Instance._settingsConfigRef;
    }

    public static Ticker Getticker() {
        Instance.CheckAccess();
        return Instance._ticker;
    }

}
