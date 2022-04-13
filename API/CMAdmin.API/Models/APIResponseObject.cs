using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{

    public class ApiResponse
    {
        public int Status { get; set; }
        public String Message { get; set; }
        public object Result { get; set; }       
        public Pagination Paging { get; set; }

        public ApiResponse(int status = 0, string message = Return.Messages.Success, object result = null, Pagination paging = null)
        {
            this.Status = status;
            this.Message = message;
            this.Result = result;
            this.Paging = paging;          
        }
    }

    public class ApiFailResponse
    {
        public int Status { get; set; }
        public String Message { get; set; }
        public Error Error { get; set; }
        public ApiFailResponse(int status = -1, string message = Return.Messages.Fail, Error error = null)
        {
            this.Status = status;
            this.Message = message;
            this.Error = error;
        }
    }

    public class ApiException
    {
        public int Status { get; set; }
        public String Message { get; set; }
        public Error Error { get; set; }
        public ApiException(int status = -2, string message = Return.Messages.Exception, Error error = null)
        {
            this.Status = status;
            this.Message = message;
            this.Error = error;
        }
    }

    public class Pagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string TotalRecordsText { get; set; }
        public List<PageListItem> PageList { get; set; }

        public List<PageListItem> GetPageListItem(int TotalItems, int PageSize)
        {

            int totalPageCount = (int)Math.Ceiling((decimal)TotalItems / PageSize);

            List<PageListItem> pageList = new List<PageListItem>();
            if (totalPageCount > 5)
            {
                for (int i = 1; i <= totalPageCount; i++)
                {
                    PageListItem obj = new PageListItem();
                    obj.key = i;
                    obj.value = i.ToString();
                    pageList.Add(obj);
                }
            }
            else
            {
                PageListItem obj = new PageListItem();
                obj.key = 1;
                obj.value = "1";
                pageList.Add(obj);
            }
            return pageList;
        }
    }

    public class PageListItem
    {
        public int key { get; set; }
        public string value { get; set; }
    }

}
