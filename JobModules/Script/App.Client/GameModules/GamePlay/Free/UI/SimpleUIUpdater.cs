using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class SimpleUIUpdater : DisposableSingleton<SimpleUIUpdater>
    {
        private IList<IUIUpdater> _updaters = new List<IUIUpdater>();

        public void Add(IUIUpdater updater)
        {
            _updaters.Add(updater);
        }

        public void Remove(IUIUpdater updater)
        {
            _updaters.Remove(updater);
        }

        public void RemoveAll()
        {
            _updaters.Clear();
        }

        protected override void OnDispose()
        {
            RemoveAll();
        }

        public void Update()
        {
            for (var index = 0; index < _updaters.Count; index++)
            {
                var updater = _updaters[index];
                if (!updater.IsDisabled)
                    updater.UIUpdate((int)(Time.deltaTime * 1000));
            }
        }
    }
}