﻿using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace TextToMindMapDiagram
{
    public partial class DiagramSaveDialog
    {
#pragma warning disable CS8618
        /// <summary>
        /// Represents the DiagramMenuBar instance that serves as the parent.
        /// </summary>
        internal DiagramMenuBar Parent;

        /// <summary>
        /// The name of the saved diagram file.
        /// </summary>
        public string diagramfileName = "Diagram1";
        /// <summary>
        /// Represents the SfDialog instance for the save dialog.
        /// </summary>
        public SfDialog SaveDialog;
        /// <summary>
        /// Indicates whether the save dialog is currently visible.
        /// </summary>
        public bool SaveDialogVisible = false;
#pragma warning restore CS8618
        /// <summary>
        /// This method is used to save the diagram.
        /// </summary>
        private async Task BtnSave()
        {
            string fileName = await jsRuntime.InvokeAsync<string>("getDiagramFileName", "save");
            await Parent.Download(fileName);
            await SaveDialog.HideAsync();
        }
        /// <summary>
        /// This method is used to close the diaglog box.
        /// </summary>
        private async Task btnCancelClick()
        {
            await SaveDialog.HideAsync();
        }
    }
}
