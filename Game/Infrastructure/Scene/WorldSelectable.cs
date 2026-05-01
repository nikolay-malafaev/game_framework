using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFramework.Infrastructure
{
    public class WorldSelectable : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        public UnityEvent OnSelectEvent;
        public UnityEvent OnDeselectEvent;
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            OnSelectEvent?.Invoke();
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            OnDeselectEvent?.Invoke();
        }
    }
}