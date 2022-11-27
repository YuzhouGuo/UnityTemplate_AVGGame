using System;
using Assets.Script.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEditor.Localization;

namespace Assets.Script.VisualScript{
    public class VisualLine : Unit{
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput stringTable;

        [DoNotSerialize]
        public ValueInput actor;

        protected override void Definition(){
            inputTrigger = ControlInput("in", (flow) => { return outputTrigger; });
            outputTrigger = ControlOutput("out");

            //stringTableCollection = ValueInput<StringTableCollection>("string table", LocalizationEditorSettings.GetStringTableCollection("test"));
            //text = ValueInput<LocalizedString>("text", new LocalizedString { TableReference = "test", TableEntryReference = "New Entry" });
            // var collection = LocalizationEditorSettings.GetStringTableCollection("test");
            // var entry = collection.SharedData.GetEntry("New Entry");
            stringTable = ValueInput<StringTableCollection>("stringTable", LocalizationEditorSettings.GetStringTableCollection("test"));
            actor = ValueInput<Actor>("actor", Actor.NULL);
        }
    }
}
