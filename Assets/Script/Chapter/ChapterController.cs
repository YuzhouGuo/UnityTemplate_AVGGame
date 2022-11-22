﻿using Assets.Script.Model;
using Assets.Script.Model.Components;
using Assets.Script.Model.Datas;
using Assets.Script.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Script.Chapter
{
    public class ChapterController : MonoBehaviour
    {
        #region consts
        private static float standingOnRight = 4.0f;
        private static float standingOnLeft = -4.0f;
        #endregion

        #region Fields
        private static string rootPath;
        private static string savedDataPath;
        private static string savedDataFile;

        private GameObject titleContainer;
        private GameObject lineContainer;
        private GameObject popupWindow;
        private GameObject menuField;
        private GameObject selectorField;
        private Text actorName;
        private Text line;   // Script line showing text
        private Font font;

        #region Game objects
        private GameObject mainCanvas;

        #region Prefabs
        private Text historyTextPrefab;
        private Button saveDataModelPrefab;
        private Button selectorOptionPrefab;
        #endregion

        #region History gameobjects
        private GameObject historyField;
        private GameObject historyTexts;
        private Text currentActiveHistoryText;
        #endregion

        #region Saved data field
        private GameObject savedDataField;
        private GameObject savedDataPanel;
        private static List<List<Button>> savedDataButtons;
        private static List<SavedDataModel> savedDatas;
        private static int savdDataPageCount;
        // Last loaded savedDataPage
        private static int lastLoadedSavedDataPage;
        #endregion

        #region Setting field
        private GameObject settingField;
        #endregion

        #region Operation buttons
        private GameObject operationButtons;
        private GameObject autoPlayButton;
        private GameObject skipButton;
        private GameObject saveButton;
        private GameObject loadButton;
        private GameObject settingButton;
        #endregion

        #region Selector field
        private GameObject selector;
        #endregion
        #endregion

        // Game objects end

        // TODO: GalgameScript object may be a large data, so something must be done to optimize RAM.
        // Current displaying script
        private GalgameScript currentScript;
        private SpriteRenderer bgSpriteRenderer;
        private SpriteRenderer fgSpriteRenderer;
        private Transform fgSpriteTransform;
        private GameObject videoPlayer;
        private GameObject audioSource;
        private GameObject voiceAudioSource;

        private List<GalgameAction> galgameActions;
        private Dictionary<string, GalgameAction> galgameActionsMap;
        private GalgameAction currentGalgameAction;
        private PSelectorOption ActiveSelectorOption;
        private int currentSelectorOptionActionIndex;
        private int currentSelectorOptionLineIndex;
        // Current showing line's index in `currentScript`
        private int currentLineIndex;
        // Current showing line text's index of char
        private int currentLineCharIndex;
        // Is a line text is showing
        private bool isShowingLine;
        // Is now a option line text display time
        private bool isShowingSelectorOptionActionTime;
        // Is auto-reading mode is actived
        // private bool isAutoReadingModeOn;
        // Is skip mode is actived
        // private bool isSkipModeOn;
        // The previous skip time
        private DateTime preSkipTime;
        // Is menu is actived
        public bool isMenuActive;
        // Is saving saved data now
        private bool isSavingGameData;
        // Is loading saved data now
        private bool isLoadingSavedData;
        // Next line to display
        private string nextLine;

        private GameController gameController;

        private Coroutine currentTextShowCoroutine;
        private Coroutine currentLineSwitchCoroutine;
        private Coroutine hidehistoryFieldCoroutine;
        private static WaitForSeconds textShowWaitForSeconds;
        private static WaitForSeconds lineSwitchWaitForSeconds;
        private static WaitForSeconds skipModeLineSwitchWaitForSeconds;
        private VideoPlayer _video;
        private AudioSource _bgmAudio;
        private AudioSource _voiceAudio;
        private AudioClip bgmMusic;
        #endregion

        // Use this for initialization
        void Start()
        {
            mainCanvas = GameObject.Find("DisplayCanvas");
            gameController = Camera.main.GetComponent<GameController>();

            // Container
            titleContainer = mainCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = mainCanvas.transform.Find("LineContainer").gameObject;
            historyField = mainCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = mainCanvas.transform.Find("SavedDataField").gameObject;
            settingField = mainCanvas.transform.Find("SettingField").gameObject;
            popupWindow = mainCanvas.transform.Find("PopupWindow").gameObject;
            menuField = mainCanvas.transform.Find("MenuField").gameObject;
            selectorField = mainCanvas.transform.Find("SelectorField").gameObject;
            historyTexts = mainCanvas.transform.Find("HistoryField/Viewport/Content/HistoryTexts").gameObject;
            operationButtons = mainCanvas.transform.Find("LineContainer/OperationBtn").gameObject;

            // Prefabs
            historyTextPrefab = Resources.Load<GameObject>("Prefabs/HistoryText").GetComponent<Text>();
            saveDataModelPrefab = Resources.Load<GameObject>("Prefabs/SaveDataModel").GetComponent<Button>();
            selectorOptionPrefab = Resources.Load<GameObject>("Prefabs/SelectorOption").GetComponent<Button>();

            bgSpriteRenderer = GameObject.Find("Background/Bg").GetComponent<SpriteRenderer>();
            fgSpriteRenderer = GameObject.Find("Background/Fg").GetComponent<SpriteRenderer>();
            fgSpriteTransform = GameObject.Find("Background/Fg").GetComponent<Transform>();
            videoPlayer = GameObject.Find("VideoPlayer/VideoPlayer");
            audioSource = GameObject.Find("AudioSource/BgmAudioSource");
            voiceAudioSource = GameObject.Find("AudioSource/VoiceAudioSource");

            currentScript = Resources.Load<GalgameScript>("Chapter/Chapter-test");

            // Init line
            // Init currentScript, galgameActions, currentLineIndex
            // currentLineIndex = 0; // ?
            line = lineContainer.transform.Find("Line").GetComponent<Text>();
            actorName = lineContainer.transform.Find("ActorName").GetComponent<Text>();

            galgameActions = currentScript.GalgameActions;
            if (currentScript.Bg != null){
                bgSpriteRenderer.sprite = currentScript.Bg;
            }

            // Reload galgameAction as a hashtable
            galgameActionsMap = new Dictionary<string, GalgameAction>();
            foreach (GalgameAction a in currentScript.GalgameActions){
                galgameActionsMap.Add(a.Id, a);
            }

            textShowWaitForSeconds = new WaitForSeconds(SettingModel.textShowDuration);
            lineSwitchWaitForSeconds = new WaitForSeconds(SettingModel.lineSwitchDuration);
            skipModeLineSwitchWaitForSeconds = new WaitForSeconds(SettingModel.skipModeLineSwitchDuration);

            // opearation buttons
            autoPlayButton = operationButtons.transform.Find("AutoPlayBtn").gameObject;
            skipButton = operationButtons.transform.Find("SkipBtn").gameObject;
            saveButton = operationButtons.transform.Find("SaveBtn").gameObject;
            loadButton = operationButtons.transform.Find("LoadBtn").gameObject;
            settingButton = operationButtons.transform.Find("SettingBtn").gameObject;

            // media player
            _video = videoPlayer.GetComponent<VideoPlayer>();
            _bgmAudio = audioSource.GetComponent<AudioSource>();
            _voiceAudio = voiceAudioSource.GetComponent<AudioSource>();

            // selector field
            selector = selectorField.transform.Find("Selector").gameObject;

            // savedDatas = gameController.LoadSavedDatas();
            savedDataPanel = savedDataField.transform.Find("SavedDataPanel").gameObject;
            lastLoadedSavedDataPage = GameController.lastLoadedSavedDataPage;
            savdDataPageCount = GameController.savdDataPageCount;
            savedDatas = GameController.savedDatas;
            savedDataButtons = gameController.InitList<List<Button>>(savdDataPageCount);

            // InitSceneGameObject();
        }

        // Update is called once per frame
        void Update()
        {
            // mosue left button click
            if (Input.GetButtonDown("Fire1"))
            {
                if (titleContainer.activeSelf){
                    return;
                }

                GameObject hitUIObject = null;

                if (EventSystem.current.IsPointerOverGameObject()){
                    hitUIObject = GetMouseOverUIObject(mainCanvas);
                    Debug.Log("---- EventSystem.current.IsPointerOverGameObject ----" + hitUIObject);
                }

                if (null != hitUIObject && hitUIObject.tag.Trim() == "OperationButton"){
                    Debug.Log("hitUIObject.name: " + hitUIObject.name);
                    switch (hitUIObject.name){
                        case "CloseSaveData":
                            isSavingGameData = false;
                            isLoadingSavedData = false;
                            SetActiveSavedDataPanel(0);
                            break;
                    }
                }

                if (currentLineIndex <= galgameActions.Count && IsSwitchLineAllowed()){
                    if (!lineContainer.activeSelf){
                        gameController.ActiveGameObject(lineContainer);
                    }
                    if (null == hitUIObject || (null != hitUIObject && hitUIObject.tag.Trim() != "OperationButton")){
                        SwitchLine();
                    }
                }
            }

            if (SettingModel.isSkipModeOn && IsSwitchLineAllowed() && (null == preSkipTime || (DateTime.Now - preSkipTime).TotalSeconds > SettingModel.skipModeLineSwitchDuration)){
                if (!lineContainer.activeSelf){
                    gameController.ActiveGameObject(lineContainer);
                }
                SwitchLine();
                preSkipTime = DateTime.Now;
            }

            // mouse scroll up
            if (Input.mouseScrollDelta.y > 0 && !historyField.activeSelf && gameController.inGame){
                // TODO: Fix bug of adding double history text
                ShowhistoryField();
            }
        }

        #region Pubilc scene action
        /// <summary>
        /// Show history TextView
        /// </summary>
        public void ShowhistoryField(){
            historyField.SetActive(true);
            // If `currentTextShowCoroutine` is going
            if (isShowingLine){
                ShowLineImmediately();
            }
            SetManualMode(true);
        }

        /// <summary>
        /// Hide history TextView
        /// </summary>
        public void HideHistoryField(){
            hidehistoryFieldCoroutine = StartCoroutine(HideHistoryFieldTimeOut());
        }

        /// <summary>
        /// Set reading mode to manual
        /// </summary>
        /// <param name="manual"></param>
        public void SetManualMode(bool manual){
            SettingModel.isManualModeOn = manual;
            if (manual){
                SetAutoMode(false);
                SetSkipMode(false);
                StopAllCoroutines();
                // SettingModel.isAutoReadingModeOn = false;
                // SettingModel.isSkipModeOn = false;
            }
        }

        /// <summary>
        /// Auto Reading
        /// </summary>
        public void AutoReading(){
            SetAutoMode(!SettingModel.isAutoReadingModeOn);
        }

        /// <summary>
        /// Set auto mode
        /// </summary>
        /// <param name="auto"></param>
        public void SetAutoMode(bool auto){
            skipButton.GetComponent<Image>().color = Color.white;
            SettingModel.isAutoReadingModeOn = auto;
            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if (!auto){
                autoPlayButton.GetComponent<Image>().color = Color.white;
            }
            else{
                SettingModel.isManualModeOn = false;
                SettingModel.isSkipModeOn = false;
            }
            if (SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed() && !SettingModel.isSkipModeOn && !isShowingLine){
                autoPlayButton.GetComponent<Image>().color = Color.black;
                currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
            }
        }

        /// <summary>
        /// Enable/Disable skip mode
        /// </summary>
        public void ChangeSkipMode(){
            SetSkipMode(!SettingModel.isSkipModeOn);
        }

        /// <summary>
        /// Set skip mode
        /// </summary>
        /// <param name="skip"></param>
        public void SetSkipMode(bool skip){
            autoPlayButton.GetComponent<Image>().color = Color.white;
            SettingModel.isSkipModeOn = skip;
            if (!skip){
                skipButton.GetComponent<Image>().color = Color.white;
            }
            else{
                SettingModel.isManualModeOn = false;
                SettingModel.isAutoReadingModeOn = false;
            }
            if (SettingModel.isSkipModeOn && IsSwitchLineAllowed()){
                skipButton.GetComponent<Image>().color = Color.black;
                StopAllCoroutines();
                ShowLineImmediately();
                SwitchLine();
            }
        }

        /// <summary>
        /// Save game data
        /// </summary>
        public void SaveData(){
            isSavingGameData = true;
            SetSavedDataModelButtons(0, 12);
            // gameController.ShowCG();
            gameController.ActiveGameObject(savedDataField);
            SetManualMode(true);
            Debug.Log(string.Format("Save Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        /// <summary>
        /// Quick save
        /// </summary>
        public void QuickSave(){
            Debug.Log(string.Format("Quick Save Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        /// <summary>
        /// Load saved data
        /// </summary>
        public void LoadSavedData(){
            isLoadingSavedData = true;
            SetSavedDataModelButtons(0, 12);
            // gameController.ShowCG();
            gameController.ActiveGameObject(savedDataField);
            SetManualMode(true);
            Debug.Log(string.Format("Load Saved Game Data: CurrentScript={0}, CurrentLineIndex={1}", currentScript.ChapterName, currentLineIndex));
        }

        /// <summary>
        /// Switch page display to display saved data
        /// </summary>
        /// <param name="step"></param>
        public void SwitchSavedDataPage(int step){
            // If out of range, do nothing
            if (lastLoadedSavedDataPage + step < 0 || lastLoadedSavedDataPage + step >= savdDataPageCount){
                return;
            }
            // Hide previous display saved data page
            Transform target = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            if (null != target){
                target.gameObject.SetActive(false);
                // gameController.DeactiveGameObject(target.gameObject);
            }

            lastLoadedSavedDataPage += step;
            Transform nTarget = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            if (null != nTarget){
                nTarget.gameObject.SetActive(true);
                // gameController.ActiveGameObject(nTarget.gameObject);
            }
            // Set now display saved data page
            SetSavedDataModelButtons(lastLoadedSavedDataPage);
        }

        /// <summary>
        /// Open setting field
        /// </summary>
        public void OpenSetting(){
            // gameController.ShowCG();
            gameController.ActiveGameObject(settingField);
            SetManualMode(true);
        }

        /// <summary>
        /// Close setting field
        /// </summary>
        public void CloseSetting(){
            gameController.DeactiveGameObject(settingField);
            gameController.PersistSettingConfig();
            GameObject lineObject = lineContainer.transform.Find("Line").gameObject;
            if (SettingModel.showTextShadow){
                if (!lineObject.GetComponent<Shadow>()){
                    Shadow s = lineObject.AddComponent<Shadow>();
                    s.effectDistance = new Vector2(2, -1);
                }
            }
            else{
                if (lineObject.GetComponent<Shadow>()){
                    Destroy(lineObject.GetComponent<Shadow>());
                }
            }
        }

        /// <summary>
        /// Reset 
        /// </summary>
        public void ResetChapter(){
            currentLineIndex = 0;
            currentLineCharIndex = 0;
            currentGalgameAction = null;
        }

        #endregion

        #region Private methods

        private void InitSceneGameObject(){
            Vector2 sceneDeltaSize = mainCanvas.GetComponent<RectTransform>().sizeDelta;

            historyField.GetComponent<RectTransform>().sizeDelta = sceneDeltaSize;
            savedDataField.GetComponent<RectTransform>().sizeDelta = sceneDeltaSize;
        }

        /// <summary>
        /// Whether switch line operation is allow or not
        /// </summary>
        /// <returns></returns>
        private bool IsSwitchLineAllowed(){
            return gameController.inGame && !isMenuActive && !historyField.activeSelf && !savedDataField.activeSelf && !settingField.activeSelf && !popupWindow.activeSelf && !selectorField.activeSelf;
        }

        /// <summary>
        /// Hide history TextView with duration
        /// </summary>
        private IEnumerator HideHistoryFieldTimeOut(){
            if (historyField.activeSelf){
                historyField.SetActive(false);
                if (SettingModel.isAutoReadingModeOn){
                    yield return lineSwitchWaitForSeconds;
                    SwitchLine();
                }
            }
        }

        /// <summary>
        /// Hide history TextView with duration
        /// </summary>
        private IEnumerator HideSelectorFieldTimeOut(){
            if (selectorField.activeSelf){
                selectorField.SetActive(false);
                if (SettingModel.isAutoReadingModeOn){
                    yield return lineSwitchWaitForSeconds;
                    SwitchLine();
                }
            }
        }

        /// <summary>
        /// Switch action
        /// </summary>
        private void SwitchAction(string actionId){
            if (!string.IsNullOrEmpty(actionId)){
                // set current galgame action
                currentGalgameAction = galgameActionsMap[actionId];
                // reset line index
                currentLineIndex = 0;
                // build acrrent game action
                BuildAAction(currentGalgameAction);

                // If this action do not contains Lines and other display components
                if (currentGalgameAction.Lines.Count == 0
                    && string.IsNullOrEmpty(currentGalgameAction.Selector.Id)
                    && null == currentGalgameAction.Bgm
                    && null == currentGalgameAction.Background
                    && null == currentGalgameAction.Video
                    ){
                    // Continue immediately
                    SwitchAction(currentGalgameAction.NextActionId);
                }
            }
            else{
                // this chapter is end
                if (SettingModel.isSkipModeOn){
                    SetSkipMode(false);
                }
                if (SettingModel.isAutoReadingModeOn){
                    SetAutoMode(false);
                }
                // TODO: maybe consider loading another chapter?
                currentScript = Resources.Load<GalgameScript>("Chapter/Chapter-test");
                currentGalgameAction = null;
                actorName.text = string.Empty;
                currentLineIndex = 0;
                currentLineCharIndex = -1;
                line.text = "『つづく...』";
                return;
            }
        }

        /// <summary>
        /// Show new line controller
        /// </summary>
        private void SwitchLine(){
            if (null == currentGalgameAction){
                SwitchAction(currentScript.StartActionId);
            }
            line.text = string.Empty; // clear previous line
            if (isShowingLine){
                Debug.Log(DateTime.Now.ToString() + "准备跳过");
                if (null != currentTextShowCoroutine){
                    StopCoroutine(currentTextShowCoroutine);
                    ShowLineImmediately(nextLine);
                    AddHistoryText(nextLine); // Add line to history text list
                    isShowingLine = false;
                    Debug.Log(DateTime.Now.ToString() + "已跳过");
                    return;
                }
            }

            currentLineCharIndex = -1; // read from index: -1

            if (isShowingSelectorOptionActionTime){
                if (currentSelectorOptionLineIndex >= ActiveSelectorOption.Action.Lines.Count){
                    isShowingSelectorOptionActionTime = false;
                }
                else{
                    GalgameScriptLine sLine = ActiveSelectorOption.Action.Lines[currentSelectorOptionLineIndex];
                    nextLine = sLine.text.Replace("\\n", "\n");
                    actorName.text = sLine.actor.ToString();
                    if (SettingModel.isSkipModeOn){
                        ShowLineImmediately();
                    }
                    else{
                        currentTextShowCoroutine = StartCoroutine(ShowLineTimeOut(nextLine));
                    }
                    currentSelectorOptionLineIndex++;
                }
            }
            else{
                if (IsSwitchLineAllowed() && currentLineIndex == currentGalgameAction.Lines.Count){
                    // reach end point of current action, switch
                    SwitchAction(currentGalgameAction.NextActionId);
                    // Debug.Log("Switch action: " + currentGalgameAction.Id + " to " + currentGalgameAction.NextActionId);
                    SwitchLine();
                    return;
                }

                if (currentLineIndex < currentGalgameAction.Lines.Count){
                    nextLine = currentGalgameAction.Lines[currentLineIndex].text.Replace("\\n", "\n");
                    if (SettingModel.isSkipModeOn){
                        ShowLineImmediately();
                    }
                    else{
                        currentTextShowCoroutine = StartCoroutine(ShowLineTimeOut(nextLine));
                    }

                    // Move index to next
                    currentLineIndex++;
                }
            }
        }

        /// <summary>
        /// Show new line controller
        /// </summary>
        private void SwitchLine_Old(){
            line.text = string.Empty; // clear previous line
            if (isShowingLine){
                Debug.Log(DateTime.Now.ToString() + "准备跳过");
                if (null != currentTextShowCoroutine){
                    StopCoroutine(currentTextShowCoroutine);
                    ShowLineImmediately(nextLine);
                    AddHistoryText(nextLine); // Add line to history text list
                    isShowingLine = false;
                    Debug.Log(DateTime.Now.ToString() + "已跳过");
                    return;
                }
            }

            currentLineCharIndex = -1; // read from index: -1

            if (isShowingSelectorOptionActionTime){
                GalgamePlainAction action = ActiveSelectorOption.Action;

                if (++currentSelectorOptionLineIndex >= ActiveSelectorOption.Action.Lines.Count){
                    isShowingSelectorOptionActionTime = false;
                }
            }
            else{
                if (currentLineIndex == galgameActions.Count){
                    // this chapter is end
                    if (SettingModel.isSkipModeOn){
                        SetSkipMode(false);
                    }
                    if (SettingModel.isAutoReadingModeOn){
                        SetAutoMode(false);
                    }
                    // TODO: maybe consider loading another chapter?
                    currentScript = Resources.Load<GalgameScript>("Chapter/Chapter-test");
                    actorName.text = string.Empty;
                    currentLineIndex = 0;
                    currentLineCharIndex = -1;
                    line.text = "『つづく...』";
                    return;
                }

                Debug.Log("galgameActions'Size: " + galgameActions.Count + " currentLineIndex: " + currentLineIndex);
                currentGalgameAction = galgameActions[currentLineIndex];

                BuildAAction(currentGalgameAction);

                // Move index to next
                currentLineIndex++;

                // If is a empty line with only bgm/bg
                if (string.Empty.Equals(currentGalgameAction.Line.text.Trim()) && (null != currentGalgameAction.Bgm || null != currentGalgameAction.Background)){
                    // Continue immediately
                    SwitchLine();
                }
            }

        }

        /// <summary>
        /// To build a action
        /// </summary>
        /// <param name="action"></param>
        private void BuildAAction(GalgamePlainAction action){
            if (action.Bgm != null){
                // bgm
                _bgmAudio.clip = action.Bgm;
                _bgmAudio.Play();
            }
            if (action.Voice != null){
                // voice
                _voiceAudio.clip = action.Voice;
                _voiceAudio.Play();
            }
            if (action.Actor != Actor.NULL){
                // actor's name
                // actorName.text = action.Actor.ToString();
            }
            else{
                actorName.text = string.Empty;
            }
            if (action.Background != null){
                // current background
                bgSpriteRenderer.sprite = action.Background;
            }
            if (action.Foreground != null){
                // current foreground
                fgSpriteRenderer.sprite = action.Foreground;
                Vector3 p = fgSpriteTransform.position;
                if(action.Actor == Actor.女の子){
                    fgSpriteRenderer.flipX = true;
                    p.x = standingOnRight;
                }
                else{
                    fgSpriteRenderer.flipX = false;
                    p.x = standingOnLeft;
                }
                fgSpriteTransform.position = p;
            }
            // text-align
            line.alignment = EnumMap.AlignToTextAnchor(action.Line.align);

            // font
            if (font != null){
                line.font = font;
            }
            // font-style
            if (action.Line.fstyle == FontStyle.Normal){
                line.fontStyle = DefaultScriptProperty.fstyle;
            }
            else{
                line.fontStyle = action.Line.fstyle;
            }
            // font-size
            if (action.Line.fsize != 0){
                line.fontSize = Mathf.RoundToInt(action.Line.fsize);
            }
            else if (DefaultScriptProperty.fsize != 0){
                line.fontSize = Mathf.RoundToInt(DefaultScriptProperty.fsize);
            }
            // line-spacing
            if (action.Line.linespacing != 0){
                line.lineSpacing = action.Line.linespacing;
            }
            else if (DefaultScriptProperty.linespacing != 0){
                line.lineSpacing = DefaultScriptProperty.linespacing;
            }
            // font-color
            if (!string.IsNullOrEmpty(action.Line.fcolor)){
                line.color = ColorUtil.HexToUnityColor(uint.Parse(action.Line.fcolor.Replace("0x", "").Substring(0, 6), System.Globalization.NumberStyles.HexNumber));
            }
            else if (!string.IsNullOrEmpty(DefaultScriptProperty.fcolor)){
                line.color = ColorUtil.HexToUnityColor(uint.Parse(DefaultScriptProperty.fcolor, System.Globalization.NumberStyles.HexNumber));
            }

            if (action.GetType().Equals(typeof(GalgameAction))){

                // If there a selector component
                if (!string.IsNullOrEmpty(((GalgameAction)action).Selector.Id)){
                    BuildSelector(((GalgameAction)action).Selector);
                }

                // If there a adjuster component
                if (!string.IsNullOrEmpty(((GalgameAction)action).GameValuesAdjuster.Id)){
                    ExecuteAdjuster(((GalgameAction)action).GameValuesAdjuster);
                }

                // If there a events component
                if (!string.IsNullOrEmpty(((GalgameAction)action).Events.Id)){
                    TriggerEvents(((GalgameAction)action).Events);
                }

                // If there a judge component
                if (!string.IsNullOrEmpty(((GalgameAction)action).Judge.Id)){
                    ExecuteJudge(((GalgameAction)action).Judge);
                }
            }
        }

        /// <summary>
        /// Build current scene's Selector component
        /// </summary>
        private void BuildSelector(PSelector selector){
            if ((null == selector.Options || selector.Options.Count == 0) && (null == selector.Texts || selector.Texts.Count == 0)
                && (null == selector.Bgms || selector.Bgms.Count == 0) && (null == selector.Bgs || selector.Bgs.Count == 0)){
                    return;
                }

            // Selector.Options has higher priority than Options set on properties(Texts,Bgs,Bgms) of Selector
            if (selector.Options == null || selector.Options.Count == 0){
                selector.Options = BuildSelectorOptions(selector);
                // TODO: Consider update chapter
            }

            // Re-build Selector Container
            if (this.selector != null){
                Destroy(this.selector);
            }
            this.selector = new GameObject("Selector");
            Image image = this.selector.AddComponent<Image>();
            image.color = new Color(1.0f, 1.0f, 1.0f, 0f);

            Vector2 opSize = Vector2.zero;
            Vector2 opSpacing = new Vector2(10.0f, 10.0f);
            int opNumber = selector.Options.Count;

            Grid selectorGrid = this.selector.AddComponent<Grid>();
            GridLayoutGroup selectorGroup = this.selector.AddComponent<GridLayoutGroup>();
            selectorGroup.spacing = opSpacing;

            // Set size of selector panel
            switch (selector.Type){
                case Model.Enum.SelectorType.HORIZONTAL:
                    opSize = new Vector2(200.0f, 300.0f);
                    selectorGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    selectorGroup.cellSize = opSize;
                    this.selector.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(opNumber * opSize.x + (opNumber - 1) * opSize.x, opSize.y);
                    break;
                case Model.Enum.SelectorType.VERTICLE:
                    opSize = new Vector2(400.0f, 80.0f);
                    selectorGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                    selectorGroup.cellSize = opSize;
                    this.selector.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(opSize.x, opNumber * opSize.y + (opNumber - 1) * opSpacing.y);
                    break;
            }
            selectorGroup.constraintCount = opNumber;

            foreach (PSelectorOption option in selector.Options){
                Button newEmptyOption = Instantiate(selectorOptionPrefab);
                Button.ButtonClickedEvent optionClickEvent = new Button.ButtonClickedEvent();
                optionClickEvent.AddListener(() =>{
                    selector.IsSelected = true;
                    selector.SelectedItem = selector.Options.IndexOf(option);
                    if (option.Action != null){
                        this.ActiveSelectorOption = option;
                        this.isShowingSelectorOptionActionTime = true;                                  // Hide selector panel
                        this.currentSelectorOptionLineIndex = 0;

                        nextLine = option.Action.Lines.First().text.Replace("\\n", "\n");
                        BuildAAction(option.Action);
                    }
                    gameController.UpdateGlobalGameValues(option.DeltaGameValues);                  // Update global values
                    StartCoroutine(HideSelectorFieldTimeOut());
                    SwitchLine();
                    Debug.Log("Global Values: \n" + GlobalGameData.GameValues.ToJSONString());
                });
                newEmptyOption.onClick = optionClickEvent;
                if (option.Bg != null){
                    newEmptyOption.GetComponent<RawImage>().texture = option.Bg.texture;
                }
                if (option.Bgm != null){
                    newEmptyOption.GetComponent<AudioSource>().clip = option.Bgm;
                }
                Text ot = newEmptyOption.transform.Find("OptionText").GetComponent<Text>();
                ot.text = option.Text.text;
                ot.alignment = EnumMap.AlignToTextAnchor(option.Text.align);

                // font-style
                ot.fontStyle = (option.Text.fstyle == FontStyle.Normal) ? DefaultScriptProperty.fstyle : option.Text.fstyle;

                // font-size
                if (option.Text.fsize != 0){
                    ot.fontSize = Mathf.RoundToInt(option.Text.fsize);
                }
                else if (DefaultScriptProperty.fsize != 0){
                    ot.fontSize = Mathf.RoundToInt(DefaultScriptProperty.fsize);
                }

                // line-spacing
                if (option.Text.linespacing != 0){
                    ot.lineSpacing = option.Text.linespacing;
                }
                else if (DefaultScriptProperty.linespacing != 0){
                    ot.lineSpacing = DefaultScriptProperty.linespacing;
                }

                // font-color
                if (!string.IsNullOrEmpty(option.Text.fcolor)){
                    ot.color = ColorUtil.HexToUnityColor(uint.Parse(option.Text.fcolor.Replace("0x", "").Substring(0, 6), System.Globalization.NumberStyles.HexNumber));
                }
                else if (!string.IsNullOrEmpty(DefaultScriptProperty.fcolor)){
                    ot.color = ColorUtil.HexToUnityColor(uint.Parse(DefaultScriptProperty.fcolor, System.Globalization.NumberStyles.HexNumber));
                }
                newEmptyOption.transform.SetParent(this.selector.transform);
            }
            // this.selector.GetComponent<RectTransform>().position = Vector3x.zero;
            this.selector.transform.SetParent(selectorField.transform);
            this.selector.transform.localScale = Vector3.one;
            selectorField.SetActive(true);
        }

        /// <summary>
        /// Build Selector's Options
        /// </summary>
        private List<PSelectorOption> BuildSelectorOptions(PSelector selector){
            List<PSelectorOption> options = new List<PSelectorOption>();
            int optionNumber = new int[] { selector.Bgms.Count, selector.Bgs.Count, selector.Texts.Count }.Max();
            for (int n = 0; n < optionNumber; n++){
                PSelectorOption o = new PSelectorOption();
                o.Text = n > selector.Texts.Count - 1 ? default(PText) : selector.Texts[n];
                o.Bg = n > selector.Bgs.Count - 1 ? default(Sprite) : selector.Bgs[n];
                o.Bgm = n > selector.Bgms.Count - 1 ? default(AudioClip) : selector.Bgms[n];
                options.Add(o);
            }
            return options;
        }

        /// <summary>
        /// Execute this adjuster to change global values
        /// </summary>
        /// <param name="adjuster"></param>
        private void ExecuteAdjuster(PGameValuesAdjuster adjuster){
            gameController.UpdateGlobalGameValues(adjuster.DeltaGameValues);                  // Update global values
        }

        /// <summary>
        /// Trigger this adjuster to change global values
        /// </summary>
        private void TriggerEvents(PEvents events){
            foreach (PEventItem eventItem in events.Events){
                int evtId = Convert.ToInt32(eventItem.EvtId);
                string evtDesc = EventCollection.Instance.Get(evtId);
                if (!string.IsNullOrEmpty(evtDesc)){
                    // TODO: Maybe display event description here.

                    if (null != eventItem.DeltaGameValues){
                        gameController.UpdateGlobalGameValues(eventItem.DeltaGameValues);                  // Update global values
                    }
                }
            }
        }

        /// <summary>
        /// Execute this judge component
        /// </summary>
        /// <param name="judge"></param>
        public void ExecuteJudge(PJudge judge){
            // TODO: deal with MeetGameValues
            if (judge.MeetGameValues != null && judge.MeetGameValues.Count > 0){
                foreach (GameValues gvs in judge.MeetGameValues){
                    if (GlobalGameData.GameValues.Equals(gvs)){
                        List<PEventItem> events = judge.Events;
                        foreach (PEventItem eventItem in events){
                            int evtId = Convert.ToInt32(eventItem.EvtId);
                            string evtDesc = EventCollection.Instance.Get(evtId);
                            if (!string.IsNullOrEmpty(evtDesc)){
                                // TODO: Maybe display event description here.

                                if (eventItem.DeltaGameValues != null){
                                    gameController.UpdateGlobalGameValues(eventItem.DeltaGameValues);                  // Update global values
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(judge.NextActionId)){
                            SwitchAction(judge.NextActionId);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set current active history text when a history text clicked
        /// </summary>
        private void SetCurrentActiveHistoryText(Text nextActiveHistoryText){
            if (currentActiveHistoryText != null){
                currentActiveHistoryText.color = Color.white;
            }
            nextActiveHistoryText.color = Color.blue;
            currentActiveHistoryText = nextActiveHistoryText;
        }

        /// <summary>
        /// Show line text with duration <see cref="textShowDuration"/>
        /// </summary>
        /// <param name="newLine">A new full-text line</param>
        /// <param name="autoSwitchLine">Auto reading mode</param>
        /// <returns></returns>
        private IEnumerator ShowLineTimeOut(string newLine, bool autoSwitchLine = false){
            isShowingLine = true;
            if (string.IsNullOrEmpty(newLine)){
                if (SettingModel.isAutoReadingModeOn){
                    yield return lineSwitchWaitForSeconds;
                    SwitchLine();
                }
            }
            foreach (char lineChar in newLine){
                currentLineCharIndex++;
                line.text += lineChar;
                if (currentLineCharIndex == newLine.Length - 1){
                    isShowingLine = false;
                    Debug.Log(DateTime.Now.ToString() + "清除ShowingLine状态");
                    AddHistoryText(newLine); // Add line to history text list
                    Debug.Log("currentLineCharIndex: " + currentLineCharIndex);
                    if (SettingModel.isAutoReadingModeOn){
                        yield return lineSwitchWaitForSeconds;
                        SwitchLine();
                    }
                }
                yield return textShowWaitForSeconds;
            }
        }

        /// <summary>
        /// Show line text immediately
        /// </summary>
        private void ShowLineImmediately(){
            if (isShowingLine){
                Debug.Log(DateTime.Now.ToString() + "准备跳过");
                if (currentTextShowCoroutine != null){
                    StopCoroutine(currentTextShowCoroutine);
                    isShowingLine = false;
                }
            }
            AddHistoryText(nextLine); // Add line to history text list
            ShowLineImmediately(nextLine);
            Debug.Log(DateTime.Now.ToString() + "已跳过");
            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if (!SettingModel.isSkipModeOn && SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed()){
                currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
            }
        }

        /// <summary>
        /// Show line text immediately
        /// </summary>
        /// <param name="newLine">A new full-text line</param>
        private void ShowLineImmediately(string newLine){
            line.text = newLine;

            // If SettingModel.isAutoReadingModeOn == true, call SwitchLine()
            if (!SettingModel.isSkipModeOn && SettingModel.isAutoReadingModeOn && IsSwitchLineAllowed()){
                Debug.Log("1: historyField.activeSelf=" + historyField.activeSelf);
                currentLineSwitchCoroutine = StartCoroutine(SwitchLineTimeout());
            }
        }

        /// <summary>
        /// Show line text with duration <see cref="waitForSeconds" />
        /// </summary>
        /// <param name="waitForSeconds"></param>
        private IEnumerator SwitchLineTimeout(WaitForSeconds waitForSeconds){
            yield return waitForSeconds;
            if (!isShowingLine && IsSwitchLineAllowed()){
                SwitchLine();
            }
        }

        /// <summary>
        /// Show line text with duration <see cref="lineSwitchDuration"/>
        /// </summary>
        private IEnumerator SwitchLineTimeout(){
            yield return lineSwitchWaitForSeconds;
            if (!SettingModel.isSkipModeOn && !isShowingLine && IsSwitchLineAllowed()){
                SwitchLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex">The index of page, total number of page will be `savdDataPageCount`.</param>
        /// <param name="number">The page size, default: 12.</param>
        /// <returns></returns>
        private List<Button> SetSavedDataModelButtons(int pageIndex, int number = 12){
            int savedDataPageNumbers = savedDataPanel.transform.childCount;
            List<Button> pageButtons;

            if (savedDataPageNumbers >= pageIndex + 1){
                // Saved data buttons are already initialized, load from cache
                pageButtons = savedDataButtons[pageIndex];
                return pageButtons;
            }
            else{
                // No saved data buttons in this page yet, initial they
                pageButtons = InitSavedDataButton(pageIndex, number);
                savedDataButtons[lastLoadedSavedDataPage] = pageButtons;
                return pageButtons;
            }
        }

        /// <summary>
        /// SavedData show page controller, set page at <paramref name="activeIndex"/> to be actived
        /// </summary>
        /// <param name="activeIndex">active page</param>
        private void SetActiveSavedDataPanel(int activeIndex){
            Transform pre = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            pre.gameObject.SetActive(false);
            Transform now = savedDataPanel.transform.Find(string.Format("SavedDataPage_{0}", activeIndex));
            now.gameObject.SetActive(true);
            lastLoadedSavedDataPage = activeIndex;
        }

        /// <summary>
        /// Init saved data button in SavedDataPage
        /// </summary>
        /// <param name="pageIndex">Index of SavedDataPage, 0 as start</param>
        /// <param name="number">Button number per page, default: 12</param>
        /// <returns></returns>
        internal List<Button> InitSavedDataButton(int pageIndex, int number = 12){
            List<Button> currentSaveDataList = new List<Button>();
            GameObject gameObject = new GameObject(string.Format("SavedDataPage_{0}", lastLoadedSavedDataPage));
            Grid savedDataGrid = gameObject.AddComponent<Grid>();
            GridLayoutGroup savedDataGroup = gameObject.AddComponent<GridLayoutGroup>();
            savedDataGroup.cellSize = new Vector2(200.0f, 120.0f);
            savedDataGroup.spacing = Vector2.one;

            for (int i = 0; i < number; i++){
                Button newEmptySaveDataModel = Instantiate(saveDataModelPrefab);
                Button.ButtonClickedEvent saveDataClickEvent = new Button.ButtonClickedEvent();
                int savedDataIndex = pageIndex * number + i;
                saveDataClickEvent.AddListener(delegate (){
                    // Click Callback
                    if (isSavingGameData){
                        // Save game data

                        // TODO: Test it
                        // Save current status of game.
                        if (null == savedDatas[savedDataIndex]){
                            savedDatas[savedDataIndex] = new SavedDataModel(){
                                savedDataIndex = savedDataIndex,
                                savedTime = DateTime.Now,
                                galgameActionId = currentGalgameAction.Id,
                                galgameActionLineIndex = currentLineIndex - 1,
                                gameValues = GlobalGameData.GameValues
                            };
                        }
                        else{
                            savedDatas[savedDataIndex].savedDataIndex = savedDataIndex;
                            savedDatas[savedDataIndex].savedTime = DateTime.Now;
                            savedDatas[savedDataIndex].galgameActionId = currentGalgameAction.Id;
                            savedDatas[savedDataIndex].galgameActionLineIndex = currentLineIndex - 1;
                            savedDatas[savedDataIndex].gameValues = GlobalGameData.GameValues;
                        }
                        // TODO: Considering doing this when application exit to avoid unnecessary IO operations?
                        // Persist saved data
                        gameController.PersistSavedDatas();
                        // Renew saved data display field
                        RenewSavedDataField(newEmptySaveDataModel, savedDatas[savedDataIndex]);
                    }
                    if (isLoadingSavedData){
                        // Load saved game data
                        SavedDataModel theSavedData = gameController.LoadSavedData(savedDataIndex);
                        if (theSavedData ==  null){
                            return;
                        }

                        // TODO: Test it
                        // Refresh scene via the saved data.
                        gameController.inGame = true;
                        gameController.ShowCG();
                        isLoadingSavedData = false;
                        SetCurrentGalgameAction(theSavedData);
                    }
                });
                newEmptySaveDataModel.onClick = saveDataClickEvent;
                newEmptySaveDataModel.transform.SetParent(gameObject.transform);
                newEmptySaveDataModel.name = string.Format("SaveData_{0}", i + 1);
                newEmptySaveDataModel.GetComponent<RectTransform>().localScale = Vector3.one;
                // Set display data
                if (savedDatas[savedDataIndex] != null){
                    RenewSavedDataField(newEmptySaveDataModel, savedDatas[savedDataIndex]);
                }
                currentSaveDataList.Add(newEmptySaveDataModel);
            }
            // Append saved data list to `savedDataPanel`
            gameObject.transform.SetParent(savedDataPanel.transform);
            gameObject.transform.position = savedDataPanel.transform.position;
            gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            gameObject.GetComponent<RectTransform>().sizeDelta = savedDataPanel.GetComponent<RectTransform>().sizeDelta;
            return currentSaveDataList;
        }

        internal void RenewSavedDataField(Button b, SavedDataModel sdm){
            Text t = b.gameObject.transform.GetChild(0).GetComponent<Text>();
            // TODO: Make decision of what should be renew. Background, display text for example.
            t.text = string.Format("Saved Date: {0}\nSaved Time: {1}", sdm.savedTime.ToString("yyyy/MM/dd"), sdm.savedTime.ToString("hh:mm:ss"));
        }

        /// <summary>
        /// Set current galgame action.
        /// TODO: The SavedDataModel class will be modeified. The method must to be adjusted.
        /// </summary>
        /// <param name="theSavedData"></param>
        internal void SetCurrentGalgameAction(SavedDataModel theSavedData){
            //SwitchAction(theSavedData.galgameActionId);
            currentLineIndex = theSavedData.galgameActionLineIndex;
            currentGalgameAction = galgameActionsMap[theSavedData.galgameActionId];
            bgSpriteRenderer.sprite = currentGalgameAction.Background;
            fgSpriteRenderer.sprite = currentGalgameAction.Foreground;
            nextLine = currentGalgameAction.Lines[currentLineIndex].text.Replace("\\n", "\n");
            GlobalGameData.GameValues = theSavedData.gameValues;
            line.text = string.Empty;
        }

        /// <summary>
        /// Add line to history
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool AddHistoryText(string text){
            Text newHistoryText = Instantiate(historyTextPrefab);
            newHistoryText.text = text;
            Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
            buttonClickedEvent.AddListener(delegate (){
                SetCurrentActiveHistoryText(newHistoryText);
            });
            newHistoryText.transform.GetComponent<Button>().onClick = buttonClickedEvent;
            newHistoryText.transform.SetParent(historyTexts.transform);
            newHistoryText.GetComponent<RectTransform>().localScale = Vector3.one;
            SetCurrentActiveHistoryText(newHistoryText);
            return true;
        }

        /// <summary>
        /// Get the UI object of current mouse pointer hover on.
        /// </summary>
        /// <param name="canvas">The specific canvas</param>
        /// <returns></returns>
        private GameObject GetMouseOverUIObject(GameObject canvas){
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            if (results.Count != 0){
                return results[0].gameObject;
            }

            return null;
        }

        /// <summary>
        /// Wait for seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private IEnumerator WaitForSeconds(float seconds){
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Wait for seconds
        /// </summary>
        /// <param name="waitForSeconds"></param>
        /// <returns></returns>
        private IEnumerator WaitForSeconds(WaitForSeconds waitForSeconds){
            yield return waitForSeconds;
        }
        #endregion
    }
}