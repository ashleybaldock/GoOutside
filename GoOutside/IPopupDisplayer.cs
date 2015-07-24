namespace GoOutside
{
    public interface IPopupDisplayer
    {
        bool CanShow();
        bool CanHide();
        void Show();
        void Hide();
    }
}