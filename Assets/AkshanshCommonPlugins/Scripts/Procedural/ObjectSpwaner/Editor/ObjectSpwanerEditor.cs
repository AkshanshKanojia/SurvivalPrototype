using UnityEditor;
using UnityEngine;

namespace AkshanshKanojia.LevelEditors
{
    [CustomEditor(typeof(ObjectSpwaner))]
    public class ObjectSpwanerEditor : Editor
    {
        bool showGrid = true, showGenerationProp, showDebugProperties;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var _tempMang = target as ObjectSpwaner;
            switch (_tempMang.GenerationMode)
            {
                case ObjectSpwaner.AvailableGenerationModes.RandomGrid:
                case ObjectSpwaner.AvailableGenerationModes.OrderedGrid:
                    //Grid Properties
                    showGrid = EditorGUILayout.Foldout(showGrid, "Grid Properties");
                    if (showGrid)
                    {
                        _tempMang.GridCellSize = EditorGUILayout.FloatField("Grid Size", _tempMang.GridCellSize);
                        _tempMang.GridXSize = EditorGUILayout.IntField("X Size", _tempMang.GridXSize);
                        _tempMang.GridZSize = EditorGUILayout.IntField("Z Size", _tempMang.GridZSize);
                        _tempMang.RandomizeInsideCell = EditorGUILayout.Toggle("RandomizeObjectInCell", _tempMang.RandomizeInsideCell);
                    }
                    break;
            }

            if (_tempMang.GenerationMode != ObjectSpwaner.AvailableGenerationModes.RemoveRandomObjects)
            {
                //generation properties
                showGenerationProp = EditorGUILayout.Foldout(showGenerationProp, "Generation Properties");
                if (showGenerationProp)
                {
                    //overlap properties
                    _tempMang.AvoidObjectOverlaps = EditorGUILayout.Toggle("Try Avoiding Overlaps", _tempMang.AvoidObjectOverlaps);
                    if (_tempMang.AvoidObjectOverlaps)
                    {
                        EditorGUILayout.HelpBox("Make sure objects you are spwaning have a collider in it " +
                            "to detect overlapping!", MessageType.Info);
                        _tempMang.OverlapDetectionRadius = EditorGUILayout.FloatField("Overlap Avoidance Range", _tempMang.OverlapDetectionRadius);
                        _tempMang.MaxOverlapItteration = EditorGUILayout.IntSlider("Max Avoidance itteration", _tempMang.MaxOverlapItteration, 1, 1000);
                        _tempMang.SkipOnOverlap = EditorGUILayout.Toggle("Skip On Overlap", _tempMang.SkipOnOverlap);
                    }
                    if (GUILayout.Button("Generate Objects"))
                    {
                        _tempMang.GenerateObjects();
                    }
                }
            }

            //debug properties
            showDebugProperties = EditorGUILayout.Foldout(showDebugProperties, "Debug Properties");
            if (showDebugProperties)
            {
                if (_tempMang.GenerationMode == ObjectSpwaner.AvailableGenerationModes.RandomGrid ||
                    _tempMang.GenerationMode == ObjectSpwaner.AvailableGenerationModes.OrderedGrid)
                {
                    _tempMang.ShowGrid = EditorGUILayout.Toggle("Show Grid", _tempMang.ShowGrid);
                    _tempMang.GridVertSize = EditorGUILayout.FloatField("Vert Size", _tempMang.GridVertSize);
                }
                else
                {
                    _tempMang.ShowGrid = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    _tempMang.SetGridDebug();
                }
            }
        }
    }
}
