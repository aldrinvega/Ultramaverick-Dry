using ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.TRANSFORMATION_REPOSITORY
{
    public class TransformationPreparationRepository : ITransformationPreparation
    {
        private readonly StoreContext _context;
        public TransformationPreparationRepository(StoreContext context)
        {

            _context = context;


        }

        public async Task<IReadOnlyList<TransformationPreparationDto>> GetAllListOfTransformationByTransformationId(TransformationPlanning planning)
        {
            var prep = (from plan in _context.Transformation_Planning
                        join request in _context.Transformation_Request
                        on plan.Id equals request.TransformId into leftJ
                        from request in leftJ.DefaultIfEmpty()

                        select new TransformationPreparationDto
                        {

                            TranformationId = plan.Id,
                            FormulaCode = plan.ItemCode,
                            FormulaDescription = plan.ItemDescription,
                            FormulaQuantity = plan.Quantity,
                            RawmaterialCode = request.ItemCode,
                            RawmaterialDescription = request.ItemDescription,
                            Uom = plan.Uom,
                            Batch = request.Batch,
                            RawmaterialQuantity = request.Quantity,
                            IsRequirementActive = request.IsActive,
                            IsPrepared = request.IsPrepared


                        }).Where(x => x.TranformationId == planning.Id)
                          .Where(x => x.IsPrepared == false)
                          .Where(x => x.IsRequirementActive == true);

            return await prep.ToListAsync();

        }

        public async Task<bool> AddPreparationMaterials(TransformationPreparation preparation)
        {
            await _context.Transformation_Preparation.AddAsync(preparation);
       
            return true;

        }

        public async Task<bool> PrepareTransformationMaterials(TransformationPreparation preparation)
        {

            decimal max = (decimal)1.05;
            decimal min = (decimal)0.95;

            //var computeStock = await _context.WarehouseReceived.Where(x => x.ItemCode == preparation.ItemCode)
            //                                                 .Where(x => x.IsWarehouseReceive == true)
            //                                                 .Where(x => x.WarehouseItemStatus == true)
            //                                                 .SumAsync(x => x.ActualGood);

            //var computePrepared = await _context.Transformation_Preparation.Where(x => x.ItemCode == preparation.ItemCode)
            //                                                               .Where(x => x.IsActive == true)
            //                                                               .SumAsync(x => x.WeighingScale);

            //var warehousereceived = (from request in _context.Transformation_Request
            //                         where request.IsActive == true
            //                         join warehouse in _context.WarehouseReceived
            //                         on request.ItemCode equals warehouse.ItemCode into leftJ
            //                         from warehouse in leftJ.DefaultIfEmpty()
            //                         where warehouse.ItemCode == preparation.ItemCode
            //                         orderby warehouse.ExpirationDays ascending

            //                         select new RawmaterialDetailsFromWarehouseDto
            //                         {
            //                             WarehouseReceivedId = warehouse.Id,
            //                             Supplier = warehouse.Supplier,
            //                             ItemCode = warehouse.ItemCode,
            //                             ItemDescription = warehouse.ItemDescription,
            //                             ManufacturingDate = warehouse.ManufacturingDate.ToString("MM/dd/yyyy"),
            //                             ExpirationDate = warehouse.Expiration.ToString("MM/dd/yyyy"),
            //                             ExpirationDays = warehouse.ExpirationDays,
            //                             Balance = computeStock - computePrepared,
            //                             QuantityNeeded = request.Quantity,
            //                             Batch = request.Batch

            //                         }).ToListAsync();


            //var getRequirements =       await (from request in _context.Transformation_Request
            //                             where request.IsActive == true
            //                             join warehouse in _context.WarehouseReceived
            //                             on request.ItemCode equals warehouse.ItemCode into leftJ
            //                             from warehouse in leftJ.DefaultIfEmpty()
            //                             orderby warehouse.ExpirationDays ascending

            //                             select new
            //                             {
            //                                 request.TransformId,
            //                                 request.ItemCode,
            //                                 request.ItemDescription,
            //                                 request.Quantity,
            //                                 warehouse.ManufacturingDate,
            //                                 warehouse.Expiration,
            //                                 request.Batch,
            //                                 request.IsActive,
            //                                 warehouse.ExpirationDays,
            //                         //        warehouse.WarehouseItemStatus,
            //                                 warehouse.Id
            //                             })
            //                               .Where(x => x.TransformId == preparation.TransformId)
            //                       //        .Where(x => x.WarehouseItemStatus == true)
            //                               .Where(x => x.ItemCode == preparation.ItemCode)
            //                               .FirstOrDefaultAsync();

            //var updateStatus =  await _context.Transformation_Request.Where(x => x.TransformId == preparation.TransformId)
            //                                                        .Where(x => x.ItemCode == preparation.ItemCode)
            //                                                        .Where(x => x.IsActive == true)
            //                                                        .FirstOrDefaultAsync();

            //var updateWarehouse =  _context.WarehouseReceived.Where(x => x.ItemCode == preparation.ItemCode)
            //                                                      .FirstOrDefaultAsync();


            //var computeWarehousestock = await (from request in _context.Transformation_Request
            //                                   where request.IsActive == true
            //                                   join warehouse in _context.WarehouseReceived
            //                                   on request.ItemCode equals warehouse.ItemCode into leftJ
            //                                   from warehouse in leftJ.DefaultIfEmpty()
            //                                   where warehouse.ItemCode == preparation.ItemCode
            //                                   orderby warehouse.ExpirationDays ascending

            //                                   select new RawmaterialDetailsFromWarehouseDto
            //                                   {
            //                                       WarehouseReceivedId = warehouse.Id,
            //                                       Supplier = warehouse.Supplier,
            //                                       ItemCode = warehouse.ItemCode,
            //                                       ItemDescription = warehouse.ItemDescription,
            //                                       ManufacturingDate = warehouse.ManufacturingDate.ToString("MM/dd/yyyy"),
            //                                       ExpirationDate = warehouse.Expiration.ToString("MM/dd/yyyy"),
            //                                       ExpirationDays = warehouse.ExpirationDays,
            //                                       Balance = warehouse.ActualGood - computePrepared,
            //                                       QuantityNeeded = request.Quantity,
            //                                       Batch = request.Batch,
            //                               //        WarehouseItemStatus = warehouse.WarehouseItemStatus

            //                                   })
            //                               //      .Where(x => x.WarehouseItemStatus == true)
            //                                     .FirstOrDefaultAsync();

            //var getAvailableItem = await _context.WarehouseReceived.Where(x => x.ItemCode == preparation.ItemCode)
            //                                                       .Where(x => x.WarehouseItemStatus == true)
            //                                                       .OrderBy(x => x.ExpirationDays)
            //                                                       .ToListAsync();

            //if (computeWarehousestock.Balance < preparation.WeighingScale)

            //{
            //    preparation.TransformId = getRequirements.TransformId;
            //    preparation.ItemCode = getRequirements.ItemCode;
            //    preparation.ItemDescription = getRequirements.ItemDescription;
            //    preparation.ManufacturingDate = getRequirements.ManufacturingDate;
            //    preparation.ExpirationDate = getRequirements.Expiration;
            //    preparation.Batch = getRequirements.Batch;
            //    preparation.QuantityNeeded = getRequirements.Quantity;
            //    preparation.PreparedDate = DateTime.Now;
            //    preparation.IsActive = true;
            //    preparation.WarehouseId = getRequirements.Id;
            //    preparation.WeighingScale = computeWarehousestock.Balance;


                await AddPreparationMaterials(preparation);
            //}

            //else
            //{
                 
                //preparation.TransformId = getRequirements.TransformId;
                //preparation.ItemCode = getRequirements.ItemCode;
                //preparation.ItemDescription = getRequirements.ItemDescription;
                //preparation.ManufacturingDate = getRequirements.ManufacturingDate;
                //preparation.ExpirationDate = getRequirements.Expiration;
                //preparation.Batch = getRequirements.Batch;
                //preparation.QuantityNeeded = getRequirements.Quantity;
                //preparation.PreparedDate = DateTime.Now;
                //preparation.IsActive = true;
                //preparation.WarehouseId = getRequirements.Id;

                //if (getRequirements.Quantity * (decimal)min > preparation.WeighingScale ||
                //                getRequirements.Quantity * (decimal)max < preparation.WeighingScale)

            //    await AddPreparationMaterials(preparation);

            //    updateStatus.IsPrepared = true;


            //}

            //preparation.TransformId = getRequirements.TransformId;
            //preparation.ItemCode = getRequirements.ItemCode;
            //preparation.ItemDescription = getRequirements.ItemDescription;
            //preparation.ManufacturingDate = getRequirements.ManufacturingDate;
            //preparation.ExpirationDate = getRequirements.Expiration;
            //preparation.Batch = getRequirements.Batch;
            //preparation.QuantityNeeded = getRequirements.Quantity;
            //preparation.PreparedDate = DateTime.Now;
            //preparation.IsActive = true;
            //preparation.WarehouseId = getRequirements.Id;

            //if (getRequirements.Quantity * (decimal)min > preparation.WeighingScale ||
            //                getRequirements.Quantity * (decimal)max < preparation.WeighingScale)
            //    return false;

            return true;
        } 

        public async Task<bool> UpdatePrepareStatusInRequest(int id)
        {
            var validate = await _context.Transformation_Request
                                                          .Where(x => x.TransformId == id)
                                                          .Where(x => x.IsPrepared == false)
                                                          .Where(x => x.IsActive == true)
                                                          .ToListAsync();

            var getRequest = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                   .FirstOrDefaultAsync();
            if (validate.Count == 0)
                getRequest.IsPrepared = true;

            return true;
        }

        public async Task<bool> ValidatePreparedMaterials(int id, string code)
        {
            var validate = await _context.Transformation_Request.Where(x => x.TransformId == id)
                                                          .Where(x => x.ItemCode == code)
                                                          .Where(x => x.IsPrepared == false)
                                                          .Where(x => x.IsActive == true)
                                                          .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllListOfTransformationRequestForMixing()
        {
            return await _context.Transformation_Planning.Select(planning => new TransformationPlanningDto
            {
                Id = planning.Id,
                ItemCode = planning.ItemCode,
                ItemDescription = planning.ItemDescription,
                Uom = planning.Uom,
                Version = planning.Version,
                ProdPlan = planning.ProdPlan.ToString("MM/dd/yyyy"),
                Batch = planning.Batch,
                Quantity = planning.Quantity,
                AddedBy = planning.AddedBy,
                Status = planning.Status,
                DateAdded = planning.DateAdded.ToString("MM/dd/yyyy"),
                IsPrepared = planning.IsPrepared,
                IsApproved = planning.Is_Approved != null,
                StatusRemarks = planning.StatusRequest
            }).Where(x => x.Status == true)
              .Where(x => x.IsApproved == true)
              .Where(x => x.IsPrepared == false)
              .Where(x => x.StatusRemarks == "Approved")
              .ToListAsync();
        }

        public async Task<bool> ValidateIfApproved(int id)
        {
            var validate = await _context.Transformation_Planning.Where(x => x.Id == id)
                                                                 .Where(x => x.Status == true)
                                                                 .Where(x => x.Is_Approved == true)
                                                                 .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<TransformationMixingRequirements>> GetAllRequirementsForMixing(int id)
        {
            var requirements =  _context.Transformation_Preparation.Select(mixing => new TransformationMixingRequirements
            {
                TransformId = mixing.TransformId,
                ItemCode = mixing.ItemCode,
                ItemDescription = mixing.ItemDescription,
                Batch = mixing.Batch,
                QuantityBatch = mixing.QuantityNeeded,
                TotalQuantity = mixing.QuantityNeeded,
                WeighingScale = mixing.WeighingScale

            });

            return await requirements.ToListAsync();
                                
        }

        public async Task<bool> AddMixingTransformation(WarehouseReceiving warehouse)
        {
            await _context.WarehouseReceived.AddAsync(warehouse);

            return true;
        }

        public async Task<bool> FinishedMixedMaterialsForWarehouse(WarehouseReceiving warehouse)
        {
            DateTime dateNow = DateTime.Now;

       //     var mixing = await _context.Transformation_Planning.Where(x => x.Id == warehouse.TransformId)
                  //                                             .FirstOrDefaultAsync();

            //var countBatch = await _context.WarehouseReceived.Where(x => x.TransformId == warehouse.TransformId)
            //                                                 .ToListAsync();

            //if (mixing.Batch <= countBatch.Count)
            //    return false;

            //warehouse.TransformId = mixing.Id;
            //warehouse.ManufacturingDate = DateTime.Now;
            //warehouse.ItemCode = mixing.ItemCode;
            //warehouse.ItemDescription = mixing.ItemDescription;
            //warehouse.Uom = mixing.Uom;
            //warehouse.Supplier = "RDF";
            //warehouse.ReceivingDate = DateTime.Now;
            //warehouse.TransactionType = "Transformation";
            //warehouse.BatchCount = countBatch.Count + 1;
            //warehouse.ActualGood = mixing.Quantity;
            //warehouse.IsWarehouseReceive = true;
            //warehouse.IsActive = true;
            //warehouse.ExpirationDays = warehouse.Expiration.Subtract(dateNow).Days;

            //await AddMixingTransformation(warehouse);

            return true;

        }

        public async Task<RawmaterialDetailsFromWarehouseDto> GetReceivingDetailsForRawmaterials(string code)
        {

            //var computeStock = await _context.WarehouseReceived.Where(x => x.ItemCode == code)
            //                                                   .Where(x => x.IsWarehouseReceive == true)
            //                                                   .Where(x => x.WarehouseItemStatus == true)
            //                                                   .SumAsync(x => x.ActualGood);

            //var computePrepared = await _context.Transformation_Preparation.Where(x => x.ItemCode == code)
            //                                                               .Where(x => x.WarehouseId == id)
            //                                                               .Where(x => x.IsActive == true)
            //                                                               .SumAsync(x => x.WeighingScale);


            //var totalRequest = _context.Transformation_Preparation.GroupBy(x => new
            //{
            //    x.ItemCode,
            //    x.IsActive,
            //    x.WarehouseId,

            //}).Select(x => new MaterialRequirements
            //{
            //    ItemCode = x.Key.ItemCode,
            //    Reserve = x.Sum(x => x.WeighingScale),
            //    IsActive = x.Key.IsActive,
            //    WarehouseId = x.Key.WarehouseId

            //}).Where(x => x.IsActive == true);

            //var computePrepared = await _context.Transformation_Preparation.Where(x => x.ItemCode == code)
            //                                                               .Where(x => x.IsActive == true)
            //                                                               .SumAsync(x => x.WeighingScale);

            var warehouseStock = (from warehouse in _context.WarehouseReceived
                                  join req in _context.Transformation_Preparation
                                  on warehouse.Id equals req.WarehouseId into leftJ

                                  from req in leftJ.DefaultIfEmpty()

                                  group req by new
                                  {

                                      warehouse.Id,
                                      warehouse.Supplier,
                                      warehouse.ItemCode,
                                      warehouse.ItemDescription,
                                      warehouse.ManufacturingDate,
                                      warehouse.Expiration,
                                      warehouse.ExpirationDays,
                                 //     warehouse.WarehouseItemStatus,
                                      warehouse.ActualGood

                                  } into total

                                  select new
                                  {

                                      total.Key.Id,
                                      total.Key.Supplier,
                                      total.Key.ItemCode,
                                      total.Key.ItemDescription,
                                      total.Key.ManufacturingDate,
                                      total.Key.Expiration,
                                      total.Key.ExpirationDays,
                                      Reserve = total.Sum(x => x.WeighingScale),
                                //      total.Key.WarehouseItemStatus,
                                      total.Key.ActualGood

                                  });

            var warehousereceived = (from request in _context.Transformation_Request
                                     where request.IsActive == true && request.IsPrepared == false                         
                                     join warehouse in warehouseStock
                                     on request.ItemCode equals warehouse.ItemCode
    
                         //            where warehouse.ItemCode == code && warehouse.WarehouseItemStatus == true
                                                           
                                     group warehouse by new
                                     {
                                        
                                         warehouse.Id, 
                                         warehouse.Supplier,
                                         warehouse.ItemCode,
                                         warehouse.ItemDescription,
                                         warehouse.ManufacturingDate,
                                         warehouse.Expiration,
                                         warehouse.ExpirationDays,
                                 //        warehouse.WarehouseItemStatus,
                                         warehouse.ActualGood,                                     
                                         request.Quantity,
                                         request.Batch,                            
                                         request.IsPrepared,
                                         warehouse.Reserve,
                                         request.TransformId

                                     } into total

                                     orderby total.Key.ExpirationDays ascending

                                     select new RawmaterialDetailsFromWarehouseDto
                                     {
                                         TransformId = total.Key.TransformId,
                                         WarehouseReceivedId = total.Key.Id,
                                         Supplier = total.Key.Supplier,
                                         ItemCode = total.Key.ItemCode,
                                         ItemDescription = total.Key.ItemDescription,
                                         ManufacturingDate = total.Key.ManufacturingDate.ToString("MM/dd/yyyy"),
                                         ExpirationDate = total.Key.Expiration.ToString("MM/dd/yyyy"),
                                         ExpirationDays = total.Key.ExpirationDays,                          
                                         QuantityNeeded = total.Key.Quantity,
                                         Batch = total.Key.Batch,
                                         IsPrepared = total.Key.IsPrepared,
                                         Balance = total.Key.ActualGood - total.Key.Reserve

                                     });

             return await warehousereceived.FirstOrDefaultAsync();

        }

        public async Task<bool> UpdatedWarehouseStock(string code)
        {
   
            var warehouseStock = await _context.WarehouseReceived.Where(x => x.ItemCode == code)
                                                             //    .Where(x => x.WarehouseItemStatus == true)
                                                                 .OrderBy(x => x.ExpirationDays)                                                           
                                                                 .FirstOrDefaultAsync();

            var computePrepared = await _context.Transformation_Preparation.Where(x => x.ItemCode == code)
                                                                           .Where(x => x.IsActive == true)
                                                                           .SumAsync(x => x.WeighingScale);

            var warehousereceived = await (from request in _context.Transformation_Request
                                     where request.IsActive == true
                                     join warehouse in _context.WarehouseReceived
                                     on request.ItemCode equals warehouse.ItemCode into leftJ
                                     from warehouse in leftJ.DefaultIfEmpty()
                                     where warehouse.ItemCode == code
                                     orderby warehouse.ExpirationDays ascending

                                     select new RawmaterialDetailsFromWarehouseDto
                                     {
                                         WarehouseReceivedId = warehouse.Id,
                                         Supplier = warehouse.Supplier,
                                         ItemCode = warehouse.ItemCode,
                                         ItemDescription = warehouse.ItemDescription,
                                         ManufacturingDate = warehouse.ManufacturingDate.ToString("MM/dd/yyyy"),
                                         ExpirationDate = warehouse.Expiration.ToString("MM/dd/yyyy"),
                                         ExpirationDays = warehouse.ExpirationDays,
                                         Balance = warehouse.ActualGood - computePrepared,
                                         QuantityNeeded = request.Quantity,
                                         Batch = request.Batch,
                                    //     WarehouseItemStatus = warehouse.WarehouseItemStatus

                                     }).Where(x => x.WarehouseItemStatus == true)
                                       .FirstOrDefaultAsync();

          //  if (warehousereceived.Balance == 0)
             //   warehouseStock.WarehouseItemStatus = false;

            return false;

        }

        public async Task<IReadOnlyList<TransformationPlanningDto>> GetAllTransformationFormulaInformation()
        {

            var transformplanning =  (from planning in _context.Transformation_Planning
                                     join warehouse in _context.WarehouseReceived
                                     on planning.ItemCode equals warehouse.ItemCode into leftJ
                                     from warehouse in leftJ.DefaultIfEmpty()

                                     group warehouse by new
                                     {
                                     
                                         planning.Id,
                                         planning.ItemCode,
                                         planning.ItemDescription,
                                         planning.Quantity,
                                         planning.Batch,

                                     } into total

                                 select new TransformationPlanningDto
                                 {
                                     Id = total.Key.Id,
                                     ItemCode = total.Key.ItemCode,
                                     ItemDescription = total.Key.ItemDescription,
                                     Quantity = Math.Round(Convert.ToDecimal(total.Key.Quantity * total.Key.Batch), 2),
                                     Batch = total.Key.Batch,
                                     WarehouseStock = total.Sum(x => x.ActualGood),
                         
                                 });

            return await transformplanning.ToListAsync();


        }

        public async Task<decimal> ValidatePreparedItems(TransformationPreparation preparation)
        {
            var validate =   _context.Transformation_Preparation.Where(x => x.TransformId == preparation.TransformId)
                                                                .Where(x => x.ItemCode == preparation.ItemCode)
                                                                .SumAsync(x => x.WeighingScale);

            var total = validate;

            return await total;

        }

        public async Task<IReadOnlyList<ForTesting>> GetAllAvailableStocks()
        {

            //var warehouseStock = (from warehouse in _context.WarehouseReceived
            //                      join req in _context.Transformation_Preparation
            //                      on warehouse.Id equals req.WarehouseId into leftJ

            //                      from req in leftJ.DefaultIfEmpty()

            //                      group req by new
            //                      {

            //                          warehouse.Id,
            //                          warehouse.Supplier,
            //                          warehouse.ItemCode,
            //                          warehouse.ItemDescription,
            //                          warehouse.ManufacturingDate,
            //                          warehouse.Expiration,
            //                          warehouse.ExpirationDays,
            //                          warehouse.WarehouseItemStatus,
            //                          warehouse.ActualGood

            //                      } into total

            //                      select new
            //                      {

            //                          total.Key.Id,
            //                          total.Key.Supplier,
            //                          total.Key.ItemCode,
            //                          total.Key.ItemDescription,
            //                          total.Key.ManufacturingDate,
            //                          total.Key.Expiration,
            //                          total.Key.ExpirationDays,
            //                          Reserve = total.Sum(x => x.WeighingScale),
            //                          total.Key.WarehouseItemStatus,
            //                          total.Key.ActualGood

            //                      });

            //var warehousereceived = (from request in _context.Transformation_Request
            //                         where request.IsActive == true && request.IsPrepared == false
            //                         join warehouse in warehouseStock
            //                         on request.ItemCode equals warehouse.ItemCode

            //                    //     where warehouse.ItemCode == code && warehouse.WarehouseItemStatus == true

            //                         group warehouse by new
            //                         {

            //                             warehouse.Id,
            //                             warehouse.Supplier,
            //                             warehouse.ItemCode,
            //                             warehouse.ItemDescription,
            //                             warehouse.ManufacturingDate,
            //                             warehouse.Expiration,
            //                             warehouse.ExpirationDays,
            //                             warehouse.WarehouseItemStatus,
            //                             warehouse.ActualGood,
            //                             request.Quantity,
            //                             request.Batch,
            //                             request.IsPrepared,
            //                             warehouse.Reserve,
            //                             request.TransformId

            //                         } into total

            //                         orderby total.Key.ExpirationDays ascending

            //                         select new RawmaterialDetailsFromWarehouseDto
            //                         {
            //                             TransformId = total.Key.TransformId,
            //                             WarehouseReceivedId = total.Key.Id,
            //                             Supplier = total.Key.Supplier,
            //                             ItemCode = total.Key.ItemCode,
            //                             ItemDescription = total.Key.ItemDescription,
            //                             ManufacturingDate = total.Key.ManufacturingDate.ToString("MM/dd/yyyy"),
            //                             ExpirationDate = total.Key.Expiration.ToString("MM/dd/yyyy"),
            //                             ExpirationDays = total.Key.ExpirationDays,
            //                             QuantityNeeded = total.Key.Quantity,
            //                             Batch = total.Key.Batch,
            //                             WarehouseItemStatus = total.Key.WarehouseItemStatus,
            //                             IsPrepared = total.Key.IsPrepared,
            //                             Balance = total.Key.ActualGood - total.Key.Reserve
            //                             //   PreparationBalance = total.Sum(x => x.)

            //                         });

            //return await warehousereceived.ToListAsync();

            var totalOut = (from warehouse in _context.WarehouseReceived
                       join req in _context.Transformation_Preparation
                       on warehouse.Id equals req.WarehouseId into leftJ

                       from req in leftJ.DefaultIfEmpty()

                       group req by new
                       {

                           warehouse.Id,
                           warehouse.Supplier,
                           warehouse.ItemCode,
                           warehouse.ItemDescription,
                           warehouse.ManufacturingDate,
                           warehouse.Expiration,
                           warehouse.ExpirationDays,
                         //  warehouse.WarehouseItemStatus,
                           warehouse.ActualGood

                       } into total

                       select new
                       {

                           total.Key.Id,
                           total.Key.Supplier,
                           total.Key.ItemCode,
                           total.Key.ItemDescription,
                           total.Key.ManufacturingDate,
                           total.Key.Expiration,
                           total.Key.ExpirationDays,
                           Reserve = total.Sum(x => x.WeighingScale),
                      //     total.Key.WarehouseItemStatus,
                           total.Key.ActualGood

                       });

            var warehousestock = _context.WarehouseReceived.OrderBy(x => x.ExpirationDays)                                                    
                                                      .Select(x => new ForTesting
            {
                ReceivedId = x.Id,
                ItemCode = x.ItemCode,
                ExpirationDays = x.ExpirationDays,
                In = x.ActualGood,
             //   Out  = x.ActualGood

            });


            return await warehousestock.ToListAsync();

                                                              
        }

        public async Task<IReadOnlyList<ItemStocks>> GetAllRemainingStocksPerReceivingId(string itemcode)
        {
            var totalout = _context.Transformation_Preparation.GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var totalRemaining = (from totalIn in _context.WarehouseReceived
                                  join totalOut in totalout

                                  on totalIn.Id equals totalOut.WarehouseId
                                  into leftJ
                                  from totalOut in leftJ.DefaultIfEmpty()

                                  group totalOut by new
                                  {

                                      totalIn.Id,
                                      totalIn.ItemCode,
                                      totalIn.ActualGood,
                                      totalIn.ExpirationDays
                                      
                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.Id,
                                      ItemCode = total.Key.ItemCode,
                                      ExpirationDays = total.Key.ExpirationDays,
                                      In = total.Key.ActualGood,
                                      Out = total.Sum(x => x.Out),
                                      Remaining = total.Key.ActualGood - total.Sum(x => x.Out)
                                  });

            return await totalRemaining.Where(x => x.ItemCode == itemcode)
                                       .Where(x => x.Remaining != 0)
                                       .ToListAsync();

        }
    }
}
