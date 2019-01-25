namespace Core.WeaponAnimation
{
    public interface IWeaponAnimProgress
    {
        string FirstPersonAnimName { get; set; }
        float FirstPersonAnimProgress { get; set; }
        string ThirdPersonAnimName { get; set; }
        float ThirdPersonAnimProgress { get; set; }
    }
}