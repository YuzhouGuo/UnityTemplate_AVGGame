using System;
using Assets.Script.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEditor.Localization;

namespace Assets.Script.VisualScript{

    [UnitShortTitle("Line")]
    [UnitCategory("AVG Game Nodes")]
    public class VisualLine : Unit{
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput stringTable;

        [DoNotSerialize]
        public ValueInput actor;

        [DoNotSerialize]
        public ValueInput ffamily;

        [DoNotSerialize]
        public ValueInput fcolor;

        [DoNotSerialize]
        public ValueInput fsize;

        [DoNotSerialize]
        public ValueInput linespacing;

        [DoNotSerialize]
        public ValueInput align;

        [DoNotSerialize]
        public ValueInput fstyle;

        protected override void Definition(){
            inputTrigger = ControlInput("in", (flow) => { return outputTrigger; });
            outputTrigger = ControlOutput("out");

            //stringTableCollection = ValueInput<StringTableCollection>("string table", LocalizationEditorSettings.GetStringTableCollection("test"));
            //text = ValueInput<LocalizedString>("text", new LocalizedString { TableReference = "test", TableEntryReference = "New Entry" });
            // var collection = LocalizationEditorSettings.GetStringTableCollection("test");
            // var entry = collection.SharedData.GetEntry("New Entry");
            stringTable = ValueInput<StringTableCollection>("stringTable", LocalizationEditorSettings.GetStringTableCollection("test"));
            actor = ValueInput<Actor>("actor", Actor.NULL);

            ffamily = ValueInput<string>("ffamily", string.Empty);
            fcolor = ValueInput<string>("fcolor", "0x6079d2ff");
            fsize = ValueInput<float>("fsize", 15.0f);
            linespacing = ValueInput<float>("linespacing", 0.0f);
            align = ValueInput<Align>("align", Align.LT);
            fstyle = ValueInput<FontStyle>("fstyle", FontStyle.Normal);         
        }
    }
}
