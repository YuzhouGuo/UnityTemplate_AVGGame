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

    [Serializable]
    public enum VisualGameValueChange : int{
        IDLE = 0,
        POSITIVE = 5,
        VERY_POSTIVE = 10,
        NEGATIVE = -5,
        VERY_NEGATIVE = -10
    }

    [UnitShortTitle("Change Value")]
    [UnitCategory("AVG Game Nodes")]
    public class ChangeGameValue : Unit{
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput sorrow;

        [DoNotSerialize]
        public ValueInput angry;

        [DoNotSerialize]
        public ValueInput hate;

        protected override void Definition(){
            inputTrigger = ControlInput("in", (flow) => { return outputTrigger; });
            outputTrigger = ControlOutput("out");

            sorrow = ValueInput<VisualGameValueChange>("Sorrow", VisualGameValueChange.IDLE);
            angry = ValueInput<VisualGameValueChange>("Angry", VisualGameValueChange.IDLE);
            hate = ValueInput<VisualGameValueChange>("Hate", VisualGameValueChange.IDLE);
        }
    }
}
