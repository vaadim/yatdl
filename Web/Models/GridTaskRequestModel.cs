namespace YATDL
{
    public class GridTaskRequestModel
    {
        public GridTaskSearchModel Filter { get; set; }

        public GridTaskSortModel Sort { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public GridTaskRequestModel()
        {
            this.Skip = 0;
            this.Take = 10;
            this.Filter = new GridTaskSearchModel();
            this.Sort = new GridTaskSortModel();
        }


        public class GridTaskSearchModel
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public bool? IsDone { get; set; }

            public bool? IsNow { get; set; }
        }

        public class GridTaskSortModel
        {
            public bool? Created { get; set; }
            public bool? Importance { get; set; }
            
        }
    }
}