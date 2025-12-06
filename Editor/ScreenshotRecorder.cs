using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace Game.Editor.PhotoBooth
{
    public class ScreenshotRecorder
    {
        private readonly RecorderController recorderController;
        private readonly ImageRecorderSettings imageRecorder;
        private readonly string mediaOutputFolder;
        private CaptureSettings captureSettings;

        public ScreenshotRecorder(CaptureSettings captureSettings)
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            this.recorderController = new RecorderController(controllerSettings);

            this.mediaOutputFolder = Path.Combine(Application.dataPath, "..", "Recordings");

            // Image
            this.imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            this.imageRecorder.name = "ScreenshotRecorder";
            this.imageRecorder.Enabled = true;
            this.imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            this.imageRecorder.CaptureAlpha = false;
            this.SetCaptureSettings(captureSettings);

            // Setup Recording
            controllerSettings.AddRecorderSettings(this.imageRecorder);
            controllerSettings.SetRecordModeToSingleFrame(0);
        }

        public async UniTask<string> Capture(string name)
        {
            this.imageRecorder.OutputFile = Path.Combine(this.mediaOutputFolder, $"{name}_") + DefaultWildcard.Take;
            this.recorderController.PrepareRecording();
            this.recorderController.StartRecording();
            
            while (this.recorderController.IsRecording())
            {
                await UniTask.DelayFrame(1);
            }
            
            return this.imageRecorder.OutputFile;
        }

        public void SetCaptureSettings(CaptureSettings settings)
        {
            this.captureSettings = settings;
            
            this.imageRecorder.imageInputSettings = new GameViewInputSettings
            {
                OutputWidth = 800,
                OutputHeight = 600,
            };
        }
    }
}
