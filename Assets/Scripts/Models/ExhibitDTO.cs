﻿using System.Drawing;
using UnityEngine.UI;

namespace GenerationMap
{
    public class ExhibitDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public UnityEngine.UI.Image Icon { get; set; }
        
        public bool IsBlock { get; set; }
        
        public float HeightSpawn { get; set; }
        public string NameComponent { get; set; }
        
        public string LinkOnImage { get; set; }
        public string TextContentFirst { get; set; }
        public string TextContentSecond { get; set; }
        public Point LocalPosition { get; set; }
    }
}