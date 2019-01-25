using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Core.Utils;
using XmlConfig;


namespace Core.CameraControl.NewMotor
{

    public enum CameraActionType
    {
        Enter,
        Leave,
        End
    }

    public class CameraActionManager
    {
        private static BitVector32 EnterAction = new BitVector32();
        private static BitVector32 LeaveAction = new BitVector32();

        private static Dictionary<int, Action<PlayerEntity, ICameraMotorState>> EnterActionList =
            new Dictionary<int, Action<PlayerEntity, ICameraMotorState>>();

        private static Dictionary<int, Action<PlayerEntity, ICameraMotorState>> LeaveActionList =
            new Dictionary<int, Action<PlayerEntity, ICameraMotorState>>();

        private static int EnumToMask(int id)
        {
            return 1<<id;
        }

        public static int CalcuMotorNum(SubCameraMotorType Mode ,int id)
        {
            int result = 0;
            switch (Mode)
            {
                case SubCameraMotorType.Pose:
                    return id;
                case SubCameraMotorType.Free:
                    return id + (int)ECameraPoseMode.End + 1;
                case SubCameraMotorType.Peek:
                    return id + (int)ECameraPoseMode.End + (int)ECameraFreeMode.End + 2;
                case SubCameraMotorType.View:
                    return id + (int)ECameraPoseMode.End + (int)ECameraFreeMode.End + (int)ECameraPeekMode.End + 3;
                default:
                    return 0;
            }
        }

        public static void CopyActionCode(CameraActionType type, int data)
        {
            if (type == CameraActionType.Enter)
                EnterAction = new BitVector32(data);
            else if (type == CameraActionType.Leave)
                LeaveAction = new BitVector32(data);
        }

        public static void SetActionCode(CameraActionType actionType, SubCameraMotorType motorType, int id)
        {
            if (actionType == CameraActionType.Enter)
            {
                EnterAction[EnumToMask(CalcuMotorNum(motorType,id))] = true;
            }
            else if (actionType == CameraActionType.Leave)
            {
                LeaveAction[EnumToMask(CalcuMotorNum(motorType,id))] = true;
            }
        }

        public static int GetActionCode(CameraActionType type)
        {
            int result = 0;
            if (type == CameraActionType.Enter)
            {
                result = EnterAction.Data;
            }
            else if (type == CameraActionType.Leave)
            {
                result = LeaveAction.Data;
            }
            return result;
        }

        public static void ClearActionCode()
        {
            EnterAction = new BitVector32();
            LeaveAction = new BitVector32();
        }

        public static void AddAction(CameraActionType actionType, SubCameraMotorType motorType, int motorId,
            Action<PlayerEntity, ICameraMotorState> act)
        {
            if (actionType == CameraActionType.Enter)
            {
                EnterActionList[CalcuMotorNum(motorType, motorId)] = act;
            }
            else if (actionType == CameraActionType.Leave)
            {
                LeaveActionList[CalcuMotorNum(motorType, motorId)] = act;
            }
        }

        public static void OnAction(PlayerEntity player, ICameraMotorState state)
        {
            for (int i = 1; i < 32; i++)
            {
                if (JudgeAction(CameraActionType.Enter, i))// && 
                    if( EnterActionList.ContainsKey(i))
                        EnterActionList[i](player, state);
                if (JudgeAction(CameraActionType.Leave, i))// &&
                    if (LeaveActionList.ContainsKey(i))
                        LeaveActionList[i](player, state);
            }
        }

        private static bool JudgeAction(CameraActionType type, int id)
        {
            if (type == CameraActionType.Enter)
            {
                return EnterAction[EnumToMask(id)];
            }
            else if (type == CameraActionType.Leave)
            {
                return LeaveAction[EnumToMask(id)];
            }

            return false;
        }
    }

}
