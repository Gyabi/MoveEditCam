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
        private float _handSensitivity = 0.01f;
        private Vector3 _handPreClickPos = new Vector3();
        private bool _handMoveing = false;

        // HandWheelモード用変数
        [SerializeField, Header("HandWheelモードの感度")]
        private float _handWheelSensitivity = 0.01f;
        private Vector3 _handWheelPreClickPos = new Vector3();

        // FPSモード用変数
        [SerializeField, Header("FPS移動の感度")]
        private float _fpsMoveSensitivity = 0.01f;
        [SerializeField, Header("FPS視点移動の感度")]
        private float _fpsEyeMoveSensitivity = 0.01f;
        private Vector3 _fpsEyeMovePreClickPos = new Vector3();
        
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
                }
                if(Input.GetMouseButtonUp(2))
                {
                    this._mode = MoveEditCamMode.None;
                }


                // 右クリックFPS移動有効化
                if(Input.GetMouseButtonDown(1))
                {
                    // todo:ギズモのモードチェンジを無効にする
                    this._mode = MoveEditCamMode.FPS;
                }
                if(Input.GetMouseButtonUp(1))
                {
                    this._mode = MoveEditCamMode.None;
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
                _handPreClickPos = Input.mousePosition;
            }
            if(Input.GetMouseButtonUp(0))
            {
                _handMoveing = false;
            }
            if(_handMoveing)
            {
                Vector2 delta = Input.mousePosition - _handPreClickPos;
                this._cam.transform.Translate(-delta.x * _handSensitivity, -delta.y * _handSensitivity, 0);
                _handPreClickPos = Input.mousePosition;
            }
        }

        void HandMoveWheel()
        {
            if(Input.GetMouseButtonDown(2))
            {
                _handWheelPreClickPos = Input.mousePosition;
            }
            Vector2 delta = Input.mousePosition - _handWheelPreClickPos;
            this._cam.transform.Translate(-delta.x * _handWheelSensitivity, -delta.y * _handWheelSensitivity, 0);
            _handWheelPreClickPos = Input.mousePosition;
        }

        void FPSMove()
        {
            // FPS視点移動
            if(Input.GetMouseButtonDown(1))
            {
                _fpsEyeMovePreClickPos = Input.mousePosition;
            }
            Vector2 delta = Input.mousePosition - _fpsEyeMovePreClickPos;
            this.transform.Rotate(delta.y * _fpsEyeMoveSensitivity*-1, delta.x * _fpsEyeMoveSensitivity, 0);
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, 0);
            _fpsEyeMovePreClickPos = Input.mousePosition;

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

            // // カーソルが画面内を無限に移動できるように設定
            // if(Input.mousePosition.x == 0)
            // {
                
            // }
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
