using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using RuntimeGizmos;

namespace MoveEditCam
{

    [RequireComponent(typeof(Camera))]
    public class MoveEditCamManager : MonoBehaviour
    {
        // カメラコンポーネント
        private Camera _cam;
        // 本機能を使用するかフラグ
        private bool _active = true;
        // モード管理
        private MoveEditCamMode _mode = MoveEditCamMode.None;


        // Handモード用変数
        [SerializeField, Header("ハンドモード割り当てキー")]
        private KeyCode _modeHandKey = KeyCode.Q;
        [SerializeField, Header("Handモードの感度")]
        private float _handSensitivity = 0.5f;
        private bool _handMoveing = false;

        // HandWheelモード用変数
        [SerializeField, Header("HandWheelモードの感度")]
        private float _handWheelSensitivity = 1.0f;

        // FPSモード用変数
        [SerializeField, Header("FPS移動の感度")]
        private float _fpsMoveSensitivity = 0.01f;
        [SerializeField, Header("FPS視点移動の感度")]
        private float _fpsEyeMoveSensitivity = 1f;
        
        // ホイール拡大用変数
        [SerializeField, Header("Wheel移動感度")]
        private float _wheelSensitivity = 1f;
        

        void Awake()
        {
            this._cam = this.GetComponent<Camera>();
            _mode = MoveEditCamMode.None;
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(this._active)
            {
                // Qでハンドモード移行（ギズモ停止）
                if(Input.GetKeyDown(_modeHandKey) && _mode != MoveEditCamMode.FPS)
                {
                    // todo:ギズモのselectedobjectを空にする
                    this._mode = MoveEditCamMode.Hand;
                }
                // WERでハンドモード終了
                if(this._mode == MoveEditCamMode.Hand && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.R)))
                {
                    this._mode = MoveEditCamMode.None;
                }

                // ホイールクリックでハンドモード（ホイール）移行
                if(Input.GetMouseButtonDown(2))
                {
                    this._mode = MoveEditCamMode.HandWheel;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                if(Input.GetMouseButtonUp(2))
                {
                    this._mode = MoveEditCamMode.None;
                    Cursor.lockState = CursorLockMode.None;
                }


                // 右クリックFPS移動有効化
                if(Input.GetMouseButtonDown(1))
                {
                    // todo:ギズモのモードチェンジを無効にする
                    this._mode = MoveEditCamMode.FPS;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                if(Input.GetMouseButtonUp(1))
                {
                    this._mode = MoveEditCamMode.None;
                    Cursor.lockState = CursorLockMode.None;
                }

                ExeMove();
            }
        }

        // modeを確認してそれぞれのモードに応じた処理を行う
        void ExeMove()
        {
            if(this._mode == MoveEditCamMode.Hand)
            {
                // ハンドモード
                HandMove();
            }
            else if(this._mode == MoveEditCamMode.HandWheel)
            {
                HandMoveWheel();
            }
            else if(this._mode == MoveEditCamMode.FPS)
            {
                // FPSモード
                FPSMove();
            }
            else
            {
                // none
            }

            // ホイールで前後移動
            WheelMove();

        }

        void HandMove()
        {
            if(Input.GetMouseButtonDown(0))
            {
                _handMoveing = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            if(Input.GetMouseButtonUp(0))
            {
                _handMoveing = false;
                Cursor.lockState = CursorLockMode.None;
            }
            if(_handMoveing)
            {
                Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                this._cam.transform.Translate(-delta.x * _handSensitivity, -delta.y * _handSensitivity, 0);
            }
        }

        void HandMoveWheel()
        {
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            this._cam.transform.Translate(-delta.x * _handWheelSensitivity, -delta.y * _handWheelSensitivity, 0);
        }

        void FPSMove()
        {
            // FPS視点移動
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            this.transform.Rotate(delta.y * _fpsEyeMoveSensitivity*-1, delta.x * _fpsEyeMoveSensitivity, 0);
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, 0);

            // FPS移動
            if(Input.GetKey(KeyCode.W))
            {
                MoveForward(_fpsMoveSensitivity);
            }
            if(Input.GetKey(KeyCode.S))
            {
                MoveBack(_fpsMoveSensitivity);
            }
            if(Input.GetKey(KeyCode.A))
            {
                MoveLeft(_fpsMoveSensitivity);
            }
            if(Input.GetKey(KeyCode.D))
            {
                MoveRight(_fpsMoveSensitivity);
            }
            if(Input.GetKey(KeyCode.E))
            {
                MoveUp(_fpsMoveSensitivity);
            }
            if(Input.GetKey(KeyCode.Q))
            {
                MoveDown(_fpsMoveSensitivity);
            }
        }

        void WheelMove()
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if(wheel > 0)
            {
                MoveForward(wheel * _wheelSensitivity);
            }
            else if(wheel < 0)
            {
                MoveBack(Mathf.Abs(wheel) * _wheelSensitivity);
            }
        }


        void MoveForward(float sensitivity)
        {
            Vector3 forward = this.transform.forward;
            this.transform.position += forward * sensitivity;
        }

        void MoveBack(float sensitivity)
        {
            Vector3 back = this.transform.forward * -1;
            this.transform.position += back * sensitivity;
        }
        void MoveRight(float sensitivity)
        {
            Vector3 right = this.transform.right;
            this.transform.position += right * sensitivity;
        }
        void MoveLeft(float sensitivity)
        {
            Vector3 left = this.transform.right * -1;
            this.transform.position += left * sensitivity;
        }
        void MoveUp(float sensitivity)
        {
            Vector3 up = this.transform.up;
            this.transform.position += up * sensitivity;
        }
        void MoveDown(float sensitivity)
        {
            Vector3 down = this.transform.up * -1;
            this.transform.position += down * sensitivity;
        }
    }
}
