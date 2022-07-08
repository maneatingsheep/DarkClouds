using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using View;
using static Model.PathConfig;
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

    private static ReorderableList _waveList;
    private static BezierCalculator _bez;
    private static SerializedObject _levelsSerObj;

    [MenuItem("Window/Level Editor")]
    static void Init() {
        if (_window == null) {
            _window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
            SceneView.duringSceneGui += _window.OnSceneGUI;

            _collOptions1 = new GUILayoutOption[] { GUILayout.Width(105), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false) };
            _collOptions2 = new GUILayoutOption[] { GUILayout.Width(200), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false) };
            _buttOptions = new GUILayoutOption[] { GUILayout.Width(25), GUILayout.ExpandWidth(false) };
            _labelOptions = new GUILayoutOption[] { GUILayout.Width(70), GUILayout.ExpandWidth(false) };

        }

        ReloadLevelData();
        _window.Show();
    }

    private void OnDisable() {
        KillExistingList();
    }

    private static void ReloadWaveList() {
        KillExistingList();
        _levelsSerObj = new SerializedObject(_levels[_currentLevel]);
        

        SerializedProperty waves = _levelsSerObj.FindProperty("Waves");

        _waveList = new ReorderableList(_levelsSerObj, waves, true, true, true, true);

        _waveList.showDefaultBackground = true;

        _waveList.drawElementCallback = DrawWaveListElement;
        _waveList.drawHeaderCallback = DrawWaveHeader;
        _waveList.onSelectCallback = ListItemSelected;
    }

    private static void ListItemSelected(ReorderableList list) {
        _currentWave = list.index;
    }

    private static void DrawWaveHeader(Rect rect) {
        EditorGUI.LabelField(rect, "Waves");
    }

    private static void KillExistingList() {
        if (_waveList != null) {
            _waveList.drawElementCallback = null;
            _waveList.drawHeaderCallback = null;
        }
    }

    private static void ReloadLevelData() {
        string[] assetpaths = AssetDatabase.FindAssets("Level", new string[] { "Assets/Config/Levels/" });
        _levels = new LevelConfig[assetpaths.Length];

        for (int i = 0; i < assetpaths.Length; i++) {

            LevelConfig level = AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetDatabase.GUIDToAssetPath(assetpaths[i]));

            _levels[i] = level;
        }


        _currentLevel = Mathf.Clamp(_currentLevel, 0, _levels.Length - 1);
        _currentWave = Mathf.Clamp(_currentWave, 0, _levels[_currentLevel].Waves.Length - 1);
        ReloadWaveList();
    }

    void OnGUI() {

        EditorGUILayout.BeginHorizontal();
        DrawLevelLists();
        DrawWaveLists();

        DrawWaveEditor();

        //end main collumn
        EditorGUILayout.EndHorizontal();

    }

    void OnSceneGUI(SceneView sceneview) {

        if (_levels == null || _levels.Length == 0) return;
        if (_levels[_currentLevel].Waves == null || _levels[_currentLevel].Waves.Length == 0) return;


        WaveConfig wave = _levels[_currentLevel].Waves[_currentWave];

        PathConfig path = wave.Path;

        if (!(wave.Enemy is IPathFollow)) return;
        
        int len = wave.Path.PathDots.Length;

        for (int i = 0; i < path.PathDots.Length; i++) {

            path.PathDots[i].PointPos = Handles.DoPositionHandle(path.PathDots[i].PointPos, Quaternion.identity);
            if (i > 0) {
                path.PathDots[i].InHandlePos = Handles.DoPositionHandle(path.PathDots[i].InHandlePos, Quaternion.identity);
            }
            if (i < len - 1) {
                path.PathDots[i].OutHandlePos = Handles.DoPositionHandle(path.PathDots[i].OutHandlePos, Quaternion.identity);
                var dot2 = path.PathDots[i + 1];

                Handles.DrawBezier(path.PathDots[i].PointPos, dot2.PointPos, path.PathDots[i].OutHandlePos, dot2.InHandlePos, Color.cyan, null, 2);
            }

        }

        
    }

    private static void DrawWaveListElement(Rect rect, int index, bool isActive, bool isFocused) {

        SerializedProperty element = _waveList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

        EditorGUI.LabelField(rect, "Wave " + index);
        EditorGUI.PropertyField(
            new Rect(rect.x + 120, rect.y, rect.width - 120, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("GapToPrevious"),
            GUIContent.none
        );

        /*EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
            element,
            GUIContent.none
        );*/

        /*if (element.isExpanded) {
            EditorGUI.PropertyField(
                new Rect(rect.x + 30, rect.y, 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Enemy"),
                GUIContent.none
           );

            EditorGUI.PropertyField(
                new Rect(rect.x + 30, rect.y, 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Formation"),
                GUIContent.none
            );
        }*/

    }

    private void DrawWaveEditor() {

        if (_levels == null || _levels.Length == 0) return;
        if (_levels[_currentLevel].Waves == null || _levels[_currentLevel].Waves.Length == 0) return;

        WaveConfig wave = _levels[_currentLevel].Waves[_currentWave];

        EditorGUILayout.BeginVertical();


        EditorGUI.BeginChangeCheck();

       
        EditorGUILayout.PropertyField(_waveList.serializedProperty.GetArrayElementAtIndex(_currentWave), new GUIContent("enemy and movement"));


        if (EditorGUI.EndChangeCheck()) {
            _levelsSerObj.ApplyModifiedProperties();
        }


        if (wave.Enemy.Prefab is IStationary) {
            wave.StaticHorizPos = EditorGUILayout.FloatField("Horizontal Position", wave.StaticHorizPos);
        }
        if (wave.Enemy.Prefab is IPathFollow) {
            wave.LifeTime = EditorGUILayout.FloatField("PathtravelTime", wave.LifeTime);

            wave.GrpType = (GroupType)EditorGUILayout.EnumPopup("Wave type", wave.GrpType);

            if (wave.GrpType == GroupType.DelayedGroup) {
                wave.GroupSize = EditorGUILayout.IntField("Group Size", wave.GroupSize);
                wave.GroupSpawnDelay = EditorGUILayout.FloatField("Group Gap (time)", wave.GroupSpawnDelay);
            }
        }
        

        if (GUILayout.Button("Calculate path length")) {
            CalculatePathLen(wave.Path);
        }

        EditorGUILayout.EndVertical();


        


    }

    private void DrawLevelLists() {
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
    }

    private void DrawWaveLists() {

        _levelsSerObj.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginVertical(_collOptions2);
        _waveList.DoLayoutList();
        EditorGUILayout.EndVertical();


        if (EditorGUI.EndChangeCheck()) {
            _levelsSerObj.ApplyModifiedProperties();
        }

    }

    private void CalculatePathLen(PathConfig path) {

        path.PathDistances = new float[path.PathDots.Length - 1];

        float totalDist = 0;

        for (int i = 0; i < path.PathDots.Length - 1; i++) {
            path.PathDistances[i] = ApproxBezierLen(
                path.PathDots[i],
                path.PathDots[i + 1]);
            totalDist += path.PathDistances[i];
        }
        for (int i = 0; i < path.PathDistances.Length; i++) {
            path.PathDistances[i] /= totalDist;
        }

    }
    private float ApproxBezierLen(MidPointConfig startPos, MidPointConfig endPos) {
        if (_bez == null) _bez = new BezierCalculator();
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

    private void OnFocus() {
        
        Init();
        
    }

}
