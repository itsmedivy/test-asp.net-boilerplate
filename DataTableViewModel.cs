using Newtonsoft.Json;

namespace EIOMS.Models.DataTableViewModels
{
    public class DataTableRequest
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }
        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }
        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        [JsonProperty(PropertyName = "order")]
        public DataTableOrder[] Order { get; set; }
        [JsonProperty(PropertyName = "columns")]
        public DataTableColumn[] Columns { get; set; }
        [JsonProperty(PropertyName = "search")]
        public DataTableSearch Search { get; set; }
    }

    public class ReportDataTableRequest
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }
        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }
        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        [JsonProperty(PropertyName = "order")]
        public DataTableOrder[] Order { get; set; }
        [JsonProperty(PropertyName = "columns")]
        public DataTableColumn[] Columns { get; set; }
        [JsonProperty(PropertyName = "search")]
        public DataTableSearch Search { get; set; }

        [JsonProperty(PropertyName = "searchcol")]
        public string SearchCol { get; set; }
        [JsonProperty(PropertyName = "searchtext")]
        public string SearchText { get; set; }

        [JsonProperty(PropertyName = "betweencol")]
        public string BetweenCol { get; set; }
        [JsonProperty(PropertyName = "startdate")]
        public string StartDate { get; set; }
        [JsonProperty(PropertyName = "enddate")]
        public string EndDate { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

    }

    public class DataTableSearch
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "regex")]
        public bool Regex { get; set; }
    }
    public class DataTableColumn
    {
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "searchable")]
        public bool Searchable { get; set; }
        [JsonProperty(PropertyName = "Orderable")]
        public bool Orderable { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "search")]
        public DataTableSearch Search { get; set; }
    }
    public class DataTableOrder
    {
        [JsonProperty(PropertyName = "column")]
        public int Column { get; set; }
        [JsonProperty(PropertyName = "dir")]
        public string Dir { get; set; }
    }

    public class DataTableResponse
    {
        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object[] data { get; set; }
        public string error { get; set; }
    }
}
