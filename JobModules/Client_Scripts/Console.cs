using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wenzil.Console;

namespace Shared.Scripts
{
    public class Console : MonoBehaviour
    {
        public GameObject console;
        public GameObject scrollbar;
        public GameObject outputText;
        public GameObject outputArea;
        public GameObject inputArea;

        // Use this for initialization
        
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            var consoleObj = GameObject.Find("ConsoleUI/Console");
            if (consoleObj != null && consoleObj.GetComponent<ConsoleUI>() != null)
            {
                Destroy(gameObject);
                return;
            }

            if (console != null)
            {
                console.SetActive(false);
                var consoleUI = console.AddComponent<ConsoleUI>();
                var consoleController = console.GetComponent<ConsoleController>();
                consoleController.ui = consoleUI;
                console.AddComponent<DefaultCommands>();
                consoleUI.enabled = false;
                consoleUI.scrollbar = scrollbar.GetComponent<Scrollbar>();
                consoleUI.outputText = outputText.GetComponent<Text>();
                consoleUI.outputArea = outputArea.GetComponent<ScrollRect>();
                consoleUI.inputField = inputArea.GetComponent<InputField>();
               
                var submitEvent = new InputField.SubmitEvent();
                submitEvent.AddListener(consoleUI.OnSubmit);
                consoleUI.inputField.onEndEdit = submitEvent;
                console.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

