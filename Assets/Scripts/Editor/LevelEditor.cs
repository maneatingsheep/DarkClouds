using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using View;
using static Model.WaveConfig;

public class LevelEditor : EditorWindow {


    private static LevelConfig[] _levels = new LevelConfig[0];
    private static int _currentLevel;
    private static int _currentWave;
    private static LevelEditor _window;
    private static GUILayoutOption[] _collOptions1;
    private static GUILayoutOption[] _collOptions2;
    private static GUILayoutOption[] _buttOptions;
    private static GUILayoutOption[] _labelOptions;
    private BezierCalculator _bez;

    [MenuItem("Window/Level Editor")]
    static void Init() {
        if (_window == null) {
            _window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
            SceneView.duringSceneGui += _window.OnSceneGUI;
        }

        ReloadData();
        _window.Show();

    }

    private static void ReloadData() {
        string[] assetpaths = AssetDatabase.FindAssets("Level", new string[] { "Assets/Config/Levels/" });
        _levels = new LevelConfig[assetpaths.Length];

        for (int i = 0; i < assetpaths.Length; i++) {

            LevelConfig level = AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetDatabase.GUIDToAssetPath(assetpaths[i]));

            _levels[i] = level;
        }


        _collOptions1 = new GUILayoutOption[] { GUILayout.Width(105), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false) };
        _collOptions2 = new GUILayoutOption[] { GUILayout.Width(130), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false) };
        _buttOptions = new GUILayoutOption[] { GUILayout.Width(25), GUILayout.ExpandWidth(false) };
        _labelOptions = new GUILayoutOption[] { GUILayout.Width(70), GUILayout.ExpandWidth(false) };


        _currentLevel = Mathf.Clamp(_currentLevel, 0, _levels.Length - 1);
        _currentWave = Mathf.Clamp(_currentWave, 0, _levels[_currentLevel].Waves.Length - 1);
    }

    void OnGUI() {
        //main collumn
        EditorGUILayout.BeginHorizontal();
        CreateLevelWaveLists();

        CreateWaveEditor();

        //end main collumn
        EditorGUILayout.EndHorizontal();
    }

    void OnSceneGUI(SceneView sceneview) {

        WaveTiming wt = _levels[_currentLevel].Waves[_currentWave];
        WaveConfig wave = wt.Wave;
        if (wave.MovType == WaveConfig.MovementType.Static) return;

        //Handles.color = Color.red;


        switch (wave.MovType) {
            case MovementType.PathInOut:

                int len = wave.Path.PathDots.Length;


                for (int i = 0; i < wave.Path.PathDots.Length; i++) {
                    
                    wave.Path.PathDots[i].PointPos = Handles.DoPositionHandle(wave.Path.PathDots[i].PointPos, Quaternion.identity);
                    if (i > 0) {
                        wave.Path.PathDots[i].InHandlePos = Handles.DoPositionHandle(wave.Path.PathDots[i].InHandlePos, Quaternion.identity);
                    }
                    if (i < len - 1) {
                        wave.Path.PathDots[i].OutHandlePos = Handles.DoPositionHandle(wave.Path.PathDots[i].OutHandlePos, Quaternion.identity);
                        var dot2 = wave.Path.PathDots[i + 1];

                        Handles.DrawBezier(wave.Path.PathDots[i].PointPos, dot2.PointPos, wave.Path.PathDots[i].OutHandlePos, dot2.InHandlePos, Color.cyan, null, 2);
                    }

                }


                //Handles.DrawLine(wave.Path.PathDots[0].PointPos, wave.Path.PathDots[len - 1].PointPos);


                break;
        }



        // display object "value" in scene
        //GUI.color = color;
        //Handles.Label(pos, t.value.ToString("F1"));
    }


    private void CreateWaveEditor() {
        /*SerializedObject so = new SerializedObject(wave);
        so.Update();
        EditorGUI.BeginChangeCheck();*/

        EditorGUILayout.BeginVertical();
        WaveTiming wt = _levels[_currentLevel].Waves[_currentWave];
        WaveConfig wave = wt.Wave;

        wave.MovType = (MovementType)EditorGUILayout.EnumPopup("Position type", wave.MovType);
        if (wave.MovType == MovementType.Static) {
            wave.StaticHorizPos = EditorGUILayout.FloatField("Horizontal Position", wave.StaticHorizPos);
        } else {
            wave.Path.PathTime = EditorGUILayout.FloatField("PathtravelTime", wave.Path.PathTime);

            wave.GrpType = (GroupType)EditorGUILayout.EnumPopup("Wave type", wave.GrpType);
            
            if (wave.GrpType == GroupType.DelayedGroup) {
                wave.GroupSize = EditorGUILayout.IntField("Group Size", wave.GroupSize);
                wave.GroupSpawnDelay = EditorGUILayout.FloatField("Group Gap (time)", wave.GroupSpawnDelay);
            }
        }

        EditorGUILayout.EndVertical();

        /*SerializedProperty pts = so.FindProperty("Points");
        EditorGUILayout.PropertyField(pts, true);*/

        //finish edit
        /*if (EditorGUI.EndChangeCheck()) {
            so.ApplyModifiedProperties();
        }*/

        if (GUILayout.Button("Calculate path length")) {
            CalculatePathLen(wave);
        }
    }

    private void CalculatePathLen(WaveConfig wave) {

        wave.Path.PathDistances = new float[wave.Path.PathDots.Length - 1];

        float totalDist = 0;

        for (int i = 0; i < wave.Path.PathDots.Length - 1; i++) {
            wave.Path.PathDistances[i] = ApproxBezierLen(
                wave.Path.PathDots[i],
                wave.Path.PathDots[i + 1]);
            totalDist += wave.Path.PathDistances[i];
        }
        for (int i = 0; i < wave.Path.PathDistances.Length; i++) {
            wave.Path.PathDistances[i] /= totalDist;
        }

    }

    private float ApproxBezierLen(MidPointConfig startPos, MidPointConfig endPos) {
        if (_bez == null)_bez = new BezierCalculator();
        int iterations = 10;
        float dist = 0;
        Vector2 lastPos = startPos.PointPos;
        for (int i = 1; i <= iterations; i++) {
            Vector2 p = _bez.CalculatePosition(startPos, endPos, (float)i / (float)iterations);
            dist += (lastPos - p).magnitude;
            lastPos = p;
        }

        return dist;
    }


    private void CreateLevelWaveLists() {
        EditorGUILayout.BeginVertical(_collOptions1);
        for (int i = 0; i < _levels.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            GUI.color = (i == _currentLevel) ? Color.red : Color.white;
            EditorGUILayout.LabelField(_levels[i].name, _labelOptions);
            GUI.color = Color.white;
            if (i != _currentLevel) {
                if (GUILayout.Button("E", _buttOptions)) {
                    _currentLevel = i;
                    _currentWave = 0;
                }
            } 
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(_collOptions2);
        for (int i = 0; i < _levels[_currentLevel].Waves.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            GUI.color = (i == _currentWave) ? Color.red : Color.white;
            EditorGUILayout.LabelField(_levels[_currentLevel].Waves[i].Wave.name, _labelOptions);
            GUI.color = Color.white;
            if (i != _currentWave) {
                if (GUILayout.Button("E", _buttOptions)) {
                    _currentWave = i;
                }
            } 
            _levels[_currentLevel].Waves[i].GapToPrevious = EditorGUILayout.FloatField(_levels[_currentLevel].Waves[i].GapToPrevious, _buttOptions);


            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }


    private void OnFocus() {
        
        Init();
        
    }
}
