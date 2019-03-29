using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Ui;

namespace Assets.App.Client.GameModules.Ui
{
    public class UiSessionSystem : IGamePlaySystem
    {
        private Contexts _contexts;

        private Dictionary<string, IUiAdapter> _uiAdapters;
        private Dictionary<string, bool> _uiState;
        private Dictionary<UiGroup, List<IUiGroupController>> _uiGroup;
        private List<UiGroup> _hideGroup;
        private List<UiGroup> _holdHideGroup;

        private IUiGroupController _lastShowUi;

        public UiSessionSystem(Contexts contexts)
        {
            _contexts = contexts;
            _uiAdapters = _contexts.ui.uISession.UiAdapters;
            _hideGroup = _contexts.ui.uISession.HideGroup;
            _uiState = _contexts.ui.uISession.UiState;
            _uiGroup = _contexts.ui.uISession.UiGroup;

            _holdHideGroup = new List<UiGroup>();
        }


        public void OnGamePlay()
        {
            UpdateUiState();
            UpdateHideUiGroup();
            UpdateSingletonUiGroup();
        }

        private void UpdateSingletonUiGroup()
        {
            List<IUiGroupController> modelList;
            _uiGroup.TryGetValue(UiGroup.Singleton, out modelList);
            if (modelList == null) return;
            foreach (var model in modelList)
            {
                if (model.Enable)
                {
                    if (_lastShowUi == null)
                    {
                        _lastShowUi = model;
                        break;
                    }
                    if (!model.Equals(_lastShowUi))
                    {
                        _lastShowUi.Enable = false;
                        _lastShowUi = model;
                        break;
                    }
                }
            }
        }

        private void UpdateHideUiGroup()
        {
            if (_hideGroup == null || (_hideGroup.Count == 0 && _holdHideGroup.Count == 0)) return;

            bool holdGroupIsDirty = false;

            for (int i = 0; i < _holdHideGroup.Count; i++)
            {
                if (_hideGroup.IndexOf(_holdHideGroup[i]) == -1)
                {
                    holdGroupIsDirty = true;
                    SetUiGroup(_holdHideGroup[i], true);
                    _holdHideGroup.RemoveAt(i);
                    i--;
                }
            }

            foreach (var group in _hideGroup)
            {
                if (_holdHideGroup.IndexOf(group) == -1 || holdGroupIsDirty)
                {
                    SetUiGroup(group, false);
                    _holdHideGroup.Add(group);
                }
            }
        }

        private void SetUiGroup(UiGroup group, bool isShow)
        {
            if (_uiGroup.ContainsKey(group) == false) return;
            var modelList = _uiGroup[group];
            foreach (var model in modelList)
            {
                model.SetUiState(isShow);
            }
        }

        private void UpdateUiState()
        {
            if (_uiState == null || _uiState.Count == 0) return;
           
            foreach (var pair in _uiState)
            {
                if (_uiAdapters.ContainsKey(pair.Key))
                {
                    _uiAdapters[pair.Key].Enable = pair.Value;
                }
            }
            _uiState.Clear();
        }
    }
}