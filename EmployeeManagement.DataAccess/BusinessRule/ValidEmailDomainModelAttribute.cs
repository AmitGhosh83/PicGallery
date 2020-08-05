using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PicGallery.DataAccess.BusinessRule
{
    class ValidEmailDomainModelAttribute: ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainModelAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            var inputemailstring = value.ToString().Split("@");
            return inputemailstring[1].ToUpper() == allowedDomain.ToUpper();
        }
    }
}
