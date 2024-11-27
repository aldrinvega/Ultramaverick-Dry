namespace ELIXIR.DATA.DTOs.USER_DTOs
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string MainMenu { get; set; }
        public int MainMenuId { get; set; }
        public string ModuleName { get; set; }
        public string SubMenuName { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
        public string ModuleStatus { get; set; }


    }
}
