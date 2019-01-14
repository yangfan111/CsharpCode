using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Components;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Common
{
    public class EntityReference : MonoBehaviour
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(EntityReference));

        private EntityAdapterComponent _entityAdapter = null;

        private EntityKey _entityKey = EntityKey.Default;

        public void Init(EntityAdapterComponent entityAdapter)
        {
            _entityAdapter = entityAdapter;
            _entityKey = entityAdapter.SelfAdapter.EntityKey;
        }

        public EntityKey EntityKey
        {
            get { return _entityKey; }
        }

        public Entitas.Entity Reference
        {
            get
            {
                if (_entityAdapter != null)
                {
                    var entityKey = _entityAdapter.SelfAdapter.EntityKey;
                    if (entityKey == _entityKey)
                    {
                        return _entityAdapter.SelfAdapter.RealEntity as Entitas.Entity;
                    }
                    _logger.ErrorFormat("The entity {0} is recycled.", entityKey);
                }

                return null;
            }
        }


    }
}
