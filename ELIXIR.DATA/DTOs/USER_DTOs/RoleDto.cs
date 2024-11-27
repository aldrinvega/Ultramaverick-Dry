namespace ELIXIR.DATA.DTOs.USER_DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DateModified { get; set; }
        public string Reason { get; set; }


    }
}
