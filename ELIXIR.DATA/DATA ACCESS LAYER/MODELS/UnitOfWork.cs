using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.CORE.INTERFACES;
using ELIXIR.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.ORDERING_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORT_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.TRANSFORMATION_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.WAREHOUSE_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ELIXIR.DATA.SERVICES;
using Microsoft.AspNetCore.SignalR;
using ELIXIR.DATA.CORE.INTERFACES.CANCELLED_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.LABTEST_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.CANCELLED_ORDERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.LABTEST_REPOSITORY;
using ELIXIR.DATA.CORE.INTERFACES.ORDER_HUB;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly StoreContext _context;
    private readonly IHubContext<OrderHub, IOrderHub> _clients;

    private readonly ILogger _logger;

    //User
    public IUserRepository Users { get; private set; }
    public IModuleRepository Modules { get; private set; }
    public IRoleRepository Roles { get; private set; }

    //SetUp
    public IUomRepository Uoms { get; private set; }
    public ISupplierRepository Suppliers { get; private set; }
    public ILotNameAndCategoriesRepository Lots { get; private set; }
    public ICustomerRepository Customers { get; private set; }
    public IRawMaterialRepository RawMaterials { get; private set; }
    public IReasonRepository Reasons { get; private set; }
    public ITransformationRepository Transforms { get; private set; }

    //Import
    public IImportRepository Imports { get; private set; }

    //QC Receiving
    public IReceivingRepository Receives { get; private set; }

    //Warehouse
    public IWarehouseReceive Warehouse { get; set; }

    //Transformation
    public ITransformationPlanning Planning { get; set; }


    public ITransformationPreparation Preparation { get; set; }

    //Inventory
    public IRawMaterialInventory Inventory { get; set; }
    public IMiscellaneous Miscellaneous  { get; set; }


    //Ordering
    public IOrdering Order { get; set; }
    public IReportRepository Report { get; }
    public ITransactionRepository Transactions { get; set; }
    
    //Checklist
    public IQCChecklist QcChecklist { get; set; }
    
    //LabTest MasterList
    public ILabtestMasterlist LabtestMasterlist { get; set; }
    public IProductTypeRepository ProductType { get; set; }

    //Cancelled Orders
    public ICancelledOrders CancelledOrders { get; set; }

    //public IOrderHub Hub { get; set; }
    
    //LabTest
    
    public ILabTestRepository LaboratoryTest
    {
        get;
        set;
    }

    public IAccountTitleRepository AccountTitle
    {
        get;
        set;
    }
    

    public UnitOfWork(
        StoreContext context,
        ILoggerFactory loggerFactory
        )
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("logs");
        
    //User
        Users = new UserRepository(_context, _logger);
        Modules = new ModuleRepository(_context);
        Roles = new RoleRepository(_context);

    //SetUp
        Uoms = new UomRepository(_context);
        Suppliers = new SupplierRepository(_context);
        Lots = new LotNameAndCategoriesRepository(_context);
        Customers = new CustomerRepository(_context);
        RawMaterials = new RawMaterialRepository(_context);
        Reasons = new ReasonRepository(_context);
        Transforms = new TransformationRepository(_context);
        Transactions = new TransactionRepository(_context);

        //Import
        Imports = new ImportRepository(_context);

        //Receiving 
        Receives = new ReceivingRepository(_context);

        //Warehouse
        Warehouse = new WarehouseReceiveRepository(_context);

        //Transformation Planning
        Planning = new TransformationPlanningRepository(_context);
        Preparation = new TransformationPreparationRepository(_context);

        //Ordering
        Order = new OrderingRepository(_context, _clients);
        
        //Inventory
        Inventory = new RawMaterialInventory(_context);
        Miscellaneous = new MiscellaneousRepository(_context);

        //Reports
        Report = new ReportRepository(_context);
        
        //Checklist 
        QcChecklist = new ChecklistRepository(_context);

        LabtestMasterlist = new LabTestMasterList(_context);
        
        ProductType = new ProductTypeRepository(_context);

        //Cancelled Orders
        CancelledOrders = new CancelledOrdersRepository(_context);

        LaboratoryTest = new LabTestRepository(_context);

        AccountTitle = new AccountTitleRepository(_context);
        
        
        
    }
    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
