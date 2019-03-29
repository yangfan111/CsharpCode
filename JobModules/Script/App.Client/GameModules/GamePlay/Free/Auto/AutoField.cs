namespace Assets.Sources.Free.Auto
{
    public class AutoField
    {
        public string field;
        public IAutoValue auto;
		
        public AutoField(string field, IAutoValue auto)
        {
            this.field = field;
            this.auto = auto;
        }
    }


}
