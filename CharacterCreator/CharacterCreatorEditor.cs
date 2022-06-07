
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace Creator
{

    [CustomEditor(typeof(CharacterCreator))]
    public class CharacterCreatorEditor : Editor
    {
        CharacterCreator t;
        bool configureFoldout = false;
        private void OnEnable()
        {
            t = (CharacterCreator)target;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //EditorGUILayout.LabelField("")
            EditorGUILayout.LabelField("To Bake to an atlas Click the Character.");
            EditorGUILayout.BeginHorizontal();
            //if (GUILayout.Button("Bake")) { t.BakeSpriteSheet(); }
            EditorGUILayout.EndHorizontal();


            //Save
            //EditorGUILayout.LabelField("Save/Load the current map to an asset");
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("save"), true);


            //EditorGUILayout.PropertyField(serializedObject.FindProperty("save"), true);
            configureFoldout = EditorGUILayout.Foldout(configureFoldout, "Configure");
            if (configureFoldout)
            {
                DrawDefaultInspector();
            }
            //if (GUILayout.Button("Create Preview")) { t.CreatePreview(); }
            t.CreatePreview();
            if (t.preview != null)
            {
                if (GUILayout.Button(t.preview))
                {

                    t.BakeSpriteSheet();
                }

            }
            for (int i = 0; i < t.layers.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (t.choices[i] == -1)
                {
                    if(GUILayout.Button("Show Layer "+i))
                    {
                        t.choices[i] = 0;
                    }
                }
                else
                {
                    if(t.layers[i].required)
                    {
                        //EditorGUILayout.LabelField("Required");
                        if (GUILayout.Button("Layer\nRequired")) { }
                        else { CharacterCreatorLayer(i);}

                    }
                    else
                    {
                        if (GUILayout.Button("Hide\nLayer"))
                        {
                            t.choices[i] = -1;
                        }
                        else
                        {
                            CharacterCreatorLayer(i);
                        }
                    }
                    
                }
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(t);
        }

        public void CharacterCreatorLayer(int i)
        {
            //MakePreviewLayers();
            //if(previewLayers == null)
            // MakePreviewLayers();

            t.choices[i] -= 1;
            t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
            if (!GUILayout.Button(t.GetTex(i)))//GUILayout.Button("Left"))
            {

                t.choices[i] += 1;
                t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
                //MakePreviewLayers();
            }

            if (GUILayout.Button(t.GetTex(i)))
            {

            }

            t.choices[i] += 1;
            t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
            if (!GUILayout.Button(t.GetTex(i)))//GUILayout.Button("Right"))
            {
                t.choices[i] -= 1;
                t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
                //MakePreviewLayers();
            }
        }

        public Layer[] previewLayers;

        public void MakePreviewLayers()
        {
            previewLayers = new Layer[t.layers.Length];
            for (int i = 0; i < previewLayers.Length; i++)
            {
                previewLayers[i] = new Layer(new Texture2D[3]);

                t.choices[i] -= 1;
                t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
                previewLayers[i].options[0] = t.GetTex(i);

                t.choices[i] += 1;
                t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
                previewLayers[i].options[0] = t.GetTex(i);

                t.choices[i] += 1;
                t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
                previewLayers[i].options[0] = t.GetTex(i);

                t.choices[i] -= 1;
                t.choices[i] = GetNext(t.choices[i], 0, t.layers[i].options.Length);
            }
        }
        public int GetNext(int i, int min, int max)
        {
            while (i >= max)
                i -= max;
            while (i < min)
                i += max;
            return i;
        }
    }
}