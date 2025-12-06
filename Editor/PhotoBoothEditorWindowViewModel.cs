using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Game.Editor.PhotoBooth
{
    public class PhotoBoothEditorWindowViewModel : INotifyBindablePropertyChanged
    {
        private CaptureSettings captureSettings;
        private List<GameObject> captureTargets;
        private readonly ScreenshotRecorder recorder;
        
        [CreateProperty]
        public CaptureSettings CaptureSettings
        {
            get => this.captureSettings;
            set
            {
                this.captureSettings = value;
                this.recorder?.SetCaptureSettings(this.captureSettings);
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

        public PhotoBoothEditorWindowViewModel(CaptureSettings defaultCaptureSettings)
        {
            this.CaptureSettings = defaultCaptureSettings;
            this.CaptureTargets = new List<GameObject>();
            
            this.recorder = new ScreenshotRecorder(this.CaptureSettings);
        }

        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        public void Play()
        {
            UniTask.Void(async () =>
            {
                await this.EnterPlayModeAsync();
            });
        }

        public void Stage()
        {
            UniTask.Void(async () =>
            {
                await this.LoadStagingScene();
            });
        }
        
        public void Capture()
        {
            Debug.Log($"Capturing {this.captureTargets.Count} target(s)...");
            
            UniTask.Void(async () =>
            {
                //await this.EnterPlayModeAsync();

                var tasks = new List<UniTask<string>>();
                foreach (var target in this.captureTargets)
                {
                    Debug.Log($"Capturing {target.name}...");
                    var task = this.CaptureTarget(target);
                    tasks.Add(task);
                    
                    var outputPath = await task;
                    Debug.Log(outputPath);
                }

                //await UniTask.WhenAll(tasks);
                //await this.ExitPlayModeAsync();
            });
        }

        private async UniTask<string> CaptureTarget(GameObject target)
        {
            var instances = await Object.InstantiateAsync(target, Vector3.zero, Quaternion.identity);
            var captureTask = this.recorder.Capture(target.name);

            var results = await UniTask.WhenAll(new[]
            {
                captureTask,
            });

            var outputFilePath = results.First();
            
            foreach (var instance in instances)
            {
                instance.SetActive(false);
            }

            await UniTask.WaitUntil(() => instances.All(i => i.activeSelf == false));
            
            return outputFilePath;
        }
        
        private async UniTask EnterPlayModeAsync()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            EditorApplication.EnterPlaymode();
            await UniTask.WaitUntil(() => EditorApplication.isPlaying);
        }

        private async UniTask ExitPlayModeAsync()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            EditorApplication.ExitPlaymode();
            await UniTask.WaitUntil(() => !EditorApplication.isPlaying);
        }

        private async UniTask LoadStagingScene()
        {
            var activeScene = SceneManager.GetActiveScene(); 
            if (activeScene.name == this.CaptureSettings.Scene.name)
            {
                return;
            }

            await SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }
    }
}
