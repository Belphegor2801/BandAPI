using AutoMapper;
using BandAPI.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace BandAPI.Data
{
    public class DbHandler<TDbModel, TResultModel, TQueryModel>
        where TDbModel : class
        where TResultModel : class
        where TQueryModel : PaginationRequest
    {
        public static DbHandler<TDbModel, TResultModel, TQueryModel> Instance { get; } =
            new DbHandler<TDbModel, TResultModel, TQueryModel>();


        public async Task<Response> GetPageAsync(Expression<Func<TDbModel, bool>> predicate, TQueryModel query, IMapper mapper,
            Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetPageAsync(predicate, query);
                    if (result != null)
                        return new ResponsePagination<TResultModel>(
                            mapper.Map<Pagination<TDbModel>, Pagination<TResultModel>>(result));

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetPageAsync(Expression<Func<TDbModel, bool>> predicate, TQueryModel query,
            Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetPageAsync(predicate, query);
                    if (result != null)
                        return new ResponsePagination<TResultModel>(new Pagination<TResultModel> {

                            Content = AutoMapperUtils.AutoMap<TDbModel, TResultModel>(result.Content),
                            NumberOfElements = result.NumberOfElements,
                            Page = result.Page,
                            Size = result.Size,
                            TotalElements = result.TotalElements,
                            TotalPages = result.TotalPages
                        });

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

    
    }
}