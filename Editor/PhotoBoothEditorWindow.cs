using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.PhotoBooth
{
    public class PhotoBoothEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        [SerializeField]
        private VisualTreeAsset gridViewItemTreeAsset;
        private PhotoBoothEditorWindowViewModel viewModel;
        private GridView gridView;
        private DropZone dropZone;
        private ActionButton captureButton;
        
        [UnityEditor.MenuItem("Tools/Photo Booth")]
        public static void ShowExample()
        {
            var wnd = GetWindow<PhotoBoothEditorWindow>();
            wnd.titleContent = new GUIContent(nameof(PhotoBoothEditorWindow));
        }

        public void CreateGUI()
        {
            this.viewModel = new PhotoBoothEditorWindowViewModel();
            
            var root = this.rootVisualElement;
            var template = this.visualTreeAsset.Instantiate();
            root.Add(template);
            template.dataSource = this.viewModel;
            
            this.gridView = root.Q<GridView>("grid-view");
            this.dropZone = root.Q<DropZone>("drop-zone");
            this.captureButton = root.Q<ActionButton>("capture-button");

            this.gridView.itemsSource = this.viewModel.CaptureTargets;

            this.gridView.columnCount = 4;
            this.gridView.makeItem = this.gridViewItemTreeAsset == null
                ? () =>
                {
                    var container = new VisualElement();
                    var text = new Text()
                    {
                        text = "<NOT BOUND>"
                    };

                    container.Add(text);

                    return container;
                }
                : this.gridViewItemTreeAsset.CloneTree;
            this.gridView.bindItem = (element, i) =>
            {
                var item = this.gridView.itemsSource[i] as GameObject;
                var text = element.Q<Text>();
                text.text = item?.name;
            };

            this.gridView.selectionChanged += OnGridViewSelectionChanged;

            void OnGridViewSelectionChanged(IEnumerable<object> objs)
            {
                foreach (var obj in objs)
                {
                    Debug.Log(obj);
                }
            }

            this.gridView.Refresh();
            
            this.dropZone.controller.acceptDrag = AcceptDrag;
            this.captureButton.clicked += this.OnCaptureButtonClicked;
            this.dropZone.controller.dropped += this.OnDropZoneDropped;
            this.dropZone.controller.dragEnded += () => this.UpdateView(false);
            
            root.RegisterCallback<DragUpdatedEvent>(_ => this.UpdateView(true));
            root.RegisterCallback<DragLeaveEvent>(_ => this.UpdateView(false));
            root.RegisterCallback<DragExitedEvent>(_ => this.UpdateView(false));

            this.UpdateView(false);
            bool AcceptDrag(IEnumerable<object> arg)
            {
                return arg.Any(obj => obj is GameObject);
            }
        }
        
        void UpdateView(bool showDropZone)
        {
            this.dropZone.visibleIndicator = showDropZone;
            this.gridView.Refresh();
        }

        private void OnDropZoneDropped(IEnumerable<object> objects)
        {
            foreach (var gameObject in objects
                         .OfType<GameObject>())
            {
                if (this.viewModel.CaptureTargets.Contains(gameObject))
                {
                    continue;
                }
                
                this.viewModel.CaptureTargets.Add(gameObject);
            }
        }

        private void OnDisable()
        {
            this.captureButton.clicked -= this.OnCaptureButtonClicked;
        }

        private void OnCaptureButtonClicked()
        {
            this.viewModel.Capture();
        }
    }
}
