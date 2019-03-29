using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECM.Components;
using UnityEngine;
using Utils.Appearance;
using XmlConfig;

namespace Core.CharacterController
{
    public interface ICharacterController:ICharacterDebugDraw
    {
        object RealValue { get; }
        /// <summary>
        /// 旋转角色到目标位置
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deltaTime"></param>
        void Rotate(Quaternion target, float deltaTime);
        /// <summary>
        /// 移动角色
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="deltaTime"></param>
        void Move(Vector3 dist, float deltaTime = 0.0f);
        Transform transform { get; }
        GameObject gameObject { get; }
        float radius { get; }
        float height { get; }
        Vector3 center { get; }
        bool enabled { get; set; }
        bool isGrounded { get; }
        float slopeLimit { get; }
        Vector3 direction { get; }
        /// <summary>
        /// 把角色设置到targetPos位置
        /// </summary>
        /// <param name="targetPos"></param>
        void SetCharacterPosition(Vector3 targetPos);
        /// <summary>
        /// 把角色旋转设置到targetPos位置
        /// </summary>
        void SetCharacterRotation(Quaternion rot);
        /// <summary>
        /// 用欧拉角设置角度,采用unity的旋转角度，zxy
        /// </summary>
        /// <param name="euler"></param>
        void SetCharacterRotation(Vector3 euler);
        /// <summary>
        /// 初始化参数
        /// </summary>
        void Init();
        UnityEngine.CollisionFlags collisionFlags { get; }
        
        GroundHit GetGroundHit { get; }

        ControllerHitInfo GetCharacterControllerHitInfo(HitType type = HitType.Down);
        
        KeyValuePair<float, float> GetRotateBound(Quaternion prevRot, Vector3 prevPos, int frameInterval);
        

        Collider GetCollider();

    }

    public interface ICharacterDebugDraw
    {
        void DrawBoundingBox();
        void DrawLastGroundHit();
        void DrawGround();
    }

    public enum CharacterControllerType
    {
        UnityCharacterController,
        ProneKinematicCharacterController,
        DiveKinematicCharacterController,
        SwimKinematicCharacterController,
        End
    }

    public interface ICharacterControllerContext : ICharacterController
    {
        CharacterControllerType GetCurrentControllerType();
        void SetCurrentControllerType(CharacterControllerType type);
        void SetCurrentControllerType(PostureInConfig type);
    }
}
