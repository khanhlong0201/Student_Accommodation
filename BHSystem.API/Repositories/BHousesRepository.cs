using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Xml.Linq;

namespace BHSystem.API.Repositories
{
    public interface IBHousesRepository : IGenericRepository<BoardingHouses>
    {
        Task<IEnumerable<BoardingHouseModel>> GetAllAsync(int pUserId, bool pIsAdmin);
        Task<CliResponseModel<CliBoardingHouseModel>?> GetDataPagination(BHouseSearchModel oSearch);
        Task<CliBoardingHouseModel?> GetDataBHDetail(int pRoomId);
    }
    public class BHousesRepository: GenericRepository<BoardingHouses>, IBHousesRepository
    {
        private List<string> ListStatus = new List<string>() { "Phê duyệt", "Phòng trống" };
        public BHousesRepository(ApplicationDbContext context) : base(context) { }
        public async Task<IEnumerable<BoardingHouseModel>> GetAllAsync(int pUserId, bool pIsAdmin)
        {
            var result = await (from d in _context.BoardingHouses
                                join c in _context.Wards on d.Ward_Id equals c.Id
                                join b in _context.Distincts on c.Distincts_Id equals b.Id
                                join a in _context.Citys on b.City_Id equals a.Id
                                join u in _context.Users on d.User_Id equals u.UserId
                                where d.IsDeleted == false && (pIsAdmin == true || (d.User_Id== pUserId && pIsAdmin == false))
                                select new BoardingHouseModel()
                                {
                                    Id = d.Id,
                                    Name = d.Name,
                                    Adddress = d.Adddress,
                                    Image_Id = d.Image_Id,
                                    Ward_Id = d.Ward_Id,
                                    Ward_Name = c.Name,
                                    Distinct_Name = b.Name,
                                    City_Name = a.Name,
                                    City_Id = a.Id,
                                    Distinct_Id = b.Id,
                                    Qty = d.Qty,
                                    UserName = u.FullName,
                                    Phone = u.Phone,
                                    Date_Create = d.Date_Create
                                }).OrderByDescending(m => m.Date_Create).ToListAsync();
            return result;
        }

        /// <summary>
        /// lấy danh sách phòng theo các điều kiện tìm kiếm
        /// </summary>
        /// <param name="oSearch"></param>
        /// <returns></returns>
        public async Task<CliResponseModel<CliBoardingHouseModel>?> GetDataPagination(BHouseSearchModel oSearch)
        {
            bool isSearchPrice = !string.IsNullOrEmpty(oSearch.KeyPrice);
            decimal priceFrom = 0;
            decimal priceTo = 0;
            bool isSearchAcreage = !string.IsNullOrEmpty(oSearch.KeyAcreage);
            decimal acreageFrom = 0;
            decimal acreageTo = 0;
            if (isSearchPrice) outPrice(oSearch.KeyPrice+ "", out priceFrom, out priceTo);
            if (isSearchAcreage) outAcreage(oSearch.KeyAcreage + "", out acreageFrom, out acreageTo);
            var result = await (from t0 in _dbSet
                                join t1 in _context.Wards on t0.Ward_Id equals t1.Id
                                join t2 in _context.Distincts on t1.Distincts_Id equals t2.Id
                                join t3 in _context.Citys on t2.City_Id equals t3.Id
                                join t4 in _context.Users on t0.User_Id equals t4.UserId
                                join t5 in _context.Rooms on t0.Id equals t5.Boarding_House_Id
                                where t0.IsDeleted == false && ListStatus.Contains(t5.Status)
                                && (oSearch.CityId <=0 || (oSearch.CityId > 0 && t3.Id == oSearch.CityId))
                                && (oSearch.DistinctId <=0 || (oSearch.DistinctId > 0 && t2.Id == oSearch.DistinctId))
                                && (oSearch.WardId <=0 || (oSearch.WardId > 0 && t0.Ward_Id == oSearch.WardId))
                                && (isSearchPrice == false || (isSearchPrice && t5.Price >= priceFrom && t5.Price < priceTo))
                                && (isSearchAcreage == false || (isSearchAcreage && (t5.Length * t5.Width) >= acreageFrom && (t5.Length * t5.Width) < acreageTo))
                                select new CliBoardingHouseModel()
                                {
                                    Id = t0.Id,
                                    RoomId = t5.Id,
                                    BHouseName = t0.Name,
                                    RoomName = t5.Name,
                                    Addresss = t0.Adddress,
                                    RoomAddresss = t5.Address,
                                    CityName = t3.Name,
                                    DistinctName = t2.Name,
                                    WardName = t1.Name,
                                    Phone = t4.Phone,
                                    FullName = t4.FullName,
                                    DateCreate = t5.Date_Create,
                                    Acreage = Math.Round(t5.Length * t5.Width, 2),
                                    Price = t5.Price,
                                    Desciption = t5.Description,
                                    ImageUrlBHouse = _context.ImagesDetails.Where(m => m.Image_Id == t0.Image_Id).Select(m=>m.File_Path).FirstOrDefault(),
                                    //lấy ds ảnh
                                    ListImages = _context.ImagesDetails.Where(m=>m.Image_Id == t5.Image_Id).Select(m=>m.File_Path).ToList(),
                                    //
                                })
                                .OrderByDescending(m=>m.DateCreate).ToListAsync();
            if (result == null) return default;
  
            #region pagination
            CliResponseModel<CliBoardingHouseModel> response = new CliResponseModel<CliBoardingHouseModel>();
            int totalRecord = result.Count();
            var pagination = new PaginationModel
            {
                Count = totalRecord,
                CurrentPage = oSearch.Page,
                Pagsize = oSearch.Limit,
                TotalPage = (int)Math.Ceiling(decimal.Divide(totalRecord, oSearch.Limit)),
                IndexOne = ((oSearch.Page - 1) * oSearch.Limit + 1),
                IndexTwo = (((oSearch.Page - 1) * oSearch.Limit + oSearch.Limit) <= totalRecord ? ((oSearch.Page - 1) * oSearch.Limit * oSearch.Limit) : totalRecord)
            };
            response.ListData = result.Skip((oSearch.Page - 1) * oSearch.Limit).Take(oSearch.Limit).ToList();
            response.Pagination = pagination;
            #endregion
            return response;
        }

        /// <summary>
        /// lấy chi tiết phòng
        /// </summary>
        /// <param name="pRoomId"></param>
        /// <returns></returns>
        public async Task<CliBoardingHouseModel?> GetDataBHDetail(int pRoomId)
        {
            var result = await (from t0 in _dbSet
                          join t1 in _context.Wards on t0.Ward_Id equals t1.Id
                          join t2 in _context.Distincts on t1.Distincts_Id equals t2.Id
                          join t3 in _context.Citys on t2.City_Id equals t3.Id
                          join t4 in _context.Users on t0.User_Id equals t4.UserId
                          join t5 in _context.Rooms on t0.Id equals t5.Boarding_House_Id
                          where t0.IsDeleted == false && t5.Id == pRoomId
                          select new CliBoardingHouseModel()
                          {
                              Id = t0.Id,
                              RoomId = t5.Id,
                              BHouseName = t0.Name,
                              RoomName = t5.Name,
                              Addresss = t0.Adddress,
                              RoomAddresss = t5.Address,
                              CityName = t3.Name,
                              DistinctName = t2.Name,
                              WardName = t1.Name,
                              Phone = t4.Phone,
                              FullName = t4.FullName,
                              DateCreate = t5.Date_Create,
                              Acreage = Math.Round(t5.Length * t5.Width, 2),
                              Price = t5.Price,
                              Desciption = t5.Description,
                              //ImageUrlBHouse = _context.ImagesDetails.Where(m => m.Image_Id == t0.Image_Id).Select(m => m.File_Path).FirstOrDefault(),
                              //lấy ds ảnh
                              ListImages = _context.ImagesDetails.Where(m => m.Image_Id == t5.Image_Id).Select(m => m.File_Path).ToList(),
                              //
                          }).FirstOrDefaultAsync();
            return result;
        }

        #region Private Functions
        private void outPrice(string key, out decimal from, out decimal to)
        {
            from = 0;
            to = 100000000;
            switch(key)
            {
                case "1":
                    from = 0;
                    to = 1000000;
                    break;
                case "2":
                    from = 1000000;
                    to = 3000000;
                    break;
                case "3":
                    from = 3000000;
                    to = 5000000;
                    break;
                case "4":
                    from = 5000000;
                    to = 7000000;
                    break;
                case "5":
                    from = 7000000;
                    to = 10000000;
                    break;
                case "6":
                    from = 10000000;
                    to = 15000000;
                    break;
                case "7":
                    from = 15000000;
                    to = 100000000000;
                    break;
                default:
                    from = 0;
                    to = 100000000;
                    break;
            }    
        }

        private void outAcreage(string key, out decimal from, out decimal to)
        {
            from = 0;
            to = 500;
            switch (key)
            {
                case "1":
                    from = 0;
                    to = 20;
                    break;
                case "2":
                    from = 20;
                    to = 30;
                    break;
                case "3":
                    from = 30;
                    to = 40;
                    break;
                case "4":
                    from = 40;
                    to = 50;
                    break;
                case "5":
                    from = 50;
                    to = 500;
                    break;
                default:
                    from = 0;
                    to = 500;
                    break;
            }
        }    
        #endregion
    }
}
