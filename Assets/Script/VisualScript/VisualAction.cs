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
using UnityEngine.Video;

namespace UnityEditor.Localization.UI{

    [UnitShortTitle("Play an Action")]
    [UnitCategory("AVG Game Nodes")]
    public class VisualAction : Unit{
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput lines;

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

        [DoNotSerialize]
        public ValueInput video;    
        
        [DoNotSerialize]                                             // Video in this scene | 剧中视频
        public ValueInput voices;  
        
        [DoNotSerialize]                                                 // Actor's voice | 台词语音
        public ValueInput background;     
        
        [DoNotSerialize]                                            // Background in current scene | 当前幕的背景/环境
        public ValueInput bgLayer;  
        
        [DoNotSerialize]                              // Which Layer does the Background on | 当前背景/环境所在的展示层
        public ValueInput foreground;       
        
        [DoNotSerialize]                                          // Foreground in current scene | 当前幕的前景/演员立绘
        public ValueInput fgLayer;                             // Which Layer does the Foreground on | 当前前景/演员立绘所在的展示层

        protected override void Definition(){
            inputTrigger = ControlInput("in", (flow) => { return outputTrigger; });
            outputTrigger = ControlOutput("out");

            actor = ValueInput<Actor>("当前演出角色", Actor.NULL);

            // TODO @yuzhou: actor emotion enums

            // TODO @yuzhou: could be all grouped into enums 
            ffamily = ValueInput<string>("ffamily", string.Empty);
            fcolor = ValueInput<string>("fcolor", "0x6079d2ff");
            fsize = ValueInput<float>("fsize", 15.0f);
            linespacing = ValueInput<float>("linespacing", 0.0f);
            align = ValueInput<Align>("align", Align.LT);
            fstyle = ValueInput<FontStyle>("fstyle", FontStyle.Normal);     

            // TODO @yuzhou: GUI button to select localized string entry
            // LocalizationPackageBridge bridge = new LocalizationPackageBridge();
            // bridge.ShowPicker();   

            lines = ValueInput<List<string>>("台词集合", new List<string>());
            voices = ValueInput<List<AudioClip>>("语音集合", new List<AudioClip>());  

            video = ValueInput<VideoClip>("动画效果", null); 
            background = ValueInput<Sprite>("背景图片", null); 
            bgLayer = ValueInput<Layer>("bgLayer", Layer.BACKGROUND1); 
            foreground = ValueInput<Sprite>("前景人物图片", null); 
            fgLayer = ValueInput<Layer>("fgLayer", Layer.FOREGROUND1); 
        }
    }
}
