namespace PZ3_NetworkService
{
    public abstract class ValidationBase : BindableBase
    {
        public ValidationErrors ValidationErrors { get; set; }

        public bool IsValid { get; private set; }

        protected ValidationBase()
        {
            ValidationErrors = new ValidationErrors();
        }

        protected abstract void ValidateSelf();

        public void Validate()
        {
            ValidationErrors.Clear();
            ValidateSelf();
            IsValid = ValidationErrors.IsValid;
            OnPropertyChanged("IsValid");
            OnPropertyChanged("ValidationErrors");
        }
    }
}
