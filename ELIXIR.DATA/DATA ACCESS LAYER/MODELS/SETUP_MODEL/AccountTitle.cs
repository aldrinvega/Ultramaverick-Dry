using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class AccountTitle
    {
        public int Id
        {
            get;
            set;
        }

        public int AccountTitleId
        {
            get;
            set;
        }

        public string AccountTitleCode
        {
            get;
            set;
        }

        public string AccountTitleName
        {
            get;
            set;
        }

        public DateTime CreatedAt
        {
            get;
            set;
        } = DateTime.Now;

        public DateTime? UpdatedAt
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        } = false;
    }
}