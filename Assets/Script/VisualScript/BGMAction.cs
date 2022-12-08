using System;
using Assets.Script.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Search;

using System.Collections.Generic;

namespace UnityEditor.Localization.UI{

    [UnitShortTitle("Play BGM")]
    [UnitCategory("AVG Game Nodes")]
    public class BGMAction : Unit{
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput bgm;

        protected override void Definition(){
            inputTrigger = ControlInput("in", (flow) => { return outputTrigger; });
            outputTrigger = ControlOutput("out");

            bgm = ValueInput<AudioClip>("bgm", null);  
        }
    }
}
