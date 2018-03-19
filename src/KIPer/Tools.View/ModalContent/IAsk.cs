namespace Tools.View.ModalContent
{
    public interface IAsk
    {
        /// <summary>
        /// Результат от пользователя
        /// </summary>
        bool IsAgree { get; set; }
    }
}