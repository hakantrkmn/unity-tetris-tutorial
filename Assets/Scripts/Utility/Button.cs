using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using TMPro;

namespace Utility
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class Button : MonoBehaviour
    {
        [OnValueChanged("UpdateName")]
        public string buttonName;

        [OnValueChanged("UpdateText")]
        public string buttonText;

        [ValueDropdown("GetButtonTypes")]
        public ButtonType buttonType;

        public bool triggerEvent = true;

        [Title("Event Trigger")]
        [InfoBox("Tıklandığında tetiklenecek olan statik ve parametresiz bir UIEventManager olayını seçin.")]
        [ValueDropdown("GetUIEventActions")]
        [ShowIf("triggerEvent")]
        public string eventToTrigger;

        private UnityEngine.UI.Button _buttonComponent;

        public TextMeshProUGUI buttonTextTMP;

        private IEnumerable<string> GetUIEventActions()
        {
            // Reflection kullanarak UIEventManager içindeki tüm public static Action türündeki alanları bulur.
            return typeof(UIEventManager)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(Action))
                .Select(field => field.Name)
                .ToList();
        }

        private IEnumerable<ButtonType> GetButtonTypes()
        {
            return Enum.GetValues(typeof(ButtonType)).Cast<ButtonType>();
        }

        private void Awake()
        {
            _buttonComponent = GetComponent<UnityEngine.UI.Button>();
            _buttonComponent.onClick.AddListener(TriggerEvent);
            buttonTextTMP = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnDestroy()
        {
            if (_buttonComponent != null)
            {
                _buttonComponent.onClick.RemoveListener(TriggerEvent);
            }
        }

        private void TriggerEvent()
        {
            if (!triggerEvent) return;
            if (string.IsNullOrEmpty(eventToTrigger))
            {
                Debug.LogWarning($"Buton '{name}' için bir olay atanmamış.", this);
                return;
            }

            // Reflection kullanarak UIEventManager'daki statik event alanını bul
            FieldInfo eventField = typeof(UIEventManager).GetField(eventToTrigger, BindingFlags.Public | BindingFlags.Static);

            if (eventField != null)
            {
                // Alandan Action delegesini al
                var eventAction = eventField.GetValue(null) as Action;

                // Olayı tetikle
                eventAction?.Invoke();
            }
            else
            {
                Debug.LogError($"Olay '{eventToTrigger}' UIEventManager içinde bulunamadı.", this);
            }
        }

        private void OnValidate()
        {
            buttonTextTMP = GetComponentInChildren<TextMeshProUGUI>();
            UpdateName();
            UpdateText();
        }

        private void UpdateName()
        {
            if (!string.IsNullOrEmpty(buttonName))
            {
                gameObject.name = buttonName;
            }
        }

        private void UpdateText()
        {
            // Önce TextMeshProUGUI bileşenini bulmaya çalış
            if (buttonTextTMP != null)
            {
                buttonTextTMP.text = buttonText;
                return;
            }
        }

        private void UpdateButtonText(ButtonType buttonType)
        {
            if (buttonType == this.buttonType)
            {
                buttonTextTMP.text = "Reroll: " + GameManager.Instance.gameSession.rerollValue.ToString();
            }
        }
        private void OnEnable()
        {
            UIEventManager.UpdateButtonText += UpdateButtonText;
        }

        private void OnDisable()
        {
            UIEventManager.UpdateButtonText -= UpdateButtonText;
        }
    }
}
