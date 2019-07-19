using NG_Core_Auth.ViewModels.Validations;
using FluentValidation.Validators;

namespace NG_Core_Auth.ViewModels {

    public class CredentialsViewModel {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}