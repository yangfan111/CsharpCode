using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Assets.UiFramework.Libs;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonCrossViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonCrossView : UIView
        {
            public GameObject ShowGameObjectActiveSelf;
            [HideInInspector]
            public bool oriShowGameObjectActiveSelf;
            public GameObject HitGameObjectActiveSelf;
            [HideInInspector]
            public bool oriHitGameObjectActiveSelf;
            public RectTransform LuRectTransformLocalPosition;
            [HideInInspector]
            public Vector3 oriLuRectTransformLocalPosition;
            public RectTransform LdRectTransformLocalPosition;
            [HideInInspector]
            public Vector3 oriLdRectTransformLocalPosition;
            public RectTransform RuRectTransformLocalPosition;
            [HideInInspector]
            public Vector3 oriRuRectTransformLocalPosition;
            public RectTransform RdRectTransformLocalPosition;
            [HideInInspector]
            public Vector3 oriRdRectTransformLocalPosition;
            public GameObject AwpGameObjectActiveSelf;
            [HideInInspector]
            public bool oriAwpGameObjectActiveSelf;
            public GameObject TypeGameObjectActiveSelf1;
            public GameObject CenterGameObjectActiveSelf1;
            public Image CenterImageColor1;
            public GameObject CycleGameObjectActiveSelf1;
            public Image CycleImageColor1;
            public RectTransform CycleRectTransformLocalScale1;
            public GameObject UpGameObjectActiveSelf1;
            public Image UpImageColor1;
            public RectTransform UpRectTransformLocalPosition1;
            public GameObject DownGameObjectActiveSelf1;
            public Image DownImageColor1;
            public RectTransform DownRectTransformLocalPosition1;
            public GameObject LeftGameObjectActiveSelf1;
            public Image LeftImageColor1;
            public RectTransform LeftRectTransformLocalPosition1;
            public GameObject RightGameObjectActiveSelf1;
            public Image RightImageColor1;
            public RectTransform RightRectTransformLocalPosition1;
            public GameObject TypeGameObjectActiveSelf2;
            public GameObject CenterGameObjectActiveSelf2;
            public Image CenterImageColor2;
            public GameObject CycleGameObjectActiveSelf2;
            public Image CycleImageColor2;
            public RectTransform CycleRectTransformLocalScale2;
            public GameObject UpGameObjectActiveSelf2;
            public Image UpImageColor2;
            public RectTransform UpRectTransformLocalPosition2;
            public GameObject DownGameObjectActiveSelf2;
            public Image DownImageColor2;
            public RectTransform DownRectTransformLocalPosition2;
            public GameObject LeftGameObjectActiveSelf2;
            public Image LeftImageColor2;
            public RectTransform LeftRectTransformLocalPosition2;
            public GameObject RightGameObjectActiveSelf2;
            public Image RightImageColor2;
            public RectTransform RightRectTransformLocalPosition2;
            public GameObject TypeGameObjectActiveSelf3;
            public GameObject CenterGameObjectActiveSelf3;
            public Image CenterImageColor3;
            public GameObject CycleGameObjectActiveSelf3;
            public Image CycleImageColor3;
            public RectTransform CycleRectTransformLocalScale3;
            public GameObject UpGameObjectActiveSelf3;
            public Image UpImageColor3;
            public RectTransform UpRectTransformLocalPosition3;
            public GameObject DownGameObjectActiveSelf3;
            public Image DownImageColor3;
            public RectTransform DownRectTransformLocalPosition3;
            public GameObject LeftGameObjectActiveSelf3;
            public Image LeftImageColor3;
            public RectTransform LeftRectTransformLocalPosition3;
            public GameObject RightGameObjectActiveSelf3;
            public Image RightImageColor3;
            public RectTransform RightRectTransformLocalPosition3;
            public GameObject TypeGameObjectActiveSelf4;
            public GameObject CenterGameObjectActiveSelf4;
            public Image CenterImageColor4;
            public GameObject CycleGameObjectActiveSelf4;
            public Image CycleImageColor4;
            public RectTransform CycleRectTransformLocalScale4;
            public GameObject UpGameObjectActiveSelf4;
            public Image UpImageColor4;
            public RectTransform UpRectTransformLocalPosition4;
            public GameObject DownGameObjectActiveSelf4;
            public Image DownImageColor4;
            public RectTransform DownRectTransformLocalPosition4;
            public GameObject LeftGameObjectActiveSelf4;
            public Image LeftImageColor4;
            public RectTransform LeftRectTransformLocalPosition4;
            public GameObject RightGameObjectActiveSelf4;
            public Image RightImageColor4;
            public RectTransform RightRectTransformLocalPosition4;
            public GameObject TypeGameObjectActiveSelf5;
            public GameObject CenterGameObjectActiveSelf5;
            public Image CenterImageColor5;
            public GameObject CycleGameObjectActiveSelf5;
            public Image CycleImageColor5;
            public RectTransform CycleRectTransformLocalScale5;
            public GameObject UpGameObjectActiveSelf5;
            public Image UpImageColor5;
            public RectTransform UpRectTransformLocalPosition5;
            public GameObject DownGameObjectActiveSelf5;
            public Image DownImageColor5;
            public RectTransform DownRectTransformLocalPosition5;
            public GameObject LeftGameObjectActiveSelf5;
            public Image LeftImageColor5;
            public RectTransform LeftRectTransformLocalPosition5;
            public GameObject RightGameObjectActiveSelf5;
            public Image RightImageColor5;
            public RectTransform RightRectTransformLocalPosition5;
            public GameObject TypeGameObjectActiveSelf6;
            public GameObject CenterGameObjectActiveSelf6;
            public Image CenterImageColor6;
            public GameObject CycleGameObjectActiveSelf6;
            public Image CycleImageColor6;
            public RectTransform CycleRectTransformLocalScale6;
            public GameObject UpGameObjectActiveSelf6;
            public Image UpImageColor6;
            public RectTransform UpRectTransformLocalPosition6;
            public GameObject DownGameObjectActiveSelf6;
            public Image DownImageColor6;
            public RectTransform DownRectTransformLocalPosition6;
            public GameObject LeftGameObjectActiveSelf6;
            public Image LeftImageColor6;
            public RectTransform LeftRectTransformLocalPosition6;
            public GameObject RightGameObjectActiveSelf6;
            public Image RightImageColor6;
            public RectTransform RightRectTransformLocalPosition6;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            ShowGameObjectActiveSelf = v.gameObject;
                            break;
                        case "Hit":
                            HitGameObjectActiveSelf = v.gameObject;
                            break;
                        case "Awp":
                            AwpGameObjectActiveSelf = v.gameObject;
                            break;
                        case "Type1":
                            TypeGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Center1":
                            CenterGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Cycle1":
                            CycleGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Up1":
                            UpGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Down1":
                            DownGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Left1":
                            LeftGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Right1":
                            RightGameObjectActiveSelf1 = v.gameObject;
                            break;
                        case "Type2":
                            TypeGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Center2":
                            CenterGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Cycle2":
                            CycleGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Up2":
                            UpGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Down2":
                            DownGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Left2":
                            LeftGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Right2":
                            RightGameObjectActiveSelf2 = v.gameObject;
                            break;
                        case "Type3":
                            TypeGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Center3":
                            CenterGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Cycle3":
                            CycleGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Up3":
                            UpGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Down3":
                            DownGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Left3":
                            LeftGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Right3":
                            RightGameObjectActiveSelf3 = v.gameObject;
                            break;
                        case "Type4":
                            TypeGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Center4":
                            CenterGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Cycle4":
                            CycleGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Up4":
                            UpGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Down4":
                            DownGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Left4":
                            LeftGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Right4":
                            RightGameObjectActiveSelf4 = v.gameObject;
                            break;
                        case "Type5":
                            TypeGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Center5":
                            CenterGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Cycle5":
                            CycleGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Up5":
                            UpGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Down5":
                            DownGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Left5":
                            LeftGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Right5":
                            RightGameObjectActiveSelf5 = v.gameObject;
                            break;
                        case "Type6":
                            TypeGameObjectActiveSelf6 = v.gameObject;
                            break;
                        case "Center6":
                            CenterGameObjectActiveSelf6 = v.gameObject;
                            break;
                        case "Cycle6":
                            CycleGameObjectActiveSelf6 = v.gameObject;
                            break;
                        case "Up6":
                            UpGameObjectActiveSelf6 = v.gameObject;
                            break;
                        case "Down6":
                            DownGameObjectActiveSelf6 = v.gameObject;
                            break;
                        case "Left6":
                            LeftGameObjectActiveSelf6 = v.gameObject;
                            break;
                        case "Right6":
                            RightGameObjectActiveSelf6 = v.gameObject;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Lu":
                            LuRectTransformLocalPosition = v;
                            break;
                        case "Ld":
                            LdRectTransformLocalPosition = v;
                            break;
                        case "Ru":
                            RuRectTransformLocalPosition = v;
                            break;
                        case "Rd":
                            RdRectTransformLocalPosition = v;
                            break;
                        case "Cycle1":
                            CycleRectTransformLocalScale1 = v;
                            break;
                        case "Up1":
                            UpRectTransformLocalPosition1 = v;
                            break;
                        case "Down1":
                            DownRectTransformLocalPosition1 = v;
                            break;
                        case "Left1":
                            LeftRectTransformLocalPosition1 = v;
                            break;
                        case "Right1":
                            RightRectTransformLocalPosition1 = v;
                            break;
                        case "Cycle2":
                            CycleRectTransformLocalScale2 = v;
                            break;
                        case "Up2":
                            UpRectTransformLocalPosition2 = v;
                            break;
                        case "Down2":
                            DownRectTransformLocalPosition2 = v;
                            break;
                        case "Left2":
                            LeftRectTransformLocalPosition2 = v;
                            break;
                        case "Right2":
                            RightRectTransformLocalPosition2 = v;
                            break;
                        case "Cycle3":
                            CycleRectTransformLocalScale3 = v;
                            break;
                        case "Up3":
                            UpRectTransformLocalPosition3 = v;
                            break;
                        case "Down3":
                            DownRectTransformLocalPosition3 = v;
                            break;
                        case "Left3":
                            LeftRectTransformLocalPosition3 = v;
                            break;
                        case "Right3":
                            RightRectTransformLocalPosition3 = v;
                            break;
                        case "Cycle4":
                            CycleRectTransformLocalScale4 = v;
                            break;
                        case "Up4":
                            UpRectTransformLocalPosition4 = v;
                            break;
                        case "Down4":
                            DownRectTransformLocalPosition4 = v;
                            break;
                        case "Left4":
                            LeftRectTransformLocalPosition4 = v;
                            break;
                        case "Right4":
                            RightRectTransformLocalPosition4 = v;
                            break;
                        case "Cycle5":
                            CycleRectTransformLocalScale5 = v;
                            break;
                        case "Up5":
                            UpRectTransformLocalPosition5 = v;
                            break;
                        case "Down5":
                            DownRectTransformLocalPosition5 = v;
                            break;
                        case "Left5":
                            LeftRectTransformLocalPosition5 = v;
                            break;
                        case "Right5":
                            RightRectTransformLocalPosition5 = v;
                            break;
                        case "Cycle6":
                            CycleRectTransformLocalScale6 = v;
                            break;
                        case "Up6":
                            UpRectTransformLocalPosition6 = v;
                            break;
                        case "Down6":
                            DownRectTransformLocalPosition6 = v;
                            break;
                        case "Left6":
                            LeftRectTransformLocalPosition6 = v;
                            break;
                        case "Right6":
                            RightRectTransformLocalPosition6 = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Center1":
                            CenterImageColor1 = v;
                            break;
                        case "Cycle1":
                            CycleImageColor1 = v;
                            break;
                        case "Up1":
                            UpImageColor1 = v;
                            break;
                        case "Down1":
                            DownImageColor1 = v;
                            break;
                        case "Left1":
                            LeftImageColor1 = v;
                            break;
                        case "Right1":
                            RightImageColor1 = v;
                            break;
                        case "Center2":
                            CenterImageColor2 = v;
                            break;
                        case "Cycle2":
                            CycleImageColor2 = v;
                            break;
                        case "Up2":
                            UpImageColor2 = v;
                            break;
                        case "Down2":
                            DownImageColor2 = v;
                            break;
                        case "Left2":
                            LeftImageColor2 = v;
                            break;
                        case "Right2":
                            RightImageColor2 = v;
                            break;
                        case "Center3":
                            CenterImageColor3 = v;
                            break;
                        case "Cycle3":
                            CycleImageColor3 = v;
                            break;
                        case "Up3":
                            UpImageColor3 = v;
                            break;
                        case "Down3":
                            DownImageColor3 = v;
                            break;
                        case "Left3":
                            LeftImageColor3 = v;
                            break;
                        case "Right3":
                            RightImageColor3 = v;
                            break;
                        case "Center4":
                            CenterImageColor4 = v;
                            break;
                        case "Cycle4":
                            CycleImageColor4 = v;
                            break;
                        case "Up4":
                            UpImageColor4 = v;
                            break;
                        case "Down4":
                            DownImageColor4 = v;
                            break;
                        case "Left4":
                            LeftImageColor4 = v;
                            break;
                        case "Right4":
                            RightImageColor4 = v;
                            break;
                        case "Center5":
                            CenterImageColor5 = v;
                            break;
                        case "Cycle5":
                            CycleImageColor5 = v;
                            break;
                        case "Up5":
                            UpImageColor5 = v;
                            break;
                        case "Down5":
                            DownImageColor5 = v;
                            break;
                        case "Left5":
                            LeftImageColor5 = v;
                            break;
                        case "Right5":
                            RightImageColor5 = v;
                            break;
                        case "Center6":
                            CenterImageColor6 = v;
                            break;
                        case "Cycle6":
                            CycleImageColor6 = v;
                            break;
                        case "Up6":
                            UpImageColor6 = v;
                            break;
                        case "Down6":
                            DownImageColor6 = v;
                            break;
                        case "Left6":
                            LeftImageColor6 = v;
                            break;
                        case "Right6":
                            RightImageColor6 = v;
                            break;
                    }
                }

            }
        }


        private bool _showGameObjectActiveSelf;
        private bool _hitGameObjectActiveSelf;
        private Vector3 _luRectTransformLocalPosition;
        private Vector3 _ldRectTransformLocalPosition;
        private Vector3 _ruRectTransformLocalPosition;
        private Vector3 _rdRectTransformLocalPosition;
        private bool _awpGameObjectActiveSelf;
        private bool _typeGameObjectActiveSelf1;
        private bool _centerGameObjectActiveSelf1;
        private Color _centerImageColor1;
        private bool _cycleGameObjectActiveSelf1;
        private Color _cycleImageColor1;
        private Vector3 _cycleRectTransformLocalScale1;
        private bool _upGameObjectActiveSelf1;
        private Color _upImageColor1;
        private Vector3 _upRectTransformLocalPosition1;
        private bool _downGameObjectActiveSelf1;
        private Color _downImageColor1;
        private Vector3 _downRectTransformLocalPosition1;
        private bool _leftGameObjectActiveSelf1;
        private Color _leftImageColor1;
        private Vector3 _leftRectTransformLocalPosition1;
        private bool _rightGameObjectActiveSelf1;
        private Color _rightImageColor1;
        private Vector3 _rightRectTransformLocalPosition1;
        private bool _typeGameObjectActiveSelf2;
        private bool _centerGameObjectActiveSelf2;
        private Color _centerImageColor2;
        private bool _cycleGameObjectActiveSelf2;
        private Color _cycleImageColor2;
        private Vector3 _cycleRectTransformLocalScale2;
        private bool _upGameObjectActiveSelf2;
        private Color _upImageColor2;
        private Vector3 _upRectTransformLocalPosition2;
        private bool _downGameObjectActiveSelf2;
        private Color _downImageColor2;
        private Vector3 _downRectTransformLocalPosition2;
        private bool _leftGameObjectActiveSelf2;
        private Color _leftImageColor2;
        private Vector3 _leftRectTransformLocalPosition2;
        private bool _rightGameObjectActiveSelf2;
        private Color _rightImageColor2;
        private Vector3 _rightRectTransformLocalPosition2;
        private bool _typeGameObjectActiveSelf3;
        private bool _centerGameObjectActiveSelf3;
        private Color _centerImageColor3;
        private bool _cycleGameObjectActiveSelf3;
        private Color _cycleImageColor3;
        private Vector3 _cycleRectTransformLocalScale3;
        private bool _upGameObjectActiveSelf3;
        private Color _upImageColor3;
        private Vector3 _upRectTransformLocalPosition3;
        private bool _downGameObjectActiveSelf3;
        private Color _downImageColor3;
        private Vector3 _downRectTransformLocalPosition3;
        private bool _leftGameObjectActiveSelf3;
        private Color _leftImageColor3;
        private Vector3 _leftRectTransformLocalPosition3;
        private bool _rightGameObjectActiveSelf3;
        private Color _rightImageColor3;
        private Vector3 _rightRectTransformLocalPosition3;
        private bool _typeGameObjectActiveSelf4;
        private bool _centerGameObjectActiveSelf4;
        private Color _centerImageColor4;
        private bool _cycleGameObjectActiveSelf4;
        private Color _cycleImageColor4;
        private Vector3 _cycleRectTransformLocalScale4;
        private bool _upGameObjectActiveSelf4;
        private Color _upImageColor4;
        private Vector3 _upRectTransformLocalPosition4;
        private bool _downGameObjectActiveSelf4;
        private Color _downImageColor4;
        private Vector3 _downRectTransformLocalPosition4;
        private bool _leftGameObjectActiveSelf4;
        private Color _leftImageColor4;
        private Vector3 _leftRectTransformLocalPosition4;
        private bool _rightGameObjectActiveSelf4;
        private Color _rightImageColor4;
        private Vector3 _rightRectTransformLocalPosition4;
        private bool _typeGameObjectActiveSelf5;
        private bool _centerGameObjectActiveSelf5;
        private Color _centerImageColor5;
        private bool _cycleGameObjectActiveSelf5;
        private Color _cycleImageColor5;
        private Vector3 _cycleRectTransformLocalScale5;
        private bool _upGameObjectActiveSelf5;
        private Color _upImageColor5;
        private Vector3 _upRectTransformLocalPosition5;
        private bool _downGameObjectActiveSelf5;
        private Color _downImageColor5;
        private Vector3 _downRectTransformLocalPosition5;
        private bool _leftGameObjectActiveSelf5;
        private Color _leftImageColor5;
        private Vector3 _leftRectTransformLocalPosition5;
        private bool _rightGameObjectActiveSelf5;
        private Color _rightImageColor5;
        private Vector3 _rightRectTransformLocalPosition5;
        private bool _typeGameObjectActiveSelf6;
        private bool _centerGameObjectActiveSelf6;
        private Color _centerImageColor6;
        private bool _cycleGameObjectActiveSelf6;
        private Color _cycleImageColor6;
        private Vector3 _cycleRectTransformLocalScale6;
        private bool _upGameObjectActiveSelf6;
        private Color _upImageColor6;
        private Vector3 _upRectTransformLocalPosition6;
        private bool _downGameObjectActiveSelf6;
        private Color _downImageColor6;
        private Vector3 _downRectTransformLocalPosition6;
        private bool _leftGameObjectActiveSelf6;
        private Color _leftImageColor6;
        private Vector3 _leftRectTransformLocalPosition6;
        private bool _rightGameObjectActiveSelf6;
        private Color _rightImageColor6;
        private Vector3 _rightRectTransformLocalPosition6;
        public bool ShowGameObjectActiveSelf { get { return _showGameObjectActiveSelf; } set {if(_showGameObjectActiveSelf != value) Set(ref _showGameObjectActiveSelf, value, "ShowGameObjectActiveSelf"); } }
        public bool HitGameObjectActiveSelf { get { return _hitGameObjectActiveSelf; } set {if(_hitGameObjectActiveSelf != value) Set(ref _hitGameObjectActiveSelf, value, "HitGameObjectActiveSelf"); } }
        public Vector3 LuRectTransformLocalPosition { get { return _luRectTransformLocalPosition; } set {if(_luRectTransformLocalPosition != value) Set(ref _luRectTransformLocalPosition, value, "LuRectTransformLocalPosition"); } }
        public Vector3 LdRectTransformLocalPosition { get { return _ldRectTransformLocalPosition; } set {if(_ldRectTransformLocalPosition != value) Set(ref _ldRectTransformLocalPosition, value, "LdRectTransformLocalPosition"); } }
        public Vector3 RuRectTransformLocalPosition { get { return _ruRectTransformLocalPosition; } set {if(_ruRectTransformLocalPosition != value) Set(ref _ruRectTransformLocalPosition, value, "RuRectTransformLocalPosition"); } }
        public Vector3 RdRectTransformLocalPosition { get { return _rdRectTransformLocalPosition; } set {if(_rdRectTransformLocalPosition != value) Set(ref _rdRectTransformLocalPosition, value, "RdRectTransformLocalPosition"); } }
        public bool AwpGameObjectActiveSelf { get { return _awpGameObjectActiveSelf; } set {if(_awpGameObjectActiveSelf != value) Set(ref _awpGameObjectActiveSelf, value, "AwpGameObjectActiveSelf"); } }
        public bool TypeGameObjectActiveSelf1 { get { return _typeGameObjectActiveSelf1; } set {if(_typeGameObjectActiveSelf1 != value) Set(ref _typeGameObjectActiveSelf1, value, "TypeGameObjectActiveSelf1"); } }
        public bool CenterGameObjectActiveSelf1 { get { return _centerGameObjectActiveSelf1; } set {if(_centerGameObjectActiveSelf1 != value) Set(ref _centerGameObjectActiveSelf1, value, "CenterGameObjectActiveSelf1"); } }
        public Color CenterImageColor1 { get { return _centerImageColor1; } set {if(_centerImageColor1 != value) Set(ref _centerImageColor1, value, "CenterImageColor1"); } }
        public bool CycleGameObjectActiveSelf1 { get { return _cycleGameObjectActiveSelf1; } set {if(_cycleGameObjectActiveSelf1 != value) Set(ref _cycleGameObjectActiveSelf1, value, "CycleGameObjectActiveSelf1"); } }
        public Color CycleImageColor1 { get { return _cycleImageColor1; } set {if(_cycleImageColor1 != value) Set(ref _cycleImageColor1, value, "CycleImageColor1"); } }
        public Vector3 CycleRectTransformLocalScale1 { get { return _cycleRectTransformLocalScale1; } set {if(_cycleRectTransformLocalScale1 != value) Set(ref _cycleRectTransformLocalScale1, value, "CycleRectTransformLocalScale1"); } }
        public bool UpGameObjectActiveSelf1 { get { return _upGameObjectActiveSelf1; } set {if(_upGameObjectActiveSelf1 != value) Set(ref _upGameObjectActiveSelf1, value, "UpGameObjectActiveSelf1"); } }
        public Color UpImageColor1 { get { return _upImageColor1; } set {if(_upImageColor1 != value) Set(ref _upImageColor1, value, "UpImageColor1"); } }
        public Vector3 UpRectTransformLocalPosition1 { get { return _upRectTransformLocalPosition1; } set {if(_upRectTransformLocalPosition1 != value) Set(ref _upRectTransformLocalPosition1, value, "UpRectTransformLocalPosition1"); } }
        public bool DownGameObjectActiveSelf1 { get { return _downGameObjectActiveSelf1; } set {if(_downGameObjectActiveSelf1 != value) Set(ref _downGameObjectActiveSelf1, value, "DownGameObjectActiveSelf1"); } }
        public Color DownImageColor1 { get { return _downImageColor1; } set {if(_downImageColor1 != value) Set(ref _downImageColor1, value, "DownImageColor1"); } }
        public Vector3 DownRectTransformLocalPosition1 { get { return _downRectTransformLocalPosition1; } set {if(_downRectTransformLocalPosition1 != value) Set(ref _downRectTransformLocalPosition1, value, "DownRectTransformLocalPosition1"); } }
        public bool LeftGameObjectActiveSelf1 { get { return _leftGameObjectActiveSelf1; } set {if(_leftGameObjectActiveSelf1 != value) Set(ref _leftGameObjectActiveSelf1, value, "LeftGameObjectActiveSelf1"); } }
        public Color LeftImageColor1 { get { return _leftImageColor1; } set {if(_leftImageColor1 != value) Set(ref _leftImageColor1, value, "LeftImageColor1"); } }
        public Vector3 LeftRectTransformLocalPosition1 { get { return _leftRectTransformLocalPosition1; } set {if(_leftRectTransformLocalPosition1 != value) Set(ref _leftRectTransformLocalPosition1, value, "LeftRectTransformLocalPosition1"); } }
        public bool RightGameObjectActiveSelf1 { get { return _rightGameObjectActiveSelf1; } set {if(_rightGameObjectActiveSelf1 != value) Set(ref _rightGameObjectActiveSelf1, value, "RightGameObjectActiveSelf1"); } }
        public Color RightImageColor1 { get { return _rightImageColor1; } set {if(_rightImageColor1 != value) Set(ref _rightImageColor1, value, "RightImageColor1"); } }
        public Vector3 RightRectTransformLocalPosition1 { get { return _rightRectTransformLocalPosition1; } set {if(_rightRectTransformLocalPosition1 != value) Set(ref _rightRectTransformLocalPosition1, value, "RightRectTransformLocalPosition1"); } }
        public bool TypeGameObjectActiveSelf2 { get { return _typeGameObjectActiveSelf2; } set {if(_typeGameObjectActiveSelf2 != value) Set(ref _typeGameObjectActiveSelf2, value, "TypeGameObjectActiveSelf2"); } }
        public bool CenterGameObjectActiveSelf2 { get { return _centerGameObjectActiveSelf2; } set {if(_centerGameObjectActiveSelf2 != value) Set(ref _centerGameObjectActiveSelf2, value, "CenterGameObjectActiveSelf2"); } }
        public Color CenterImageColor2 { get { return _centerImageColor2; } set {if(_centerImageColor2 != value) Set(ref _centerImageColor2, value, "CenterImageColor2"); } }
        public bool CycleGameObjectActiveSelf2 { get { return _cycleGameObjectActiveSelf2; } set {if(_cycleGameObjectActiveSelf2 != value) Set(ref _cycleGameObjectActiveSelf2, value, "CycleGameObjectActiveSelf2"); } }
        public Color CycleImageColor2 { get { return _cycleImageColor2; } set {if(_cycleImageColor2 != value) Set(ref _cycleImageColor2, value, "CycleImageColor2"); } }
        public Vector3 CycleRectTransformLocalScale2 { get { return _cycleRectTransformLocalScale2; } set {if(_cycleRectTransformLocalScale2 != value) Set(ref _cycleRectTransformLocalScale2, value, "CycleRectTransformLocalScale2"); } }
        public bool UpGameObjectActiveSelf2 { get { return _upGameObjectActiveSelf2; } set {if(_upGameObjectActiveSelf2 != value) Set(ref _upGameObjectActiveSelf2, value, "UpGameObjectActiveSelf2"); } }
        public Color UpImageColor2 { get { return _upImageColor2; } set {if(_upImageColor2 != value) Set(ref _upImageColor2, value, "UpImageColor2"); } }
        public Vector3 UpRectTransformLocalPosition2 { get { return _upRectTransformLocalPosition2; } set {if(_upRectTransformLocalPosition2 != value) Set(ref _upRectTransformLocalPosition2, value, "UpRectTransformLocalPosition2"); } }
        public bool DownGameObjectActiveSelf2 { get { return _downGameObjectActiveSelf2; } set {if(_downGameObjectActiveSelf2 != value) Set(ref _downGameObjectActiveSelf2, value, "DownGameObjectActiveSelf2"); } }
        public Color DownImageColor2 { get { return _downImageColor2; } set {if(_downImageColor2 != value) Set(ref _downImageColor2, value, "DownImageColor2"); } }
        public Vector3 DownRectTransformLocalPosition2 { get { return _downRectTransformLocalPosition2; } set {if(_downRectTransformLocalPosition2 != value) Set(ref _downRectTransformLocalPosition2, value, "DownRectTransformLocalPosition2"); } }
        public bool LeftGameObjectActiveSelf2 { get { return _leftGameObjectActiveSelf2; } set {if(_leftGameObjectActiveSelf2 != value) Set(ref _leftGameObjectActiveSelf2, value, "LeftGameObjectActiveSelf2"); } }
        public Color LeftImageColor2 { get { return _leftImageColor2; } set {if(_leftImageColor2 != value) Set(ref _leftImageColor2, value, "LeftImageColor2"); } }
        public Vector3 LeftRectTransformLocalPosition2 { get { return _leftRectTransformLocalPosition2; } set {if(_leftRectTransformLocalPosition2 != value) Set(ref _leftRectTransformLocalPosition2, value, "LeftRectTransformLocalPosition2"); } }
        public bool RightGameObjectActiveSelf2 { get { return _rightGameObjectActiveSelf2; } set {if(_rightGameObjectActiveSelf2 != value) Set(ref _rightGameObjectActiveSelf2, value, "RightGameObjectActiveSelf2"); } }
        public Color RightImageColor2 { get { return _rightImageColor2; } set {if(_rightImageColor2 != value) Set(ref _rightImageColor2, value, "RightImageColor2"); } }
        public Vector3 RightRectTransformLocalPosition2 { get { return _rightRectTransformLocalPosition2; } set {if(_rightRectTransformLocalPosition2 != value) Set(ref _rightRectTransformLocalPosition2, value, "RightRectTransformLocalPosition2"); } }
        public bool TypeGameObjectActiveSelf3 { get { return _typeGameObjectActiveSelf3; } set {if(_typeGameObjectActiveSelf3 != value) Set(ref _typeGameObjectActiveSelf3, value, "TypeGameObjectActiveSelf3"); } }
        public bool CenterGameObjectActiveSelf3 { get { return _centerGameObjectActiveSelf3; } set {if(_centerGameObjectActiveSelf3 != value) Set(ref _centerGameObjectActiveSelf3, value, "CenterGameObjectActiveSelf3"); } }
        public Color CenterImageColor3 { get { return _centerImageColor3; } set {if(_centerImageColor3 != value) Set(ref _centerImageColor3, value, "CenterImageColor3"); } }
        public bool CycleGameObjectActiveSelf3 { get { return _cycleGameObjectActiveSelf3; } set {if(_cycleGameObjectActiveSelf3 != value) Set(ref _cycleGameObjectActiveSelf3, value, "CycleGameObjectActiveSelf3"); } }
        public Color CycleImageColor3 { get { return _cycleImageColor3; } set {if(_cycleImageColor3 != value) Set(ref _cycleImageColor3, value, "CycleImageColor3"); } }
        public Vector3 CycleRectTransformLocalScale3 { get { return _cycleRectTransformLocalScale3; } set {if(_cycleRectTransformLocalScale3 != value) Set(ref _cycleRectTransformLocalScale3, value, "CycleRectTransformLocalScale3"); } }
        public bool UpGameObjectActiveSelf3 { get { return _upGameObjectActiveSelf3; } set {if(_upGameObjectActiveSelf3 != value) Set(ref _upGameObjectActiveSelf3, value, "UpGameObjectActiveSelf3"); } }
        public Color UpImageColor3 { get { return _upImageColor3; } set {if(_upImageColor3 != value) Set(ref _upImageColor3, value, "UpImageColor3"); } }
        public Vector3 UpRectTransformLocalPosition3 { get { return _upRectTransformLocalPosition3; } set {if(_upRectTransformLocalPosition3 != value) Set(ref _upRectTransformLocalPosition3, value, "UpRectTransformLocalPosition3"); } }
        public bool DownGameObjectActiveSelf3 { get { return _downGameObjectActiveSelf3; } set {if(_downGameObjectActiveSelf3 != value) Set(ref _downGameObjectActiveSelf3, value, "DownGameObjectActiveSelf3"); } }
        public Color DownImageColor3 { get { return _downImageColor3; } set {if(_downImageColor3 != value) Set(ref _downImageColor3, value, "DownImageColor3"); } }
        public Vector3 DownRectTransformLocalPosition3 { get { return _downRectTransformLocalPosition3; } set {if(_downRectTransformLocalPosition3 != value) Set(ref _downRectTransformLocalPosition3, value, "DownRectTransformLocalPosition3"); } }
        public bool LeftGameObjectActiveSelf3 { get { return _leftGameObjectActiveSelf3; } set {if(_leftGameObjectActiveSelf3 != value) Set(ref _leftGameObjectActiveSelf3, value, "LeftGameObjectActiveSelf3"); } }
        public Color LeftImageColor3 { get { return _leftImageColor3; } set {if(_leftImageColor3 != value) Set(ref _leftImageColor3, value, "LeftImageColor3"); } }
        public Vector3 LeftRectTransformLocalPosition3 { get { return _leftRectTransformLocalPosition3; } set {if(_leftRectTransformLocalPosition3 != value) Set(ref _leftRectTransformLocalPosition3, value, "LeftRectTransformLocalPosition3"); } }
        public bool RightGameObjectActiveSelf3 { get { return _rightGameObjectActiveSelf3; } set {if(_rightGameObjectActiveSelf3 != value) Set(ref _rightGameObjectActiveSelf3, value, "RightGameObjectActiveSelf3"); } }
        public Color RightImageColor3 { get { return _rightImageColor3; } set {if(_rightImageColor3 != value) Set(ref _rightImageColor3, value, "RightImageColor3"); } }
        public Vector3 RightRectTransformLocalPosition3 { get { return _rightRectTransformLocalPosition3; } set {if(_rightRectTransformLocalPosition3 != value) Set(ref _rightRectTransformLocalPosition3, value, "RightRectTransformLocalPosition3"); } }
        public bool TypeGameObjectActiveSelf4 { get { return _typeGameObjectActiveSelf4; } set {if(_typeGameObjectActiveSelf4 != value) Set(ref _typeGameObjectActiveSelf4, value, "TypeGameObjectActiveSelf4"); } }
        public bool CenterGameObjectActiveSelf4 { get { return _centerGameObjectActiveSelf4; } set {if(_centerGameObjectActiveSelf4 != value) Set(ref _centerGameObjectActiveSelf4, value, "CenterGameObjectActiveSelf4"); } }
        public Color CenterImageColor4 { get { return _centerImageColor4; } set {if(_centerImageColor4 != value) Set(ref _centerImageColor4, value, "CenterImageColor4"); } }
        public bool CycleGameObjectActiveSelf4 { get { return _cycleGameObjectActiveSelf4; } set {if(_cycleGameObjectActiveSelf4 != value) Set(ref _cycleGameObjectActiveSelf4, value, "CycleGameObjectActiveSelf4"); } }
        public Color CycleImageColor4 { get { return _cycleImageColor4; } set {if(_cycleImageColor4 != value) Set(ref _cycleImageColor4, value, "CycleImageColor4"); } }
        public Vector3 CycleRectTransformLocalScale4 { get { return _cycleRectTransformLocalScale4; } set {if(_cycleRectTransformLocalScale4 != value) Set(ref _cycleRectTransformLocalScale4, value, "CycleRectTransformLocalScale4"); } }
        public bool UpGameObjectActiveSelf4 { get { return _upGameObjectActiveSelf4; } set {if(_upGameObjectActiveSelf4 != value) Set(ref _upGameObjectActiveSelf4, value, "UpGameObjectActiveSelf4"); } }
        public Color UpImageColor4 { get { return _upImageColor4; } set {if(_upImageColor4 != value) Set(ref _upImageColor4, value, "UpImageColor4"); } }
        public Vector3 UpRectTransformLocalPosition4 { get { return _upRectTransformLocalPosition4; } set {if(_upRectTransformLocalPosition4 != value) Set(ref _upRectTransformLocalPosition4, value, "UpRectTransformLocalPosition4"); } }
        public bool DownGameObjectActiveSelf4 { get { return _downGameObjectActiveSelf4; } set {if(_downGameObjectActiveSelf4 != value) Set(ref _downGameObjectActiveSelf4, value, "DownGameObjectActiveSelf4"); } }
        public Color DownImageColor4 { get { return _downImageColor4; } set {if(_downImageColor4 != value) Set(ref _downImageColor4, value, "DownImageColor4"); } }
        public Vector3 DownRectTransformLocalPosition4 { get { return _downRectTransformLocalPosition4; } set {if(_downRectTransformLocalPosition4 != value) Set(ref _downRectTransformLocalPosition4, value, "DownRectTransformLocalPosition4"); } }
        public bool LeftGameObjectActiveSelf4 { get { return _leftGameObjectActiveSelf4; } set {if(_leftGameObjectActiveSelf4 != value) Set(ref _leftGameObjectActiveSelf4, value, "LeftGameObjectActiveSelf4"); } }
        public Color LeftImageColor4 { get { return _leftImageColor4; } set {if(_leftImageColor4 != value) Set(ref _leftImageColor4, value, "LeftImageColor4"); } }
        public Vector3 LeftRectTransformLocalPosition4 { get { return _leftRectTransformLocalPosition4; } set {if(_leftRectTransformLocalPosition4 != value) Set(ref _leftRectTransformLocalPosition4, value, "LeftRectTransformLocalPosition4"); } }
        public bool RightGameObjectActiveSelf4 { get { return _rightGameObjectActiveSelf4; } set {if(_rightGameObjectActiveSelf4 != value) Set(ref _rightGameObjectActiveSelf4, value, "RightGameObjectActiveSelf4"); } }
        public Color RightImageColor4 { get { return _rightImageColor4; } set {if(_rightImageColor4 != value) Set(ref _rightImageColor4, value, "RightImageColor4"); } }
        public Vector3 RightRectTransformLocalPosition4 { get { return _rightRectTransformLocalPosition4; } set {if(_rightRectTransformLocalPosition4 != value) Set(ref _rightRectTransformLocalPosition4, value, "RightRectTransformLocalPosition4"); } }
        public bool TypeGameObjectActiveSelf5 { get { return _typeGameObjectActiveSelf5; } set {if(_typeGameObjectActiveSelf5 != value) Set(ref _typeGameObjectActiveSelf5, value, "TypeGameObjectActiveSelf5"); } }
        public bool CenterGameObjectActiveSelf5 { get { return _centerGameObjectActiveSelf5; } set {if(_centerGameObjectActiveSelf5 != value) Set(ref _centerGameObjectActiveSelf5, value, "CenterGameObjectActiveSelf5"); } }
        public Color CenterImageColor5 { get { return _centerImageColor5; } set {if(_centerImageColor5 != value) Set(ref _centerImageColor5, value, "CenterImageColor5"); } }
        public bool CycleGameObjectActiveSelf5 { get { return _cycleGameObjectActiveSelf5; } set {if(_cycleGameObjectActiveSelf5 != value) Set(ref _cycleGameObjectActiveSelf5, value, "CycleGameObjectActiveSelf5"); } }
        public Color CycleImageColor5 { get { return _cycleImageColor5; } set {if(_cycleImageColor5 != value) Set(ref _cycleImageColor5, value, "CycleImageColor5"); } }
        public Vector3 CycleRectTransformLocalScale5 { get { return _cycleRectTransformLocalScale5; } set {if(_cycleRectTransformLocalScale5 != value) Set(ref _cycleRectTransformLocalScale5, value, "CycleRectTransformLocalScale5"); } }
        public bool UpGameObjectActiveSelf5 { get { return _upGameObjectActiveSelf5; } set {if(_upGameObjectActiveSelf5 != value) Set(ref _upGameObjectActiveSelf5, value, "UpGameObjectActiveSelf5"); } }
        public Color UpImageColor5 { get { return _upImageColor5; } set {if(_upImageColor5 != value) Set(ref _upImageColor5, value, "UpImageColor5"); } }
        public Vector3 UpRectTransformLocalPosition5 { get { return _upRectTransformLocalPosition5; } set {if(_upRectTransformLocalPosition5 != value) Set(ref _upRectTransformLocalPosition5, value, "UpRectTransformLocalPosition5"); } }
        public bool DownGameObjectActiveSelf5 { get { return _downGameObjectActiveSelf5; } set {if(_downGameObjectActiveSelf5 != value) Set(ref _downGameObjectActiveSelf5, value, "DownGameObjectActiveSelf5"); } }
        public Color DownImageColor5 { get { return _downImageColor5; } set {if(_downImageColor5 != value) Set(ref _downImageColor5, value, "DownImageColor5"); } }
        public Vector3 DownRectTransformLocalPosition5 { get { return _downRectTransformLocalPosition5; } set {if(_downRectTransformLocalPosition5 != value) Set(ref _downRectTransformLocalPosition5, value, "DownRectTransformLocalPosition5"); } }
        public bool LeftGameObjectActiveSelf5 { get { return _leftGameObjectActiveSelf5; } set {if(_leftGameObjectActiveSelf5 != value) Set(ref _leftGameObjectActiveSelf5, value, "LeftGameObjectActiveSelf5"); } }
        public Color LeftImageColor5 { get { return _leftImageColor5; } set {if(_leftImageColor5 != value) Set(ref _leftImageColor5, value, "LeftImageColor5"); } }
        public Vector3 LeftRectTransformLocalPosition5 { get { return _leftRectTransformLocalPosition5; } set {if(_leftRectTransformLocalPosition5 != value) Set(ref _leftRectTransformLocalPosition5, value, "LeftRectTransformLocalPosition5"); } }
        public bool RightGameObjectActiveSelf5 { get { return _rightGameObjectActiveSelf5; } set {if(_rightGameObjectActiveSelf5 != value) Set(ref _rightGameObjectActiveSelf5, value, "RightGameObjectActiveSelf5"); } }
        public Color RightImageColor5 { get { return _rightImageColor5; } set {if(_rightImageColor5 != value) Set(ref _rightImageColor5, value, "RightImageColor5"); } }
        public Vector3 RightRectTransformLocalPosition5 { get { return _rightRectTransformLocalPosition5; } set {if(_rightRectTransformLocalPosition5 != value) Set(ref _rightRectTransformLocalPosition5, value, "RightRectTransformLocalPosition5"); } }
        public bool TypeGameObjectActiveSelf6 { get { return _typeGameObjectActiveSelf6; } set {if(_typeGameObjectActiveSelf6 != value) Set(ref _typeGameObjectActiveSelf6, value, "TypeGameObjectActiveSelf6"); } }
        public bool CenterGameObjectActiveSelf6 { get { return _centerGameObjectActiveSelf6; } set {if(_centerGameObjectActiveSelf6 != value) Set(ref _centerGameObjectActiveSelf6, value, "CenterGameObjectActiveSelf6"); } }
        public Color CenterImageColor6 { get { return _centerImageColor6; } set {if(_centerImageColor6 != value) Set(ref _centerImageColor6, value, "CenterImageColor6"); } }
        public bool CycleGameObjectActiveSelf6 { get { return _cycleGameObjectActiveSelf6; } set {if(_cycleGameObjectActiveSelf6 != value) Set(ref _cycleGameObjectActiveSelf6, value, "CycleGameObjectActiveSelf6"); } }
        public Color CycleImageColor6 { get { return _cycleImageColor6; } set {if(_cycleImageColor6 != value) Set(ref _cycleImageColor6, value, "CycleImageColor6"); } }
        public Vector3 CycleRectTransformLocalScale6 { get { return _cycleRectTransformLocalScale6; } set {if(_cycleRectTransformLocalScale6 != value) Set(ref _cycleRectTransformLocalScale6, value, "CycleRectTransformLocalScale6"); } }
        public bool UpGameObjectActiveSelf6 { get { return _upGameObjectActiveSelf6; } set {if(_upGameObjectActiveSelf6 != value) Set(ref _upGameObjectActiveSelf6, value, "UpGameObjectActiveSelf6"); } }
        public Color UpImageColor6 { get { return _upImageColor6; } set {if(_upImageColor6 != value) Set(ref _upImageColor6, value, "UpImageColor6"); } }
        public Vector3 UpRectTransformLocalPosition6 { get { return _upRectTransformLocalPosition6; } set {if(_upRectTransformLocalPosition6 != value) Set(ref _upRectTransformLocalPosition6, value, "UpRectTransformLocalPosition6"); } }
        public bool DownGameObjectActiveSelf6 { get { return _downGameObjectActiveSelf6; } set {if(_downGameObjectActiveSelf6 != value) Set(ref _downGameObjectActiveSelf6, value, "DownGameObjectActiveSelf6"); } }
        public Color DownImageColor6 { get { return _downImageColor6; } set {if(_downImageColor6 != value) Set(ref _downImageColor6, value, "DownImageColor6"); } }
        public Vector3 DownRectTransformLocalPosition6 { get { return _downRectTransformLocalPosition6; } set {if(_downRectTransformLocalPosition6 != value) Set(ref _downRectTransformLocalPosition6, value, "DownRectTransformLocalPosition6"); } }
        public bool LeftGameObjectActiveSelf6 { get { return _leftGameObjectActiveSelf6; } set {if(_leftGameObjectActiveSelf6 != value) Set(ref _leftGameObjectActiveSelf6, value, "LeftGameObjectActiveSelf6"); } }
        public Color LeftImageColor6 { get { return _leftImageColor6; } set {if(_leftImageColor6 != value) Set(ref _leftImageColor6, value, "LeftImageColor6"); } }
        public Vector3 LeftRectTransformLocalPosition6 { get { return _leftRectTransformLocalPosition6; } set {if(_leftRectTransformLocalPosition6 != value) Set(ref _leftRectTransformLocalPosition6, value, "LeftRectTransformLocalPosition6"); } }
        public bool RightGameObjectActiveSelf6 { get { return _rightGameObjectActiveSelf6; } set {if(_rightGameObjectActiveSelf6 != value) Set(ref _rightGameObjectActiveSelf6, value, "RightGameObjectActiveSelf6"); } }
        public Color RightImageColor6 { get { return _rightImageColor6; } set {if(_rightImageColor6 != value) Set(ref _rightImageColor6, value, "RightImageColor6"); } }
        public Vector3 RightRectTransformLocalPosition6 { get { return _rightRectTransformLocalPosition6; } set {if(_rightRectTransformLocalPosition6 != value) Set(ref _rightRectTransformLocalPosition6, value, "RightRectTransformLocalPosition6"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonCrossView _view;
		
		public void Destory()
        {
            if (_viewGameObject != null)
            {
				UnityEngine.Object.Destroy(_viewGameObject);
            }
        }
		public void Visible(bool isViaible)
		{
		    if (_viewGameObject != null)
            {
				_viewGameObject.SetActive(isViaible);
            }
		
		}
		public void SetCanvasEnabled(bool value)
        {
            if (_viewCanvas != null)
            {
                _viewCanvas.enabled = value;
            }
        }
        public void CreateBinding(GameObject obj)
        {
			_viewGameObject = obj;
			_viewCanvas = _viewGameObject.GetComponent<Canvas>();

			var view = obj.GetComponent<CommonCrossView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonCrossView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonCrossView, CommonCrossViewModel> bindingSet =
                view.CreateBindingSet<CommonCrossView, CommonCrossViewModel>();

            view.oriShowGameObjectActiveSelf = _showGameObjectActiveSelf = view.ShowGameObjectActiveSelf.activeSelf;
            bindingSet.Bind(view.ShowGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.ShowGameObjectActiveSelf).OneWay();
            view.oriHitGameObjectActiveSelf = _hitGameObjectActiveSelf = view.HitGameObjectActiveSelf.activeSelf;
            bindingSet.Bind(view.HitGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.HitGameObjectActiveSelf).OneWay();
            view.oriLuRectTransformLocalPosition = _luRectTransformLocalPosition = view.LuRectTransformLocalPosition.localPosition;
            bindingSet.Bind(view.LuRectTransformLocalPosition).For(v => v.localPosition).To(vm => vm.LuRectTransformLocalPosition).OneWay();
            view.oriLdRectTransformLocalPosition = _ldRectTransformLocalPosition = view.LdRectTransformLocalPosition.localPosition;
            bindingSet.Bind(view.LdRectTransformLocalPosition).For(v => v.localPosition).To(vm => vm.LdRectTransformLocalPosition).OneWay();
            view.oriRuRectTransformLocalPosition = _ruRectTransformLocalPosition = view.RuRectTransformLocalPosition.localPosition;
            bindingSet.Bind(view.RuRectTransformLocalPosition).For(v => v.localPosition).To(vm => vm.RuRectTransformLocalPosition).OneWay();
            view.oriRdRectTransformLocalPosition = _rdRectTransformLocalPosition = view.RdRectTransformLocalPosition.localPosition;
            bindingSet.Bind(view.RdRectTransformLocalPosition).For(v => v.localPosition).To(vm => vm.RdRectTransformLocalPosition).OneWay();
            view.oriAwpGameObjectActiveSelf = _awpGameObjectActiveSelf = view.AwpGameObjectActiveSelf.activeSelf;
            bindingSet.Bind(view.AwpGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.AwpGameObjectActiveSelf).OneWay();
            bindingSet.Bind(view.TypeGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.TypeGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.CenterGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.CenterGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.CenterImageColor1).For(v => v.color).To(vm => vm.CenterImageColor1).OneWay();
            bindingSet.Bind(view.CycleGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.CycleGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.CycleImageColor1).For(v => v.color).To(vm => vm.CycleImageColor1).OneWay();
            bindingSet.Bind(view.CycleRectTransformLocalScale1).For(v => v.localScale).To(vm => vm.CycleRectTransformLocalScale1).OneWay();
            bindingSet.Bind(view.UpGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.UpGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.UpImageColor1).For(v => v.color).To(vm => vm.UpImageColor1).OneWay();
            bindingSet.Bind(view.UpRectTransformLocalPosition1).For(v => v.localPosition).To(vm => vm.UpRectTransformLocalPosition1).OneWay();
            bindingSet.Bind(view.DownGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.DownGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.DownImageColor1).For(v => v.color).To(vm => vm.DownImageColor1).OneWay();
            bindingSet.Bind(view.DownRectTransformLocalPosition1).For(v => v.localPosition).To(vm => vm.DownRectTransformLocalPosition1).OneWay();
            bindingSet.Bind(view.LeftGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.LeftGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.LeftImageColor1).For(v => v.color).To(vm => vm.LeftImageColor1).OneWay();
            bindingSet.Bind(view.LeftRectTransformLocalPosition1).For(v => v.localPosition).To(vm => vm.LeftRectTransformLocalPosition1).OneWay();
            bindingSet.Bind(view.RightGameObjectActiveSelf1).For(v => v.activeSelf).To(vm => vm.RightGameObjectActiveSelf1).OneWay();
            bindingSet.Bind(view.RightImageColor1).For(v => v.color).To(vm => vm.RightImageColor1).OneWay();
            bindingSet.Bind(view.RightRectTransformLocalPosition1).For(v => v.localPosition).To(vm => vm.RightRectTransformLocalPosition1).OneWay();
            bindingSet.Bind(view.TypeGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.TypeGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.CenterGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.CenterGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.CenterImageColor2).For(v => v.color).To(vm => vm.CenterImageColor2).OneWay();
            bindingSet.Bind(view.CycleGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.CycleGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.CycleImageColor2).For(v => v.color).To(vm => vm.CycleImageColor2).OneWay();
            bindingSet.Bind(view.CycleRectTransformLocalScale2).For(v => v.localScale).To(vm => vm.CycleRectTransformLocalScale2).OneWay();
            bindingSet.Bind(view.UpGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.UpGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.UpImageColor2).For(v => v.color).To(vm => vm.UpImageColor2).OneWay();
            bindingSet.Bind(view.UpRectTransformLocalPosition2).For(v => v.localPosition).To(vm => vm.UpRectTransformLocalPosition2).OneWay();
            bindingSet.Bind(view.DownGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.DownGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.DownImageColor2).For(v => v.color).To(vm => vm.DownImageColor2).OneWay();
            bindingSet.Bind(view.DownRectTransformLocalPosition2).For(v => v.localPosition).To(vm => vm.DownRectTransformLocalPosition2).OneWay();
            bindingSet.Bind(view.LeftGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.LeftGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.LeftImageColor2).For(v => v.color).To(vm => vm.LeftImageColor2).OneWay();
            bindingSet.Bind(view.LeftRectTransformLocalPosition2).For(v => v.localPosition).To(vm => vm.LeftRectTransformLocalPosition2).OneWay();
            bindingSet.Bind(view.RightGameObjectActiveSelf2).For(v => v.activeSelf).To(vm => vm.RightGameObjectActiveSelf2).OneWay();
            bindingSet.Bind(view.RightImageColor2).For(v => v.color).To(vm => vm.RightImageColor2).OneWay();
            bindingSet.Bind(view.RightRectTransformLocalPosition2).For(v => v.localPosition).To(vm => vm.RightRectTransformLocalPosition2).OneWay();
            bindingSet.Bind(view.TypeGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.TypeGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.CenterGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.CenterGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.CenterImageColor3).For(v => v.color).To(vm => vm.CenterImageColor3).OneWay();
            bindingSet.Bind(view.CycleGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.CycleGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.CycleImageColor3).For(v => v.color).To(vm => vm.CycleImageColor3).OneWay();
            bindingSet.Bind(view.CycleRectTransformLocalScale3).For(v => v.localScale).To(vm => vm.CycleRectTransformLocalScale3).OneWay();
            bindingSet.Bind(view.UpGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.UpGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.UpImageColor3).For(v => v.color).To(vm => vm.UpImageColor3).OneWay();
            bindingSet.Bind(view.UpRectTransformLocalPosition3).For(v => v.localPosition).To(vm => vm.UpRectTransformLocalPosition3).OneWay();
            bindingSet.Bind(view.DownGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.DownGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.DownImageColor3).For(v => v.color).To(vm => vm.DownImageColor3).OneWay();
            bindingSet.Bind(view.DownRectTransformLocalPosition3).For(v => v.localPosition).To(vm => vm.DownRectTransformLocalPosition3).OneWay();
            bindingSet.Bind(view.LeftGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.LeftGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.LeftImageColor3).For(v => v.color).To(vm => vm.LeftImageColor3).OneWay();
            bindingSet.Bind(view.LeftRectTransformLocalPosition3).For(v => v.localPosition).To(vm => vm.LeftRectTransformLocalPosition3).OneWay();
            bindingSet.Bind(view.RightGameObjectActiveSelf3).For(v => v.activeSelf).To(vm => vm.RightGameObjectActiveSelf3).OneWay();
            bindingSet.Bind(view.RightImageColor3).For(v => v.color).To(vm => vm.RightImageColor3).OneWay();
            bindingSet.Bind(view.RightRectTransformLocalPosition3).For(v => v.localPosition).To(vm => vm.RightRectTransformLocalPosition3).OneWay();
            bindingSet.Bind(view.TypeGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.TypeGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.CenterGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.CenterGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.CenterImageColor4).For(v => v.color).To(vm => vm.CenterImageColor4).OneWay();
            bindingSet.Bind(view.CycleGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.CycleGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.CycleImageColor4).For(v => v.color).To(vm => vm.CycleImageColor4).OneWay();
            bindingSet.Bind(view.CycleRectTransformLocalScale4).For(v => v.localScale).To(vm => vm.CycleRectTransformLocalScale4).OneWay();
            bindingSet.Bind(view.UpGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.UpGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.UpImageColor4).For(v => v.color).To(vm => vm.UpImageColor4).OneWay();
            bindingSet.Bind(view.UpRectTransformLocalPosition4).For(v => v.localPosition).To(vm => vm.UpRectTransformLocalPosition4).OneWay();
            bindingSet.Bind(view.DownGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.DownGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.DownImageColor4).For(v => v.color).To(vm => vm.DownImageColor4).OneWay();
            bindingSet.Bind(view.DownRectTransformLocalPosition4).For(v => v.localPosition).To(vm => vm.DownRectTransformLocalPosition4).OneWay();
            bindingSet.Bind(view.LeftGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.LeftGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.LeftImageColor4).For(v => v.color).To(vm => vm.LeftImageColor4).OneWay();
            bindingSet.Bind(view.LeftRectTransformLocalPosition4).For(v => v.localPosition).To(vm => vm.LeftRectTransformLocalPosition4).OneWay();
            bindingSet.Bind(view.RightGameObjectActiveSelf4).For(v => v.activeSelf).To(vm => vm.RightGameObjectActiveSelf4).OneWay();
            bindingSet.Bind(view.RightImageColor4).For(v => v.color).To(vm => vm.RightImageColor4).OneWay();
            bindingSet.Bind(view.RightRectTransformLocalPosition4).For(v => v.localPosition).To(vm => vm.RightRectTransformLocalPosition4).OneWay();
            bindingSet.Bind(view.TypeGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.TypeGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.CenterGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.CenterGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.CenterImageColor5).For(v => v.color).To(vm => vm.CenterImageColor5).OneWay();
            bindingSet.Bind(view.CycleGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.CycleGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.CycleImageColor5).For(v => v.color).To(vm => vm.CycleImageColor5).OneWay();
            bindingSet.Bind(view.CycleRectTransformLocalScale5).For(v => v.localScale).To(vm => vm.CycleRectTransformLocalScale5).OneWay();
            bindingSet.Bind(view.UpGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.UpGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.UpImageColor5).For(v => v.color).To(vm => vm.UpImageColor5).OneWay();
            bindingSet.Bind(view.UpRectTransformLocalPosition5).For(v => v.localPosition).To(vm => vm.UpRectTransformLocalPosition5).OneWay();
            bindingSet.Bind(view.DownGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.DownGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.DownImageColor5).For(v => v.color).To(vm => vm.DownImageColor5).OneWay();
            bindingSet.Bind(view.DownRectTransformLocalPosition5).For(v => v.localPosition).To(vm => vm.DownRectTransformLocalPosition5).OneWay();
            bindingSet.Bind(view.LeftGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.LeftGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.LeftImageColor5).For(v => v.color).To(vm => vm.LeftImageColor5).OneWay();
            bindingSet.Bind(view.LeftRectTransformLocalPosition5).For(v => v.localPosition).To(vm => vm.LeftRectTransformLocalPosition5).OneWay();
            bindingSet.Bind(view.RightGameObjectActiveSelf5).For(v => v.activeSelf).To(vm => vm.RightGameObjectActiveSelf5).OneWay();
            bindingSet.Bind(view.RightImageColor5).For(v => v.color).To(vm => vm.RightImageColor5).OneWay();
            bindingSet.Bind(view.RightRectTransformLocalPosition5).For(v => v.localPosition).To(vm => vm.RightRectTransformLocalPosition5).OneWay();
            bindingSet.Bind(view.TypeGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.TypeGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.CenterGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.CenterGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.CenterImageColor6).For(v => v.color).To(vm => vm.CenterImageColor6).OneWay();
            bindingSet.Bind(view.CycleGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.CycleGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.CycleImageColor6).For(v => v.color).To(vm => vm.CycleImageColor6).OneWay();
            bindingSet.Bind(view.CycleRectTransformLocalScale6).For(v => v.localScale).To(vm => vm.CycleRectTransformLocalScale6).OneWay();
            bindingSet.Bind(view.UpGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.UpGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.UpImageColor6).For(v => v.color).To(vm => vm.UpImageColor6).OneWay();
            bindingSet.Bind(view.UpRectTransformLocalPosition6).For(v => v.localPosition).To(vm => vm.UpRectTransformLocalPosition6).OneWay();
            bindingSet.Bind(view.DownGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.DownGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.DownImageColor6).For(v => v.color).To(vm => vm.DownImageColor6).OneWay();
            bindingSet.Bind(view.DownRectTransformLocalPosition6).For(v => v.localPosition).To(vm => vm.DownRectTransformLocalPosition6).OneWay();
            bindingSet.Bind(view.LeftGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.LeftGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.LeftImageColor6).For(v => v.color).To(vm => vm.LeftImageColor6).OneWay();
            bindingSet.Bind(view.LeftRectTransformLocalPosition6).For(v => v.localPosition).To(vm => vm.LeftRectTransformLocalPosition6).OneWay();
            bindingSet.Bind(view.RightGameObjectActiveSelf6).For(v => v.activeSelf).To(vm => vm.RightGameObjectActiveSelf6).OneWay();
            bindingSet.Bind(view.RightImageColor6).For(v => v.color).To(vm => vm.RightImageColor6).OneWay();
            bindingSet.Bind(view.RightRectTransformLocalPosition6).For(v => v.localPosition).To(vm => vm.RightRectTransformLocalPosition6).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonCrossView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonCrossViewModel()
        {
            Type type = typeof(CommonCrossViewModel);
            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    PropertySetter.Add(property.Name, property);
                }
            }
			foreach (var methodInfo in type.GetMethods())
            {
                if (methodInfo.IsPublic)
                {
                    MethodSetter.Add(methodInfo.Name, methodInfo);
                }
            }
        }

		private void SpriteReset()
		{
		}

		public void Reset()
		{
			ShowGameObjectActiveSelf = _view.oriShowGameObjectActiveSelf;
			HitGameObjectActiveSelf = _view.oriHitGameObjectActiveSelf;
			LuRectTransformLocalPosition = _view.oriLuRectTransformLocalPosition;
			LdRectTransformLocalPosition = _view.oriLdRectTransformLocalPosition;
			RuRectTransformLocalPosition = _view.oriRuRectTransformLocalPosition;
			RdRectTransformLocalPosition = _view.oriRdRectTransformLocalPosition;
			AwpGameObjectActiveSelf = _view.oriAwpGameObjectActiveSelf;
			SpriteReset();
		}

		public void CallFunction(string functionName)
        {
            if (MethodSetter.ContainsKey(functionName))
            {
                MethodSetter[functionName].Invoke(this, null);
            }
        }

		public bool IsPropertyExist(string propertyId)
        {
            return PropertySetter.ContainsKey(propertyId);
        }

		public Transform GetParentLinkNode()
		{
			return null;
		}

		public Transform GetChildLinkNode()
		{
			return null;
		}

        public const string TypeGameObjectActiveSelf = "TypeGameObjectActiveSelf";
        public const int TypeGameObjectActiveSelfCount = 6;
        public const string CenterGameObjectActiveSelf = "CenterGameObjectActiveSelf";
        public const int CenterGameObjectActiveSelfCount = 6;
        public const string CenterImageColor = "CenterImageColor";
        public const int CenterImageColorCount = 6;
        public const string CycleGameObjectActiveSelf = "CycleGameObjectActiveSelf";
        public const int CycleGameObjectActiveSelfCount = 6;
        public const string CycleImageColor = "CycleImageColor";
        public const int CycleImageColorCount = 6;
        public const string CycleRectTransformLocalScale = "CycleRectTransformLocalScale";
        public const int CycleRectTransformLocalScaleCount = 6;
        public const string UpGameObjectActiveSelf = "UpGameObjectActiveSelf";
        public const int UpGameObjectActiveSelfCount = 6;
        public const string UpImageColor = "UpImageColor";
        public const int UpImageColorCount = 6;
        public const string UpRectTransformLocalPosition = "UpRectTransformLocalPosition";
        public const int UpRectTransformLocalPositionCount = 6;
        public const string DownGameObjectActiveSelf = "DownGameObjectActiveSelf";
        public const int DownGameObjectActiveSelfCount = 6;
        public const string DownImageColor = "DownImageColor";
        public const int DownImageColorCount = 6;
        public const string DownRectTransformLocalPosition = "DownRectTransformLocalPosition";
        public const int DownRectTransformLocalPositionCount = 6;
        public const string LeftGameObjectActiveSelf = "LeftGameObjectActiveSelf";
        public const int LeftGameObjectActiveSelfCount = 6;
        public const string LeftImageColor = "LeftImageColor";
        public const int LeftImageColorCount = 6;
        public const string LeftRectTransformLocalPosition = "LeftRectTransformLocalPosition";
        public const int LeftRectTransformLocalPositionCount = 6;
        public const string RightGameObjectActiveSelf = "RightGameObjectActiveSelf";
        public const int RightGameObjectActiveSelfCount = 6;
        public const string RightImageColor = "RightImageColor";
        public const int RightImageColorCount = 6;
        public const string RightRectTransformLocalPosition = "RightRectTransformLocalPosition";
        public const int RightRectTransformLocalPositionCount = 6;
        public bool SetTypeGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		TypeGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		TypeGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		TypeGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		TypeGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		TypeGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		TypeGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetTypeGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return TypeGameObjectActiveSelf1;
        	case 2:
        		return TypeGameObjectActiveSelf2;
        	case 3:
        		return TypeGameObjectActiveSelf3;
        	case 4:
        		return TypeGameObjectActiveSelf4;
        	case 5:
        		return TypeGameObjectActiveSelf5;
        	case 6:
        		return TypeGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetCenterGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		CenterGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		CenterGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		CenterGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		CenterGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		CenterGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		CenterGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetCenterGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return CenterGameObjectActiveSelf1;
        	case 2:
        		return CenterGameObjectActiveSelf2;
        	case 3:
        		return CenterGameObjectActiveSelf3;
        	case 4:
        		return CenterGameObjectActiveSelf4;
        	case 5:
        		return CenterGameObjectActiveSelf5;
        	case 6:
        		return CenterGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetCenterImageColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		CenterImageColor1 = val;
        		break;
        	case 2:
        		CenterImageColor2 = val;
        		break;
        	case 3:
        		CenterImageColor3 = val;
        		break;
        	case 4:
        		CenterImageColor4 = val;
        		break;
        	case 5:
        		CenterImageColor5 = val;
        		break;
        	case 6:
        		CenterImageColor6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetCenterImageColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return CenterImageColor1;
        	case 2:
        		return CenterImageColor2;
        	case 3:
        		return CenterImageColor3;
        	case 4:
        		return CenterImageColor4;
        	case 5:
        		return CenterImageColor5;
        	case 6:
        		return CenterImageColor6;
        	default:
        		return default(Color);
        	}
        }
        public bool SetCycleGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		CycleGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		CycleGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		CycleGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		CycleGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		CycleGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		CycleGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetCycleGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return CycleGameObjectActiveSelf1;
        	case 2:
        		return CycleGameObjectActiveSelf2;
        	case 3:
        		return CycleGameObjectActiveSelf3;
        	case 4:
        		return CycleGameObjectActiveSelf4;
        	case 5:
        		return CycleGameObjectActiveSelf5;
        	case 6:
        		return CycleGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetCycleImageColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		CycleImageColor1 = val;
        		break;
        	case 2:
        		CycleImageColor2 = val;
        		break;
        	case 3:
        		CycleImageColor3 = val;
        		break;
        	case 4:
        		CycleImageColor4 = val;
        		break;
        	case 5:
        		CycleImageColor5 = val;
        		break;
        	case 6:
        		CycleImageColor6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetCycleImageColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return CycleImageColor1;
        	case 2:
        		return CycleImageColor2;
        	case 3:
        		return CycleImageColor3;
        	case 4:
        		return CycleImageColor4;
        	case 5:
        		return CycleImageColor5;
        	case 6:
        		return CycleImageColor6;
        	default:
        		return default(Color);
        	}
        }
        public bool SetCycleRectTransformLocalScale (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		CycleRectTransformLocalScale1 = val;
        		break;
        	case 2:
        		CycleRectTransformLocalScale2 = val;
        		break;
        	case 3:
        		CycleRectTransformLocalScale3 = val;
        		break;
        	case 4:
        		CycleRectTransformLocalScale4 = val;
        		break;
        	case 5:
        		CycleRectTransformLocalScale5 = val;
        		break;
        	case 6:
        		CycleRectTransformLocalScale6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetCycleRectTransformLocalScale (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return CycleRectTransformLocalScale1;
        	case 2:
        		return CycleRectTransformLocalScale2;
        	case 3:
        		return CycleRectTransformLocalScale3;
        	case 4:
        		return CycleRectTransformLocalScale4;
        	case 5:
        		return CycleRectTransformLocalScale5;
        	case 6:
        		return CycleRectTransformLocalScale6;
        	default:
        		return default(Vector3);
        	}
        }
        public bool SetUpGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		UpGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		UpGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		UpGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		UpGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		UpGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		UpGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetUpGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpGameObjectActiveSelf1;
        	case 2:
        		return UpGameObjectActiveSelf2;
        	case 3:
        		return UpGameObjectActiveSelf3;
        	case 4:
        		return UpGameObjectActiveSelf4;
        	case 5:
        		return UpGameObjectActiveSelf5;
        	case 6:
        		return UpGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetUpImageColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		UpImageColor1 = val;
        		break;
        	case 2:
        		UpImageColor2 = val;
        		break;
        	case 3:
        		UpImageColor3 = val;
        		break;
        	case 4:
        		UpImageColor4 = val;
        		break;
        	case 5:
        		UpImageColor5 = val;
        		break;
        	case 6:
        		UpImageColor6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetUpImageColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpImageColor1;
        	case 2:
        		return UpImageColor2;
        	case 3:
        		return UpImageColor3;
        	case 4:
        		return UpImageColor4;
        	case 5:
        		return UpImageColor5;
        	case 6:
        		return UpImageColor6;
        	default:
        		return default(Color);
        	}
        }
        public bool SetUpRectTransformLocalPosition (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		UpRectTransformLocalPosition1 = val;
        		break;
        	case 2:
        		UpRectTransformLocalPosition2 = val;
        		break;
        	case 3:
        		UpRectTransformLocalPosition3 = val;
        		break;
        	case 4:
        		UpRectTransformLocalPosition4 = val;
        		break;
        	case 5:
        		UpRectTransformLocalPosition5 = val;
        		break;
        	case 6:
        		UpRectTransformLocalPosition6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetUpRectTransformLocalPosition (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpRectTransformLocalPosition1;
        	case 2:
        		return UpRectTransformLocalPosition2;
        	case 3:
        		return UpRectTransformLocalPosition3;
        	case 4:
        		return UpRectTransformLocalPosition4;
        	case 5:
        		return UpRectTransformLocalPosition5;
        	case 6:
        		return UpRectTransformLocalPosition6;
        	default:
        		return default(Vector3);
        	}
        }
        public bool SetDownGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		DownGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		DownGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		DownGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		DownGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		DownGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		DownGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetDownGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return DownGameObjectActiveSelf1;
        	case 2:
        		return DownGameObjectActiveSelf2;
        	case 3:
        		return DownGameObjectActiveSelf3;
        	case 4:
        		return DownGameObjectActiveSelf4;
        	case 5:
        		return DownGameObjectActiveSelf5;
        	case 6:
        		return DownGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetDownImageColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		DownImageColor1 = val;
        		break;
        	case 2:
        		DownImageColor2 = val;
        		break;
        	case 3:
        		DownImageColor3 = val;
        		break;
        	case 4:
        		DownImageColor4 = val;
        		break;
        	case 5:
        		DownImageColor5 = val;
        		break;
        	case 6:
        		DownImageColor6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetDownImageColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return DownImageColor1;
        	case 2:
        		return DownImageColor2;
        	case 3:
        		return DownImageColor3;
        	case 4:
        		return DownImageColor4;
        	case 5:
        		return DownImageColor5;
        	case 6:
        		return DownImageColor6;
        	default:
        		return default(Color);
        	}
        }
        public bool SetDownRectTransformLocalPosition (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		DownRectTransformLocalPosition1 = val;
        		break;
        	case 2:
        		DownRectTransformLocalPosition2 = val;
        		break;
        	case 3:
        		DownRectTransformLocalPosition3 = val;
        		break;
        	case 4:
        		DownRectTransformLocalPosition4 = val;
        		break;
        	case 5:
        		DownRectTransformLocalPosition5 = val;
        		break;
        	case 6:
        		DownRectTransformLocalPosition6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetDownRectTransformLocalPosition (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return DownRectTransformLocalPosition1;
        	case 2:
        		return DownRectTransformLocalPosition2;
        	case 3:
        		return DownRectTransformLocalPosition3;
        	case 4:
        		return DownRectTransformLocalPosition4;
        	case 5:
        		return DownRectTransformLocalPosition5;
        	case 6:
        		return DownRectTransformLocalPosition6;
        	default:
        		return default(Vector3);
        	}
        }
        public bool SetLeftGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		LeftGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		LeftGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		LeftGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		LeftGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		LeftGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		LeftGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetLeftGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LeftGameObjectActiveSelf1;
        	case 2:
        		return LeftGameObjectActiveSelf2;
        	case 3:
        		return LeftGameObjectActiveSelf3;
        	case 4:
        		return LeftGameObjectActiveSelf4;
        	case 5:
        		return LeftGameObjectActiveSelf5;
        	case 6:
        		return LeftGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetLeftImageColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		LeftImageColor1 = val;
        		break;
        	case 2:
        		LeftImageColor2 = val;
        		break;
        	case 3:
        		LeftImageColor3 = val;
        		break;
        	case 4:
        		LeftImageColor4 = val;
        		break;
        	case 5:
        		LeftImageColor5 = val;
        		break;
        	case 6:
        		LeftImageColor6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetLeftImageColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LeftImageColor1;
        	case 2:
        		return LeftImageColor2;
        	case 3:
        		return LeftImageColor3;
        	case 4:
        		return LeftImageColor4;
        	case 5:
        		return LeftImageColor5;
        	case 6:
        		return LeftImageColor6;
        	default:
        		return default(Color);
        	}
        }
        public bool SetLeftRectTransformLocalPosition (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		LeftRectTransformLocalPosition1 = val;
        		break;
        	case 2:
        		LeftRectTransformLocalPosition2 = val;
        		break;
        	case 3:
        		LeftRectTransformLocalPosition3 = val;
        		break;
        	case 4:
        		LeftRectTransformLocalPosition4 = val;
        		break;
        	case 5:
        		LeftRectTransformLocalPosition5 = val;
        		break;
        	case 6:
        		LeftRectTransformLocalPosition6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetLeftRectTransformLocalPosition (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LeftRectTransformLocalPosition1;
        	case 2:
        		return LeftRectTransformLocalPosition2;
        	case 3:
        		return LeftRectTransformLocalPosition3;
        	case 4:
        		return LeftRectTransformLocalPosition4;
        	case 5:
        		return LeftRectTransformLocalPosition5;
        	case 6:
        		return LeftRectTransformLocalPosition6;
        	default:
        		return default(Vector3);
        	}
        }
        public bool SetRightGameObjectActiveSelf (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		RightGameObjectActiveSelf1 = val;
        		break;
        	case 2:
        		RightGameObjectActiveSelf2 = val;
        		break;
        	case 3:
        		RightGameObjectActiveSelf3 = val;
        		break;
        	case 4:
        		RightGameObjectActiveSelf4 = val;
        		break;
        	case 5:
        		RightGameObjectActiveSelf5 = val;
        		break;
        	case 6:
        		RightGameObjectActiveSelf6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetRightGameObjectActiveSelf (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return RightGameObjectActiveSelf1;
        	case 2:
        		return RightGameObjectActiveSelf2;
        	case 3:
        		return RightGameObjectActiveSelf3;
        	case 4:
        		return RightGameObjectActiveSelf4;
        	case 5:
        		return RightGameObjectActiveSelf5;
        	case 6:
        		return RightGameObjectActiveSelf6;
        	default:
        		return default(bool);
        	}
        }
        public bool SetRightImageColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		RightImageColor1 = val;
        		break;
        	case 2:
        		RightImageColor2 = val;
        		break;
        	case 3:
        		RightImageColor3 = val;
        		break;
        	case 4:
        		RightImageColor4 = val;
        		break;
        	case 5:
        		RightImageColor5 = val;
        		break;
        	case 6:
        		RightImageColor6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetRightImageColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return RightImageColor1;
        	case 2:
        		return RightImageColor2;
        	case 3:
        		return RightImageColor3;
        	case 4:
        		return RightImageColor4;
        	case 5:
        		return RightImageColor5;
        	case 6:
        		return RightImageColor6;
        	default:
        		return default(Color);
        	}
        }
        public bool SetRightRectTransformLocalPosition (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		RightRectTransformLocalPosition1 = val;
        		break;
        	case 2:
        		RightRectTransformLocalPosition2 = val;
        		break;
        	case 3:
        		RightRectTransformLocalPosition3 = val;
        		break;
        	case 4:
        		RightRectTransformLocalPosition4 = val;
        		break;
        	case 5:
        		RightRectTransformLocalPosition5 = val;
        		break;
        	case 6:
        		RightRectTransformLocalPosition6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetRightRectTransformLocalPosition (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return RightRectTransformLocalPosition1;
        	case 2:
        		return RightRectTransformLocalPosition2;
        	case 3:
        		return RightRectTransformLocalPosition3;
        	case 4:
        		return RightRectTransformLocalPosition4;
        	case 5:
        		return RightRectTransformLocalPosition5;
        	case 6:
        		return RightRectTransformLocalPosition6;
        	default:
        		return default(Vector3);
        	}
        }
        public string ResourceBundleName { get { return "uiprefabs/common"; } }
        public string ResourceAssetName { get { return "Cross"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
        public string UiConfigAssetName { get { return "CommonCross" ; } }
    }
}
