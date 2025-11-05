using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.PhotoBooth
{
    public class CaptureTarget : INotifyBindablePropertyChanged
    {
        private string name;
        private Texture2D preview;

        [CreateProperty]
        public string Name => this.GameObject.name;

        [CreateProperty]
        public Texture2D Preview
        {
            get => this.preview;
            set
            {
                this.preview = value;
                this.NotifyPropertyChanged();
            }
        }

        public GameObject GameObject { get; set; }
        
        public CaptureTarget(GameObject gameObject)
        {
            this.GameObject = gameObject;
            this.Preview = AssetPreview.GetAssetPreview(this.GameObject);
        }
        
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        
        protected void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }
    }
}
