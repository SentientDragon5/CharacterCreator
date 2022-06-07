using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEditor;
namespace Creator
{
    [CreateAssetMenu(fileName = "CharacterCreator", menuName = "Character Creator")]
    public class CharacterCreator : ScriptableObject
    {
        public Layer[] layers;

        public int[] choices;
        public Vector2Int sliceSize;


        public string assetPath;
        public string _name;


        Texture2D Add(Texture2D a, Texture2D b)
        {
            if (a == null && b == null)
                return null;

            if (a == null)
                return b;
            if (b == null)
                return a;

            Texture2D o = Instantiate(a);
            for (int x = 0; x < a.width; x++)
            {
                for (int y = 0; y < a.height; y++)
                {
                    if (b.GetPixel(x, y).a > 0)
                        o.SetPixel(x, y, b.GetPixel(x, y));
                }
            }
            o.Apply();
            return o;
        }
        Texture2D ClearBG(int w, int h)
        {
            Texture2D o = new Texture2D(w, h);
            for (int x = 0; x < o.width; x++)
            {
                for (int y = 0; y < o.height; y++)
                {
                    o.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
            o.Apply();
            return o;
        }
        [ContextMenu("Bake Sprite Sheet")]
        public void BakeSpriteSheet()
        {
            Texture2D old = layers[0].options[0];
            Texture2D tex = ClearBG(old.width, old.height);

            for (int l = 0; l < layers.Length; l++)
            {
                if (choices[l] == -1)
                    continue;
                else if (l < choices.Length && l > -1)
                    tex = Add(tex, layers[l].options[choices[l]]);
            }

            string cardPath = "Assets/" + assetPath + _name + ".png";
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(cardPath, bytes);
            AssetDatabase.ImportAsset(cardPath);
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(cardPath);
            ti.textureType = TextureImporterType.Sprite;
            ti.filterMode = FilterMode.Point;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            ti.isReadable = true;
            List<SpriteMetaData> newData = new List<SpriteMetaData>();
            for (int i = 0; i < tex.width; i += sliceSize.x)
            {
                for (int j = tex.height; j > 0; j -= sliceSize.y)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (tex.height - j) / sliceSize.y + ", " + i / sliceSize.x;
                    smd.rect = new Rect(i, j - sliceSize.y, sliceSize.x, sliceSize.y);

                    newData.Add(smd);
                }
            }
            ti.spritesheet = newData.ToArray();
            ti.SaveAndReimport();
        }

        [MenuItem("Sprites/Rename Sprites")]
        static void SetSpriteNames()
        {
            Texture2D myTexture = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/MyTexture.png");

            string path = AssetDatabase.GetAssetPath(myTexture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            int SliceWidth = 16;
            int SliceHeight = 16;

            for (int i = 0; i < myTexture.width; i += SliceWidth)
            {
                for (int j = myTexture.height; j > 0; j -= SliceHeight)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (myTexture.height - j) / SliceHeight + ", " + i / SliceWidth;
                    smd.rect = new Rect(i, j - SliceHeight, SliceWidth, SliceHeight);

                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        private void OnValidate()
        {
            
        }
        public Texture2D GetTex(int i)
        {
            if (i == -1)
                return null;
            return Crop(layers[i].options[choices[i]]);
        }
        public Texture2D preview;
        public void CreatePreview()
        {
            Texture2D old = Crop(layers[0].options[0]);
            Texture2D tex = ClearBG(old.width, old.height);

            for (int l = 0; l < layers.Length; l++)
            {
                if (choices[l] == -1)
                    continue;
                if (l < choices.Length && l > -1)
                    tex = Add(tex, layers[l].options[choices[l]]);
            }
            preview = Instantiate(tex);
            DestroyImmediate(tex);
        }
        public Texture2D Crop(Texture2D t)
        {
            if (t == null)
                return null;
            Texture2D o = ClearBG(sliceSize.x, sliceSize.y);
            for (int x = 0; x < o.width; x++)
            {
                for (int y = 0; y < o.height; y++)
                {
                    o.SetPixel(x, y, t.GetPixel(x, y));
                }
            }
            o.Apply();
            return o;
        }
    }
    [System.Serializable]
    public class Layer
    {
        public Texture2D[] options;
        public bool required;

        public Layer(Texture2D[] options)
        {
            this.options = options;
        }
    }
}