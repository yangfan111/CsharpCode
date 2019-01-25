using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Threading;
using Math = System.Math;

namespace JesseStiller.TerrainFormerExtension {
    internal abstract class TerrainCommand {
        protected abstract bool GetUsesShift();
        protected abstract bool GetUsesControl();
        internal abstract string GetName();
        internal abstract void OnClick(object terrainJobData);
        protected abstract void OnShiftClick(object terrainJobData);
        protected abstract void OnShiftClickDown();
        protected abstract void OnControlClick(object terrainJobData);
        
        internal float[,] brushSamples;

        private List<Object> objectsToRegisterForUndo = new List<Object>();
        
        protected static CommandArea commandArea;
        private static ManualResetEvent[] manualResetEvents;
        
        internal TerrainCommand(float[,] brushSamples) {
            this.brushSamples = brushSamples;
        }

        internal void Execute(Event currentEvent, CommandArea commandArea, bool coverEntireTerrainGrid) {
            TerrainCommand.commandArea = commandArea;
            if(this is PaintTextureCommand && TerrainFormerEditor.splatPrototypes.Length == 0) return;
            
            objectsToRegisterForUndo.Clear();
            foreach(TerrainInformation ti in TerrainFormerEditor.Instance.terrainInformations) {
                if(ti.commandArea == null) continue;

                objectsToRegisterForUndo.Add(ti.terrainData);
                
                if(this is PaintTextureCommand) {
                    objectsToRegisterForUndo.AddRange(ti.terrainData.alphamapTextures);
                }
            }

            if(objectsToRegisterForUndo.Count == 0) return;
            
            Undo.RegisterCompleteObjectUndo(objectsToRegisterForUndo.ToArray(), GetName());
            
            foreach(TerrainInformation ti in TerrainFormerEditor.Instance.terrainInformations) {
                if(ti.commandArea == null) continue;
                
                ti.hasChangedSinceLastSetHeights = true;
                ti.hasChangedSinceLastSave = true;
            }

            int tIndex = 0; // Thread index
            int jobCount = Math.Max(Math.Min(System.Environment.ProcessorCount, commandArea.height * 4), 1);
            if(manualResetEvents == null) {
                manualResetEvents = new ManualResetEvent[jobCount];
            } else if(manualResetEvents.Length == jobCount) {
                System.Array.Resize(ref manualResetEvents, jobCount);
            }
            int verticalSpan = Mathf.CeilToInt((float)commandArea.height / jobCount);

            WaitCallback callback;

            // OnControlClick
            if(currentEvent.control) {
                if(GetUsesControl() == false) return;
                callback = OnControlClick;
            }
            // OnShiftClick and OnShiftClickDown
            else if(currentEvent.shift) {
                OnShiftClickDown();

                if(GetUsesShift() == false) return;

                callback = OnShiftClick;
            }
            else {
                // OnClick
                callback = OnClick;
            }

            for(int y = 0; y < jobCount; y++) {
                int yStart = y * verticalSpan;
                int yEnd = Math.Min(yStart + verticalSpan, commandArea.height);

                if(manualResetEvents[tIndex] == null) {
                    manualResetEvents[tIndex] = new ManualResetEvent(false);
                } else {
                    manualResetEvents[tIndex].Reset();
                }

                TerrainJobData terrainJobData = new TerrainJobData(yStart, yEnd, coverEntireTerrainGrid, manualResetEvents[tIndex]);
                ThreadPool.QueueUserWorkItem(callback, terrainJobData);
                tIndex++;
            }
            WaitHandle.WaitAll(manualResetEvents);
        }
    }
}
