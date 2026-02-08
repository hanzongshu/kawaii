using UnityEngine;

namespace 移动操作手柄
{
    public class MobileJoystick : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] RectTransform _joystickOutline;
        [SerializeField] RectTransform _joystickKnob;
        [SerializeField] Canvas _Canves;

        [Header("Settings")]
        [SerializeField] float _moveFactor;

        Vector3 _clickedPositon;
        [SerializeField, ReadOnly]  Vector3 move;

        private bool _canControl;
        private void Start()
        {
            HideJoystick();
        }

        private void Update()
        {
            if (_canControl)
                ControlJoystick();
        }

        /// <summary>
        /// 点击操作杆区域的回调函数
        /// </summary>
        public void ClickedOnJoystickZoneCallback()
     {
            Debug.Log("点击了操作杆区域");
             _clickedPositon = Input.mousePosition;
            _joystickOutline.position = _clickedPositon;
            ShowJoystick();
             
     }
        /// <summary>
        /// 激活状态
        /// </summary>
       private void ShowJoystick()
        {
            _joystickOutline.gameObject.SetActive(true);
            _canControl = true;
        }
        /// <summary>
        /// 非激活状态
        /// </summary>
        private void HideJoystick()
        {
            _joystickOutline.gameObject.SetActive(false);
            _canControl = false;
        }


        private void ControlJoystick()
        {
            Vector3 currentPosition = Input.mousePosition;
            Vector3 direction = currentPosition - _clickedPositon;
            //拿到屏幕系数
            float canvasScale = _Canves.GetComponent<RectTransform>().localScale.x;
            ////拿到位移长度
            float moveMagnitude = direction.magnitude*_moveFactor*canvasScale;

            //拿到UI轮廓的宽度，基于UI坐标不是基于像素的
            float absoluteWidth = _joystickOutline.rect.width / 2;
            //用宽度乘于屏幕缩放系数
            float realWidth = absoluteWidth * canvasScale;
            //约束最大位置为UI轮廓宽的一半
            moveMagnitude = Mathf.Min(realWidth, moveMagnitude);

            move = direction.normalized;

            Vector3 knobMove = move * moveMagnitude;

            Vector3 targetPoistion = _clickedPositon + knobMove;

            _joystickKnob.position = targetPoistion;

            if (Input.GetMouseButtonUp(0))
                HideJoystick();
        }

       


    }
}
