using System.ComponentModel.DataAnnotations;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class UserRole_Modules : BaseEntity
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "NotificationId must be numeric")]
        public int RoleId {
            get; 
            set;
        }

        [RegularExpression("^[0-9]*$", ErrorMessage = "NotificationId must be numeric")]
        public int ModuleId { 
            get;
            set;
        }
        public bool IsActive {
            get; 
            set; 
        }

        public virtual UserRole Role { get; set; }
        public virtual Module Module { get; set; }
        
    }
}
