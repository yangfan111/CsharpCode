using UnityEngine;

namespace App.Client.GameModules.Ui.Utils
{
    public class ActiveSetter
    {
        private GameObject _gameObject;
        private bool _active;
        public ActiveSetter(GameObject gameObject)
        {
            _gameObject = gameObject;
            _active = _gameObject.activeSelf;
        }

        public bool Active
        {
            set {
                if (_active == value) return;
                _active = value;
                _gameObject.SetActive(_active);
            }
            get { return _active; }
        }
    }
}