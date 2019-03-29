namespace Core.Ui
{
    public interface IUiGroupController
    {
        void SetUiState(bool isShow);
        //UI是否可用
        bool Enable { set; get; }
    }
}