using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "ScriptableObject/MapSelectionData")]
public class MapSelectionData : ScriptableObject
{
    public List<MapInfo> Maps;


    [Serializable]
    public struct MapInfo
    {
        public Sprite MapThumbnail;
        public string MapName;
        public string SceneName;
    }
}
