using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.GameModules.UserInput
{
    public class InputManager : IUserInputManager
    {
        private IInputProcessor _processor;
        private bool _configSet;
        private bool _enabled = true;

        public InputManager(Contexts contexts = null)
        {
            _processor = new InputProcessor();
            _processor.SetConverter(new KeyConverter());
            _processor.SetKeyDispatcher(new KeyDispatcher());
            _processor.SetPointerDispatcher(new PointerDispatch());
            _processor.SetKeyProvider(new KeyProvider());
            _processor.SetPointerProvider(new PointerProvider(contexts));
            _processor.SetKeyDataPool(new DataPool<KeyData>()); 
            _processor.SetPointerDataPool(new DataPool<PointerData>());
        }

        public bool SetConfig(InputConfig cfg)
        {
            if (_configSet)
            {
                return false;
            }

            if (null == cfg)
            {
                return false;
            }
            _processor.SetConfig(cfg);
            return _configSet;
        }

        public void SetProcessor(IInputProcessor processor)
        {
            _processor = processor;
        }

        public void RegisterPointerReceiver(IPointerReceiver receiver)
        {
            _processor.AddPointerReceiver(receiver);
        }

        public void RegisterKeyReceiver(IKeyReceiver receiver)
        {
            _processor.AddKeyReceiver(receiver);
        }

        public void UnregisterPointerReceiver(IPointerReceiver receiver)
        {
            _processor.RemovePointerReceiver(receiver);
        }

        public void UnregisterKeyReceiver(IKeyReceiver receiver)
        {
            _processor.RemoveKeyReceiver(receiver);
        }

        public void InsertKey(KeyData key)
        {
            _processor.InsertKey(key);
        }

        public void Dispatch()
        {
            if(_enabled)
                _processor.Dispatch();
        }

        public void SetEnable(bool isEnable)
        {
            _enabled = isEnable;
        }

        public Dictionary<UserInputKey, List<KeyConvertItem>> GetInputDic()
        {
            return _processor.GetInputDic();
        }
    }
}