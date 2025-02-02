﻿namespace Application.Parameters
{
    public class RequestParameter
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsExport { get; set; }
        public string OrderBy { get; set; }

        public RequestParameter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
            this.IsExport = false;
            this.OrderBy = "CreatedOn desc";
        }

        public RequestParameter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}