using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE
{
    public interface IMiscellaneous
    {

        Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt);

        Task<bool> GenerateReceiptNumber(GenerateMReceipt receipt);


        Task<bool> AddMiscellaneousIssue(MiscellaneousIssue issue);



        Task<bool> GenerateIssueNumber(GenerateMIssue issue);


    }
}
