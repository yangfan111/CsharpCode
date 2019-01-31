using System;
using System.Collections.Generic;
using Core.CameraControl;
using Core.Utils;
using UnityEngine;
using Utils.Compare;

namespace KinematicCharacterController
{
    public enum CharacterSystemInterpolationMethod
    {
        None,
        Unity,
        Custom
    }

    /// <summary>
    /// The system that manages the simulation of KinematicCharacterMotor and PhysicsMover
    /// </summary>
    //[DefaultExecutionOrder(-100)]
    public class KinematicCharacterSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(KinematicCharacterSystem));

        private static KinematicCharacterSystem _instance = new KinematicCharacterSystem();

        /// <summary>
        /// All KinematicCharacterMotor currently being simulated
        /// </summary>
        public static List<KinematicCharacterMotor> CharacterMotors =
            new List<KinematicCharacterMotor>(CharacterMotorsBaseCapacity);

        /// <summary>
        /// All PhysicsMover currently being simulated
        /// </summary>
        public static List<PhysicsMover> PhysicsMovers = new List<PhysicsMover>(PhysicsMoversBaseCapacity);

        /// <summary>
        /// Determines if the system simulates automatically.
        /// If true, the simulation is done on FixedUpdate
        /// </summary>
        public static bool AutoSimulation = false;

        public static bool DebugShowFlyBoundingBox = false;


        private static float _lastCustomInterpolationStartTime = -1f;
        private static float _lastCustomInterpolationDeltaTime = -1f;

        private const int CharacterMotorsBaseCapacity = 100;
        private const int PhysicsMoversBaseCapacity = 100;

        [SerializeField]
        private static CharacterSystemInterpolationMethod _internalInterpolationMethod =
            CharacterSystemInterpolationMethod.Custom;

        /// <summary>
        /// 用于更新期间的物理插值。
        /// 插值用来估算在物理更新期间刚体的位置。这用来改变当图形显示更新比物理更新更频繁时，因为对象会出现急剧抖动而不是平滑运动。在内插值模式，在上一帧基于对象的位置运动被平滑。外插值模式平滑运动是在下一帧基于它估计的位置。模式的选择取决于在游戏中对象的动态。
        /// Sets the interpolation method of the system:
        /// - None: no interpolation
        /// - Unity: uses Unity's built-in rigidbody interpolation, unity的内插值
        /// - Custom: uses a custom interpolation
        /// </summary>
        public static CharacterSystemInterpolationMethod InterpolationMethod
        {
            get { return _internalInterpolationMethod; }
            set
            {
                _internalInterpolationMethod = value;

                MoveActorsToDestination();

                // Setup rigidbodies for interpolation
                RigidbodyInterpolation interpMethod =
                    (_internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity)
                        ? RigidbodyInterpolation.Interpolate
                        : RigidbodyInterpolation.None;
                for (int i = 0; i < CharacterMotors.Count; i++)
                {
                    CharacterMotors[i].Rigidbody.interpolation = interpMethod;
                }

                for (int i = 0; i < PhysicsMovers.Count; i++)
                {
                    PhysicsMovers[i].Rigidbody.interpolation = interpMethod;
                }
            }
        }

        /// <summary>
        /// Creates a KinematicCharacterSystem instance if there isn't already one
        /// </summary>
        public static void EnsureCreation()
        {
            if (_instance == null)
            {
                //GameObject systemGameObject = new GameObject("KinematicCharacterSystem");
                _instance = new KinematicCharacterSystem();

                //systemGameObject.hideFlags = HideFlags.NotEditable;
                //_instance.hideFlags = HideFlags.NotEditable;
            }
        }

        /// <summary>
        /// Gets the KinematicCharacterSystem instance if any
        /// </summary>
        /// <returns></returns>
        public static KinematicCharacterSystem GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Sets the maximum capacity of the character motors list, to prevent allocations when adding characters
        /// </summary>
        /// <param name="capacity"></param>
        public static void SetCharacterMotorsCapacity(int capacity)
        {
            if (capacity < CharacterMotors.Count)
            {
                capacity = CharacterMotors.Count;
            }

            CharacterMotors.Capacity = capacity;
        }

        /// <summary>
        /// Registers a KinematicCharacterMotor into the system
        /// </summary>
        public static void RegisterCharacterMotor(KinematicCharacterMotor motor)
        {
            CharacterMotors.Add(motor);

            RigidbodyInterpolation interpMethod =
                (_internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity)
                    ? RigidbodyInterpolation.Interpolate
                    : RigidbodyInterpolation.None;
            motor.Rigidbody.interpolation = interpMethod;
        }

        /// <summary>
        /// Unregisters a KinematicCharacterMotor from the system
        /// </summary>
        public static void UnregisterCharacterMotor(KinematicCharacterMotor motor)
        {
            CharacterMotors.Remove(motor);
        }

        /// <summary>
        /// Sets the maximum capacity of the physics movers list, to prevent allocations when adding movers
        /// </summary>
        /// <param name="capacity"></param>
        public static void SetPhysicsMoversCapacity(int capacity)
        {
            if (capacity < PhysicsMovers.Count)
            {
                capacity = PhysicsMovers.Count;
            }

            PhysicsMovers.Capacity = capacity;
        }

        /// <summary>
        /// Registers a PhysicsMover into the system
        /// </summary>
        public static void RegisterPhysicsMover(PhysicsMover mover)
        {
            PhysicsMovers.Add(mover);

            RigidbodyInterpolation interpMethod =
                (_internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity)
                    ? RigidbodyInterpolation.Interpolate
                    : RigidbodyInterpolation.None;
            mover.Rigidbody.interpolation = interpMethod;
        }

        /// <summary>
        /// Unregisters a PhysicsMover from the system
        /// </summary>
        public static void UnregisterPhysicsMover(PhysicsMover mover)
        {
            PhysicsMovers.Remove(mover);
        }

        // This is to prevent duplicating the singleton gameobject on script recompiles
        private void OnDisable()
        {
            //Destroy(this.gameObject);
        }

        private void Awake()
        {
            _instance = this;
        }

        private void FixedUpdate()
        {
            if (AutoSimulation)
            {
                float deltaTime = Time.deltaTime;

                PreSimulationUpdate(deltaTime);
                Simulate(deltaTime);
                PostSimulationUpdate(deltaTime);
            }
        }

        private void Update()
        {
            if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
            {
                CustomInterpolationUpdate();
            }
        }

        /// <summary>
        /// Ticks the character system (ticks all KinematicCharacterMotors and PhysicsMovers)
        /// </summary>
        public static void Simulate(float deltaTime)
        {
            // Update PhysicsMover velocities
            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].VelocityUpdate(deltaTime);
            }

            // Character controller update phase 1
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].UpdatePhase1(deltaTime);
            }

            // Simulate PhysicsMover displacement
            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].Transform.SetPositionAndRotation(PhysicsMovers[i].TransientPosition,
                    PhysicsMovers[i].TransientRotation);
                PhysicsMovers[i].Rigidbody.position = PhysicsMovers[i].TransientPosition;
                PhysicsMovers[i].Rigidbody.rotation = PhysicsMovers[i].TransientRotation;
            }

            // Character controller update phase 2 and move
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].UpdatePhase2(deltaTime);
                CharacterMotors[i].Transform.SetPositionAndRotation(CharacterMotors[i].TransientPosition,
                    CharacterMotors[i].TransientRotation);
                CharacterMotors[i].Rigidbody.position = CharacterMotors[i].TransientPosition;
                CharacterMotors[i].Rigidbody.rotation = CharacterMotors[i].TransientRotation;
            }
        }

        /// <summary>
        /// Ticks the character system (ticks all KinematicCharacterMotors and PhysicsMovers)
        /// </summary>
        public static void Simulate(float deltaTime, KinematicCharacterMotor[] motors, int characterMotorsCount,
            PhysicsMover[] movers, int physicsMoversCount)
        {
#pragma warning disable 0162
            // Update PhysicsMover velocities
            for (int i = 0; i < physicsMoversCount; i++)
            {
                movers[i].VelocityUpdate(deltaTime);
            }

            // Character controller update phase 1
            for (int i = 0; i < characterMotorsCount; i++)
            {
                motors[i].UpdatePhase1(deltaTime);
            }

            // Simulate PhysicsMover displacement
            for (int i = 0; i < physicsMoversCount; i++)
            {
                movers[i].Transform.SetPositionAndRotation(movers[i].TransientPosition, movers[i].TransientRotation);
                movers[i].Rigidbody.position = movers[i].TransientPosition;
                movers[i].Rigidbody.rotation = movers[i].TransientRotation;
            }

            // Character controller update phase 2 and move
            for (int i = 0; i < characterMotorsCount; i++)
            {
                motors[i].UpdatePhase2(deltaTime);
                motors[i].Transform.SetPositionAndRotation(motors[i].TransientPosition, motors[i].TransientRotation);
                motors[i].Rigidbody.position = motors[i].TransientPosition;
                motors[i].Rigidbody.rotation = motors[i].TransientRotation;
            }
#pragma warning restore 0162
        }

        public static void MySimulate(float deltaTime, KinematicCharacterMotor motor)
        {
            // Make sure all actors have reached their interpolation destination
            if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
            {
                motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
                motor.Rigidbody.position = motor.TransientPosition;
                motor.Rigidbody.rotation = motor.TransientRotation;
            }

            // Save pre-simulation poses
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                motor.InitialTickPosition = motor.Transform.position;
                motor.InitialTickRotation = motor.Transform.rotation;
            }

            Simulate(deltaTime, motor);

            // post

            // Return characters to their initial poses and move to target
//            motor.Rigidbody.position = motor.InitialTickPosition;
//            motor.Rigidbody.rotation = motor.InitialTickRotation;
            motor.Rigidbody.position = motor.TransientPosition;
            motor.Rigidbody.rotation = motor.TransientRotation;
            motor.Rigidbody.MovePosition(motor.TransientPosition);
            motor.Rigidbody.MoveRotation(motor.TransientRotation);
        }


        public static void FlySimulate(float deltaTime, KinematicCharacterMotor motor)
        {
            var prevPos = motor.TransientPosition;
            var prevRot = motor.TransientRotation;
//            Logger.InfoFormat("motor. rotation prev:{0}, use my:{1}, prevPos:{2}",
//                YawPitchUtility.Normalize(prevRot.eulerAngles).ToStringExt(),
//                YawPitchUtility
//                    .Normalize(Quaternion.Euler(0, YawPitchUtility.Normalize(motor.TransientRotation.eulerAngles.y), 0))
//                    .ToStringExt(),
//                prevPos.ToStringExt()
//                );
            //var prevRot = Quaternion.Euler(0,YawPitchUtility.Normalize(motor.TransientRotation.eulerAngles.y),0);
            // 把当前的胶囊体，沿着胶囊体的中心旋转
            var characterToCapsuleCenterMatrix = Matrix4x4.TRS(motor.Capsule.center, Quaternion.identity, Vector3.one);
            var capsuleCenterMatrix = Matrix4x4.TRS(motor.TransientPosition, motor.TransientRotation, Vector3.one) *
                                      characterToCapsuleCenterMatrix;
            var centerPos = capsuleCenterMatrix.ExtractPosition();
            var centerRot = capsuleCenterMatrix.ExtractRotation();
            centerRot = centerRot * Quaternion.Euler(motor.FlyOffsetX, 0, motor.FlyOffsetZ);
//            Logger.InfoFormat("motor.FlyOffsetX:{0},y offset:{1}",motor.FlyOffsetX, motor.CalcFlyAngleXRatio(motor.FlyOffsetX) 
//                                             * motor.FlyModePosYOffset);
            capsuleCenterMatrix = Matrix4x4.TRS(centerPos + new Vector3(0,motor.CalcFlyAngleXRatio(motor.FlyOffsetX) 
            * motor.FlyModePosYOffset,0),
                centerRot,
                Vector3.one);

            var newCharacterMatrix = capsuleCenterMatrix * characterToCapsuleCenterMatrix.inverse;
            //Logger.InfoFormat("fly simulate angleY:{0}, prev y:{1}",YawPitchUtility.Normalize(newCharacterMatrix.ExtractRotation().eulerAngles).ToStringExt() , YawPitchUtility.Normalize(prevRot.eulerAngles).ToStringExt());
            motor.SetPosition(newCharacterMatrix.ExtractPosition());
            motor.SetRotation(newCharacterMatrix.ExtractRotation());
            
            motor.UpdatePhase1(deltaTime);
            motor.UpdatePhase2(deltaTime);

            if (DebugShowFlyBoundingBox)
            {
                DrawBoundingBox(motor);
                DebugShowFlyBoundingBox = false;
            }
//            if (!CompareUtility.IsApproximatelyEqual(newCharacterMatrix.ExtractRotation().eulerAngles,
//                motor.TransientRotation.eulerAngles))
//            {
//                Logger.InfoFormat("angle not same:before:{0},after:{1}",newCharacterMatrix.ExtractRotation().eulerAngles,
//                    motor.TransientRotation.eulerAngles);
//            }
            

            // 把胶囊体放正
            capsuleCenterMatrix = Matrix4x4.TRS(motor.TransientPosition - new Vector3(0,motor.CalcFlyAngleXRatio(motor.FlyOffsetX) 
                                                                                        * motor.FlyModePosYOffset,0), motor.TransientRotation, Vector3.one) *
                                  characterToCapsuleCenterMatrix;

            centerPos = capsuleCenterMatrix.ExtractPosition();
            centerRot = prevRot;
            
            capsuleCenterMatrix = Matrix4x4.TRS(centerPos, centerRot, Vector3.one);
            
            newCharacterMatrix = capsuleCenterMatrix * characterToCapsuleCenterMatrix.inverse;

            var newPos = capsuleCenterMatrix.ExtractPosition() - motor.Capsule.center;
            
            //Logger.ErrorFormat("capsule center - (0,1,0):{0}, capsuleM * toCenter.inverse:{1}, prevPos:{2},capsule center pos:{3}", newPos.ToStringExt(), newCharacterMatrix.ExtractPosition().ToStringExt(),prevPos.ToStringExt() ,capsuleCenterMatrix.ExtractPosition().ToStringExt());

            motor.Transform.SetPositionAndRotation(newCharacterMatrix.ExtractPosition(), newCharacterMatrix.ExtractRotation());
            motor.Rigidbody.position = newCharacterMatrix.ExtractPosition();
            motor.Rigidbody.rotation = newCharacterMatrix.ExtractRotation();
            //Logger.InfoFormat("after rot:{0}", newCharacterMatrix.ExtractRotation().eulerAngles.ToStringExt());
//            var tmp1 = newCharacterMatrix.ExtractPosition();
//            var tmp2 = capsuleCenterMatrix.ExtractPosition();
//            if (CompareUtility.IsApproximatelyEqual(tmp1.x, tmp2.x) &&
//                CompareUtility.IsApproximatelyEqual(tmp1.z, tmp2.z))
//            {
//            }
//            else
//            {
//                Logger.InfoFormat("center pos:{0}, new Pos:{1}, center rot:{2}, player rot:{3}",
//                    capsuleCenterMatrix.ExtractPosition().ToStringExt(),
//                    newCharacterMatrix.ExtractPosition().ToStringExt(),
//                    YawPitchUtility.Normalize(capsuleCenterMatrix.ExtractRotation().eulerAngles).ToStringExt(),
//                    YawPitchUtility.Normalize(prevRot.eulerAngles).ToStringExt()
//                    );
//            }
        }

        private static void DrawBoundingBox(KinematicCharacterMotor motor)
        {
            var characterTransformToCapsuleBottom = motor.CharacterTransformToCapsuleBottom;
            var characterTransformToCapsuleTop = motor.CharacterTransformToCapsuleTop;
//            DebugDraw.EditorDrawCapsule(motor.TransientPosition + motor.TransientRotation * characterTransformToCapsuleBottom,
//                motor.TransientPosition + motor.TransientRotation * characterTransformToCapsuleTop, motor.CapsuleRadius,
//                Color.magenta);
            DebugDraw.DebugCapsule(motor.TransientPosition + motor.TransientRotation * characterTransformToCapsuleBottom,
                motor.TransientPosition + motor.TransientRotation * characterTransformToCapsuleTop,
                Color.magenta, motor.CapsuleRadius);
        }

        public static KeyValuePair<float, float> MyCalcRotateBound(KinematicCharacterMotor motor, int frameInterval)
        {
            motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            motor.Rigidbody.position = motor.TransientPosition;
            motor.Rigidbody.rotation = motor.TransientRotation;

            motor.UpdatePhase1(frameInterval * 0.001f);
            return motor.CalcRotateBound(frameInterval * 0.001f);

        }

        public static void Simulate(float deltaTime, KinematicCharacterMotor motor)
        {
            motor.UpdatePhase1(deltaTime);

            motor.UpdatePhase2(deltaTime);
            motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            motor.Rigidbody.position = motor.TransientPosition;
            motor.Rigidbody.rotation = motor.TransientRotation;
        }

        /// <summary>
        /// Remembers the point to interpolate from for KinematicCharacterMotors and PhysicsMovers
        /// </summary>
        public static void PreSimulationUpdate(float deltaTime)
        {
            // Make sure all actors have reached their interpolation destination
            if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
            {
                MoveActorsToDestination();
            }

            // Save pre-simulation poses
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].InitialTickPosition = CharacterMotors[i].Transform.position;
                CharacterMotors[i].InitialTickRotation = CharacterMotors[i].Transform.rotation;
            }

            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].InitialTickPosition = PhysicsMovers[i].Transform.position;
                PhysicsMovers[i].InitialTickRotation = PhysicsMovers[i].Transform.rotation;
            }
        }

        /// <summary>
        /// Initiates the interpolation for KinematicCharacterMotors and PhysicsMovers
        /// </summary>
        public static void PostSimulationUpdate(float deltaTime)
        {
            // A sync is required here to make MovePosition/Rotation work properly 
            // without getting overridden by transform changes made during simulation
#if UNITY_2017_2_OR_NEWER
            Physics.SyncTransforms();
#endif

            if (InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
            {
                _lastCustomInterpolationStartTime = Time.time;
                _lastCustomInterpolationDeltaTime = deltaTime;
            }

            // Return characters to their initial poses and move to target
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].Rigidbody.position = CharacterMotors[i].InitialTickPosition;
                CharacterMotors[i].Rigidbody.rotation = CharacterMotors[i].InitialTickRotation;
                CharacterMotors[i].Rigidbody.MovePosition(CharacterMotors[i].TransientPosition);
                CharacterMotors[i].Rigidbody.MoveRotation(CharacterMotors[i].TransientRotation);
            }

            // Return movers to their initial poses and move to target
            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].Rigidbody.position = PhysicsMovers[i].InitialTickPosition;
                PhysicsMovers[i].Rigidbody.rotation = PhysicsMovers[i].InitialTickRotation;
                PhysicsMovers[i].Rigidbody.MovePosition(PhysicsMovers[i].TransientPosition);
                PhysicsMovers[i].Rigidbody.MoveRotation(PhysicsMovers[i].TransientRotation);
            }
        }

        /// <summary>
        /// Move all actors to their goal instantly
        /// </summary>
        private static void MoveActorsToDestination()
        {
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].Transform.SetPositionAndRotation(CharacterMotors[i].TransientPosition,
                    CharacterMotors[i].TransientRotation);
                CharacterMotors[i].Rigidbody.position = CharacterMotors[i].TransientPosition;
                CharacterMotors[i].Rigidbody.rotation = CharacterMotors[i].TransientRotation;
            }

            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].Transform.SetPositionAndRotation(PhysicsMovers[i].TransientPosition,
                    PhysicsMovers[i].TransientRotation);
                PhysicsMovers[i].Rigidbody.position = PhysicsMovers[i].TransientPosition;
                PhysicsMovers[i].Rigidbody.rotation = PhysicsMovers[i].TransientRotation;
            }
        }

        /// <summary>
        /// Handles per-frame interpolation
        /// </summary>
        private static void CustomInterpolationUpdate()
        {
            float interpolationFactor =
                Mathf.Clamp01((Time.time - _lastCustomInterpolationStartTime) / _lastCustomInterpolationDeltaTime);

            // Handle characters interpolation
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].Transform.SetPositionAndRotation(
                    Vector3.Lerp(CharacterMotors[i].InitialTickPosition, CharacterMotors[i].TransientPosition,
                        interpolationFactor),
                    Quaternion.Slerp(CharacterMotors[i].InitialTickRotation, CharacterMotors[i].TransientRotation,
                        interpolationFactor));
            }

            // Handle PhysicsMovers interpolation
            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].Transform.SetPositionAndRotation(
                    Vector3.Lerp(PhysicsMovers[i].InitialTickPosition, PhysicsMovers[i].TransientPosition,
                        interpolationFactor),
                    Quaternion.Slerp(PhysicsMovers[i].InitialTickRotation, PhysicsMovers[i].TransientRotation,
                        interpolationFactor));
            }
        }
    }
}