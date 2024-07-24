using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Control
{
    public class PlayerController : MonoBehaviour
    {
        public bool isControlable = true;
        [SerializeField] Animator bodyAnimator;
        [SerializeField] Animator gunAnimator;
        [Header("Look")]
        [SerializeField] GameObject firstPersonCameraObject;
        Vector2 lookInputValue;
        float yRotation = 0f;
        float xRotation = 0f;
        // 플레이어 시점에서 x축과 y축의 감도
        // 에디터 상의 카메라 기준 x축, y축이 아님
        // 즉, x축감도는 가로 회전(y축 회전)의 감도, y축 감도는 세로 회전(x축 회전)의 감도임
        public float playerXAxisSensitivity = 1f;
        public float playerYAxisSensitivity = 1f;

        [Header("Move")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float runningSpeedModifier = 1.5f;
        Vector2 moveInputValue;
        float isMoving;

        [Header("Jump")]
        [SerializeField] float jumpPower = 5f;
        [SerializeField] int maxJumpCount = 2;
        [SerializeField] public int currentJumpCount { get; private set; }
        [SerializeField] LayerMask groundLayer;

        [Header("UI")]
        [SerializeField] GameObject infoTab;

        private void OnCollisionEnter(Collision other)
        {
            ContactPoint contact = other.GetContact(0);
            Vector3 pos = contact.point;

            // 플레이어와의 충돌이 일어난 것은 무시
            if (other.collider.tag == "Player") return;

            // 캐릭터가 몬스터와 충돌이 일어났을 때
            if (other.collider.tag == "Enemy")
            {
                // 충돌이 일어난 지점의 y좌표가 캐릭터의 y좌표보다 위에 있을 때
                // 즉, 발로 밟지 않았고 그냥 부딪혔을 때는 무시
                if (pos.y > transform.position.y) return;
                // 발로 밟았을 때 추가 예정
            }

            // 점프 횟수 초기화
            currentJumpCount = maxJumpCount;
        }

        private void Start()
        {
            currentJumpCount = maxJumpCount;
        }

        void OnMove(InputValue value)
        {
            if (!isControlable) return;

            moveInputValue = value.Get<Vector2>();

            isMoving = moveInputValue.magnitude;
        }

        void OnLook(InputValue value)
        {
            if (!isControlable) return;

            lookInputValue = value.Get<Vector2>();
        }

        void OnJump()
        {
            if (!isControlable) return;

            Rigidbody rigidbody = GetComponent<Rigidbody>();

            if (currentJumpCount > 0)
            {
                currentJumpCount--;
                rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }

        void OnRun(InputValue value)
        {
            if (!isControlable) return;

            // 누르기 시작할 떄 속도를 달리기 속도로
            if (value.Get<float>() == 1)
                moveSpeed *= runningSpeedModifier;
            // 뗄 때 속도를 다시 원래대로
            else if (value.Get<float>() == 0)
                moveSpeed /= runningSpeedModifier;
        }

        void OnInfoTab(InputValue value)
        {
            if (value.Get<float>() == 1)
            {
                infoTab.SetActive(true);
                isControlable = false;
            }
            else if (value.Get<float>() == 0)
            {
                infoTab.SetActive(false);
                isControlable = true;
            }
        }

        void FixedUpdate()
        {
            Look();
            Move();
            // bodyAnimator.SetFloat("isMoving", isMoving);
            gunAnimator.SetFloat("isMoving", isMoving);
        }
        private void Look()
        {
            yRotation += lookInputValue.x * playerXAxisSensitivity;
            xRotation -= lookInputValue.y * playerYAxisSensitivity;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            firstPersonCameraObject.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        private void Move()
        {
            Vector3 movementVec = transform.forward * moveInputValue.y + transform.right * moveInputValue.x;
            transform.position += movementVec.normalized * moveSpeed * Time.deltaTime;
        }
    }
}

