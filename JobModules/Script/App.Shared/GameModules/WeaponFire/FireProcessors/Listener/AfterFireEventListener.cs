using App.Shared.Components;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon.Behavior;
using App.Shared.Player.Events;
using Core.Event;
using Core.Utils;
using Utils.CharacterState;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="AfterFireEventListener" />
    /// </summary>
    public class AfterFireEventListener : IAfterFireProcess
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AfterFireEventListener));

        private ClientEffectContext _context;

        private IEntityIdGenerator _idGenerator;

        private WeaponEffectConfig _config;
        
        private DefaultWeaponEffectConfig DefaultCfg{get{return _config as DefaultWeaponEffectConfig;}}

        public AfterFireEventListener(ClientEffectContext context, IEntityIdGenerator idGenerator, WeaponEffectConfig config)
        {
            _config = config;
            _context = context;
            _idGenerator = idGenerator;
        }

        public void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
           
            AddFireEvent(controller);
           // AddPullBoltEvent(controller);
            AddLocalEffect(controller);
        }

        private void AddLocalEffect(PlayerWeaponController controller)
        {
            if (!SharedConfig.IsServer && DefaultCfg != null)
            {
                //枪口火花
                CreateMuzzleSparkEffect(controller);
                //子弹抛壳
                CreateBulletDropEffect(controller);
             //   CreatePullBoltEffect(controller);
            }
        }
        private void AddFireEvent(PlayerWeaponController controller)
        {
            AudioGrp_ShotMode shotMode;
            if (controller.HeldWeaponAgent.HasSilencerPart)
                shotMode = AudioGrp_ShotMode.Silencer;
            else
                shotMode = ((EFireMode)controller.HeldWeaponAgent.BaseComponent.RealFireModel).ToAudioGrpShotMode();
                var position = controller.RelatedAppearence.WeaponHandObject().transform.position ;
                if(controller.AudioController != null)
                    controller.AudioController.PlayFireAudio(controller.HeldConfigId, shotMode, position);
                
                
             
            
        }
        
        private void AddPullBoltEvent(PlayerWeaponController controller)
        {
          

            if (controller.RelatedLocalEvents != null)
            {
                var e = EventInfos.Instance.Allocate(EEventType.PullBolt, false);
                controller.RelatedLocalEvents.AddEvent(e);
            }
        }

        private void CreateBulletDropEffect(PlayerWeaponController controller)
        {
            if (DefaultCfg.BulletDrop < 1)
                return;
            //For test
          //  var ejectTrans = controller.RelatedBones.GetLocation(SpecialLocation.MuzzleEffectPosition, 
           //     controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
         var ejectTrans = controller.RelatedBones.GetLocation(SpecialLocation.EjectionLocation, controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != ejectTrans)
            {
                ClientEffectFactory.CreateBulletDrop(_context, _idGenerator, controller.Owner, ejectTrans.position, controller.RelatedOrientation.Yaw, 
                    controller.RelatedOrientation.Pitch, DefaultCfg.BulletDrop,controller.HeldConfigId,AudioGrp_FootMatType.Concrete);
            }
            else
            {
                Logger.Error("Get ejectionLocation location failed");
            }
        }

        private void CreateMuzzleSparkEffect(PlayerWeaponController controller)
        {
            if (null == controller ||DefaultCfg.Spark<1)
                return;
            var muzzleTrans = controller.RelatedBones.GetLocation(SpecialLocation.MuzzleEffectPosition, 
                controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != muzzleTrans)
            {
             //   Logger.Info("CreateMuzzleSparkEffct Once");
                ClientEffectFactory.CreateMuzzleSparkEffct(_context, _idGenerator, controller.Owner, muzzleTrans, controller.RelatedOrientation.Pitch, controller.RelatedOrientation.Yaw, DefaultCfg.Spark);
            }
            else
            {
                Logger.Error("Get muzzleLocation location failed");
            }
        }

        private void CreatePullBoltEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }
            
            var effectPos = PlayerEntityUtility.GetThrowingEmitPosition(controller);
            float effectYaw = (controller.RelatedOrientation.Yaw + 90) % 360;
            float effectPitch = controller.RelatedOrientation.Pitch;
            int effectId = 32;
            int effectTime = 3000;

            ClientEffectFactory.CreateGrenadeExplosionEffect(_context, _idGenerator,
                            controller.Owner, effectPos, effectYaw, effectPitch, effectId, effectTime, EClientEffectType.PullBolt);
        }
    }
}
