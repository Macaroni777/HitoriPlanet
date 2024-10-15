using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BotController
{
    public class Bot : MonoBehaviour
    {
        #region 方向定数の宣言
        static Vector2 botLocalPosition_Up = Vector2.up;
        static Vector2 botLocalPosition_Down = Vector2.down;
        static Vector2 botLocalPosition_Right = Vector2.right;
        static Vector2 botLocalPosition_Left = Vector2.left;
        static Vector2 botLocalPosition_UpRight = new Vector2(0.7f, 0.7f);
        static Vector2 botLocalPosition_UpLeft = new Vector2(-0.7f, 0.7f);
        static Vector2 botLocalPosition_DownRight = new Vector2(0.7f, -0.7f);
        static Vector2 botLocalPosition_DownLeft = new Vector2(-0.7f, -0.7f);
        enum Direction
        {
            DIRECTION_UP,
            DIRECTION_UPRIGHT,
            DIRECTION_RIGHT,
            DIRECTION_DOWNRIGHT,
            DIRECTION_DOWN,
            DIRECTION_DOWNLEFT,
            DIRECTION_LEFT,
            DIRECTION_UPLEFT
        }
        #endregion
        [SerializeField] private Cursor cursor;
        private Vector2 cursorPosition;
        private GameObject botGameObject;
        private Transform botTransform;
        public float botDistance;
        private GameObject player;
        private Transform playerTransform;
        private Vector3 diff;
        private float angle;
        ActorGroundSensor groundSensor;
        // Start is called before the first frame update
        void Start()
        {
            botGameObject = this.gameObject;
            botTransform = botGameObject.transform;
            groundSensor = GameObject.Find("GroundSensor").GetComponent<ActorGroundSensor>();
            botDistance = 4.0f;
            player = botTransform.parent.gameObject;
            playerTransform = player.transform;
        }

        // Update is called once per frame
        void Update()
        {
            SetPosition();
        }

        void SetPosition()
        {
            switch (devideDirection(GetCursorAngle()))
            {
                case Direction.DIRECTION_UP:
                    botTransform.localPosition = botLocalPosition_Up * botDistance;
                    break;
                case Direction.DIRECTION_UPRIGHT:
                    botTransform.localPosition = botLocalPosition_UpRight * botDistance;
                    break;
                case Direction.DIRECTION_RIGHT:
                    botTransform.localPosition = botLocalPosition_Right * botDistance;
                    break;
                case Direction.DIRECTION_DOWNRIGHT:
                    botTransform.localPosition = botLocalPosition_DownRight * botDistance;
                    break;
                case Direction.DIRECTION_DOWN:
                    botTransform.localPosition = botLocalPosition_Down * botDistance;
                    break;
                case Direction.DIRECTION_DOWNLEFT:
                    botTransform.localPosition = botLocalPosition_DownLeft * botDistance;
                    break;
                case Direction.DIRECTION_LEFT:
                    botTransform.localPosition = botLocalPosition_Left * botDistance;
                    break;
                case Direction.DIRECTION_UPLEFT:
                    botTransform.localPosition = botLocalPosition_UpLeft * botDistance;
                    break;
            }
        }

        float GetCursorAngle()
        {
            cursorPosition = cursor.GetCursorPos();
            diff = (Vector3)cursorPosition - playerTransform.position;
            angle = Vector3.Angle(playerTransform.up, diff) * (diff.x < 0 ? -1 : 1);
            return angle;
        }
        Direction devideDirection(float angle)
        {
            if (groundSensor.isGround)
            {
                #region 地面接触時
                if (-22.5 <= angle && angle <= 22.5)
                {
                    return Direction.DIRECTION_UP;
                }
                else if (22.5 < angle && angle <= 67.5)
                {
                    return Direction.DIRECTION_UPRIGHT;
                }
                else if (67.5 < angle && angle <= 180)
                {
                    return Direction.DIRECTION_RIGHT;
                }
                else if (-180 <= angle && angle < -67.5)
                {
                    return Direction.DIRECTION_LEFT;
                }
                else
                {
                    return Direction.DIRECTION_UPLEFT;
                }
                #endregion
            }
            else
            {
                #region 地面非接触時
                if (-22.5 <= angle && angle <= 22.5)
                {
                    return Direction.DIRECTION_UP;
                }
                else if (22.5 < angle && angle <= 67.5)
                {
                    return Direction.DIRECTION_UPRIGHT;
                }
                else if (67.5 < angle && angle <= 112.5)
                {
                    return Direction.DIRECTION_RIGHT;
                }
                else if (112.5 < angle && angle <= 157.5)
                {
                    return Direction.DIRECTION_DOWNRIGHT;
                }
                else if (157.5 < angle || angle < -157.5)
                {
                    return Direction.DIRECTION_DOWN;
                }
                else if (-157.5 <= angle && angle < -112.5)
                {
                    return Direction.DIRECTION_DOWNLEFT;
                }
                else if (-112.5 <= angle && angle < -67.5)
                {
                    return Direction.DIRECTION_LEFT;
                }
                else
                {
                    return Direction.DIRECTION_UPLEFT;
                }
                #endregion
            }
        }
    }
}