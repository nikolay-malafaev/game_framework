using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.Infrastructure
{
    public class EventSystemProxyBehaviour : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler,
        IScrollHandler,
        ISelectHandler, IDeselectHandler, IUpdateSelectedHandler,
        IMoveHandler, ISubmitHandler, ICancelHandler
    {
        [SerializeField]
        private GameObject _targetObject;

        public void OnPointerEnter(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.pointerEnterHandler);

        public void OnPointerExit(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.pointerExitHandler);

        public void OnPointerDown(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.pointerDownHandler);

        public void OnPointerUp(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.pointerUpHandler);

        public void OnPointerClick(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.pointerClickHandler);

        public void OnBeginDrag(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.beginDragHandler);

        public void OnDrag(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.dragHandler);

        public void OnDrop(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.dropHandler);

        public void OnEndDrag(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.endDragHandler);

        public void OnScroll(PointerEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.scrollHandler);

        public void OnSelect(BaseEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.selectHandler);

        public void OnDeselect(BaseEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.deselectHandler);

        public void OnUpdateSelected(BaseEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.updateSelectedHandler);

        public void OnMove(AxisEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.moveHandler);

        public void OnSubmit(BaseEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.submitHandler);

        public void OnCancel(BaseEventData eventData) =>
            ExecuteEvents.Execute(_targetObject, eventData, ExecuteEvents.cancelHandler);
    }
}