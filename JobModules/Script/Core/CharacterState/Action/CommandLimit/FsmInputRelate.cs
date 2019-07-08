using System.Collections.Generic;
using Core.Animation;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.Utils;

namespace Core.CharacterState.Action.CommandLimit
{
    public class FsmInputRelate
    {
	    private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FsmInputRelate));
	    
	    private readonly AnimationClipNameMatcher _matcher = new AnimationClipNameMatcher();
	    
        private static Dictionary<FsmInput, FsmInput[]> _relates =
            new Dictionary<FsmInput, FsmInput[]>(
                CommonIntEnumEqualityComparer<FsmInput>.Instance)
            {
                { FsmInput.Freefall, new []{FsmInput.Jump} },
	            { FsmInput.JumpEndFinished, new []{FsmInput.Jump} },
	            { FsmInput.FireFinished, new []{FsmInput.Fire, FsmInput.SpecialFire, 
		            FsmInput.SightsFire, FsmInput.SpecialFireEnd, FsmInput.SpecialSightsFire} },
	            { FsmInput.FireEndFinished, new []{FsmInput.Fire, FsmInput.SpecialFire, 
		            FsmInput.SightsFire, FsmInput.SpecialFireEnd, FsmInput.SpecialSightsFire} },
	            { FsmInput.InjuryFinished, new []{FsmInput.Injury} },
	            { FsmInput.ReloadFinished, new []{FsmInput.Reload, FsmInput.SpecialReload, 
		            FsmInput.ReloadEmpty, FsmInput.SpecialReloadTrigger} },
	            { FsmInput.HolsterStartFinished, new []{FsmInput.Unarm, FsmInput.SwitchWeapon} },
	            { FsmInput.HolsterEndFinished, new []{FsmInput.Unarm, FsmInput.SwitchWeapon} },
	            { FsmInput.SelectFinished, new []{FsmInput.Draw, FsmInput.SwitchWeapon} },
	            { FsmInput.PickUpEnd, new []{FsmInput.PickUp} },
	            { FsmInput.OpenDoorEnd, new []{FsmInput.OpenDoor} },
	            { FsmInput.PropsEnd, new []{FsmInput.Props, FsmInput.FinishProps} },
	            { FsmInput.MeleeAttackFinished, new []
	            { FsmInput.LightMeleeAttackOne, FsmInput.LightMeleeAttackTwo,
		            FsmInput.MeleeSpecialAttack } },
	            { FsmInput.GrenadeEndFinish, new []{FsmInput.StartFarGrenade, FsmInput.StartNearGrenade} },
	            { FsmInput.ParachuteOpen1Finished, new []{FsmInput.Parachuting, FsmInput.ParachutingEnd} },
	            { FsmInput.ToProneTransitFinish, new []{FsmInput.Prone} },
	            { FsmInput.OutProneTransitFinish, new []{FsmInput.Prone} },
	            { FsmInput.GenericActionFinished, new []{FsmInput.Climb} },
	            { FsmInput.BuriedBombFinished, new []{FsmInput.BuriedBomb} },
	            { FsmInput.DismantleBombFinished, new []{FsmInput.DismantleBomb} },
	            { FsmInput.DyingTransitionFinished, new []{FsmInput.Dying} },
	            { FsmInput.EnterLadderFinished, new []{FsmInput.EnterLadder} },
	            { FsmInput.ExitLadderFinished, new []{FsmInput.ExitLadder} },
	            { FsmInput.TransfigurationStartEnd, new []{FsmInput.TransfigurationStart} },
	            { FsmInput.TransfigurationFinishEnd, new []{FsmInput.TransfigurationFinish} },
	            { FsmInput.RageStartFinished, new []{FsmInput.RageStart} },
	            { FsmInput.RageEndFinished, new []{FsmInput.RageEnd} },
            };
	    
	    public static Dictionary<string, StateChange> AnimationFinished = new Dictionary<string, StateChange>
			{
			    {"JumpStart",       new StateChange { Value = false, Command = FsmInput.Freefall } },//有过渡
			    {"JumpEnd",         new StateChange { Value = false, Command = FsmInput.JumpEndFinished } },//有过渡
			    {"Fire",            new StateChange { Value = false, Command = FsmInput.FireFinished } },
			    {"FireEnd",         new StateChange { Value = false, Command = FsmInput.FireEndFinished } },
			    {"Injury",          new StateChange { Value = false, Command = FsmInput.InjuryFinished } },
			    {"Reload",          new StateChange { Value = false, Command = FsmInput.ReloadFinished } },
			    {"ReloadEmpty",     new StateChange { Value = false, Command = FsmInput.ReloadFinished } },
			    {"HolsterStart",    new StateChange { Value = false, Command = FsmInput.HolsterStartFinished } },
			    {"HolsterEnd",      new StateChange { Value = false, Command = FsmInput.HolsterEndFinished } },
			    {"Select",          new StateChange { Value = false, Command = FsmInput.SelectFinished } },
			    {"ReloadEnd",       new StateChange { Value = false, Command = FsmInput.ReloadFinished } },//有过渡
			    {"PickUp",          new StateChange { Value = false, Command = FsmInput.PickUpEnd } },
			    {"OpenDoor",        new StateChange { Value = false, Command = FsmInput.OpenDoorEnd } },
			    {"Props",           new StateChange { Value = false, Command = FsmInput.PropsEnd } },
			    {"Melee",           new StateChange { Value = false, Command = FsmInput.MeleeAttackFinished } },
			    {"ThrowEnd",        new StateChange { Value = false, Command = FsmInput.GrenadeEndFinish } },
			    {"ParachuteOpen1",  new StateChange { Value = false, Command = FsmInput.ParachuteOpen1Finished } },
			    {"Enter",           new StateChange { Value = false, Command = FsmInput.ToProneTransitFinish } },
			    {"Quit",            new StateChange { Value = false, Command = FsmInput.OutProneTransitFinish } },
                {"Climb",           new StateChange { Value = false, Command = FsmInput.GenericActionFinished } },
			    {"Use",             new StateChange { Value = false, Command = FsmInput.BuriedBombFinished } },
			    {"Dismantle",       new StateChange { Value = false, Command = FsmInput.DismantleBombFinished } },
			    {"2InjuredMove",       new StateChange { Value = false, Command = FsmInput.DyingTransitionFinished } },
			    {"EnterLadder",     new StateChange { Value = false, Command = FsmInput.EnterLadderFinished } },
                {"ExitLadder",      new StateChange { Value = false, Command = FsmInput.ExitLadderFinished } },
			    {"TransfigurationStart",      new StateChange { Value = false, Command = FsmInput.TransfigurationStartEnd } },
			    {"TransfigurationFinish",      new StateChange { Value = false, Command = FsmInput.TransfigurationFinishEnd } },
			    {"RageStart",       new StateChange { Value = false, Command = FsmInput.RageStartFinished } },
			    {"RageEnd",         new StateChange { Value = false, Command = FsmInput.RageEndFinished } },
            };

	    private readonly List<string> _animationClipTypes = new List<string>();
	    private List<FsmInput> _limitFsmInputsSource = new List<FsmInput>();
	    private List<FsmInput> _limitFsmInputs = new List<FsmInput>();

	    public FsmInputRelate()
	    {
		    foreach (var relate in _relates)
		    {
			    AddLimits(relate.Value, _limitFsmInputsSource);
		    }
	    }

	    public List<FsmInput> GetFsmInputLimits()
	    {
		    return _limitFsmInputs;
	    }

	    public void InitLimit()
	    {
		    _animationClipTypes.Clear();
		    _limitFsmInputs.Clear();
		    foreach (var input in _limitFsmInputsSource)
		    {
			    _limitFsmInputs.Add(input);
		    }
	    }
	    
	    private static void AddLimits(FsmInput[] limits, List<FsmInput> dest)
	    {
		    foreach (var limit in limits)
		    {
			    var ret = false;
			    foreach (var limitFsmInput in dest)
			    {
				    if(limit != limitFsmInput) continue;
				    ret = true;
				    break;
			    }
			    if(!ret) dest.Add(limit);
		    }
	    }

	    private static void RemoveLimits(FsmInput[] limits, List<FsmInput> dest)
	    {
		    foreach (var limit in limits)
		    {
			    foreach (var limitFsmInput in dest)
			    {
				    if(limit != limitFsmInput) continue;
				    dest.Remove(limit);
				    break;
			    }
		    }
	    }

	    private void GetAllAnimationClipType(Animator animator)
	    {
		    if (null == animator)
		    {
			    Logger.ErrorFormat("Animator is Null");
			    return;
		    }
		    
		    var controller = animator.runtimeAnimatorController;
		    if (null == controller)
		    {
			    Logger.ErrorFormat("RuntimeAnimatorController is Null");
			    return;
		    }

		    var clips = controller.animationClips;
		    if (null == clips)
		    {
			    Logger.ErrorFormat("Clips is Null");
			    return;
		    }
		    
		    foreach (var clip in clips)
		    {
			    var type = _matcher.Match(clip.name);
			    var ret = false;
			    foreach (var animationClipType in _animationClipTypes)
			    {
				    if (!animationClipType.Equals(type)) continue;
					ret = true;
					break;
			    }
			    if(ret) continue;
			    _animationClipTypes.Add(type);
		    }
	    }

	    public void CreateAllLimitFsmInput(Animator animator)
	    {
		    GetAllAnimationClipType(animator);
		    
		    foreach (var animationClipType in _animationClipTypes)
		    {
			    if(!AnimationFinished.ContainsKey(animationClipType)) continue;
			    var input = AnimationFinished[animationClipType].Command;
			    if(!_relates.ContainsKey(input)) continue;
			    RemoveLimits(_relates[input], _limitFsmInputs);
		    }
	    }
    }
	
	public class StateChange
	{
		public bool Value;
		public FsmInput Command;
	}
}
