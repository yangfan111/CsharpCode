
namespace App.Shared.GameModules
{

   
    public abstract class AbstractPlayerComponentsReference
    {
        protected PlayerEntity entity;

        public AbstractPlayerComponentsReference(PlayerEntity in_entity)
        {
            entity = in_entity;
        }
        protected bool IsInitialized
        {
            get { return entity != null; }
        }
    }
}
