using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.PhotoBooth
{
    public class PhotoBoothEditorWindowViewModel : INotifyBindablePropertyChanged
    {
        private CaptureSettings captureSettings;
        private List<GameObject> captureTargets;

        [CreateProperty]
        public CaptureSettings CaptureSettings
        {
            get => this.captureSettings;
            set
            {
                this.captureSettings = value;
                this.NotifyPropertyChanged();
            }
        }
        [CreateProperty]
        public List<GameObject> CaptureTargets
        {
            get => this.captureTargets;
            set
            {
                this.captureTargets = value;
                this.NotifyPropertyChanged();
            }
        }

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        public PhotoBoothEditorWindowViewModel()
        {
            this.CaptureTargets = new List<GameObject>();
        }

        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        public void Capture()
        {
            Debug.Log($"Capturing {this.captureTargets.Count} target(s)...");
            
            foreach (var target in this.captureTargets)
            {
                Debug.Log($"Capturing {target.name}...");
            }
        }
    }
}
